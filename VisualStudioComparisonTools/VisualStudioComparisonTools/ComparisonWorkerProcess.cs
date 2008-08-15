///**
/// Copyright 2008 Mikko Halttunen
/// This program is free software; you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation; version 2 of the License.
/// 
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with this program; if not, write to the Free Software Foundation, Inc.,
/// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
/// 
///**

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE;
using EnvDTE80;
using log4net;

namespace VisualStudioComparisonTools
{
    public class ComparisonWorkerProcess
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private DTE2 _applicationObject;
        private TextSelection textSelection = null;
        private string clipboardText = null;
        private string comparisonFilePath1 = null;
        private string comparisonFilePath2 = null;

        public ComparisonWorkerProcess(DTE2 applicationObject)
        {
            _applicationObject = applicationObject;
        }

        private const int LINE_TO_ARRAY_CONVERSION = 1;

        public TextSelection TextSelection
        {
            get { return textSelection; }
            set { textSelection = value; }
        }

        public string ClipboardText
        {
            get { return clipboardText; }
            set { clipboardText = value; }
        }

        public string ComparisonFilePath1
        {
            get { return comparisonFilePath1; }
            set { comparisonFilePath1 = value; }
        }

        public string ComparisonFilePath2
        {
            get { return comparisonFilePath2; }
            set { comparisonFilePath2 = value; }
        }

        public string[] CreateTempFiles()
        {
            string workDirPath = null;
            if (_applicationObject != null && _applicationObject.Solution != null && 
                !string.IsNullOrEmpty(_applicationObject.Solution.FileName))
            {
                workDirPath = Path.GetDirectoryName(_applicationObject.Solution.FileName);
            }
            if (string.IsNullOrEmpty(workDirPath))
            {
                workDirPath = Path.GetTempPath();
            }

            string workPath = workDirPath + Path.DirectorySeparatorChar + ".compare";
            if (!Directory.Exists(workPath))
            {
                Directory.CreateDirectory(workPath);
            }

            int index = 0;
            string selectionFile;
            if (IsFirstFileReal())
            {
                selectionFile = ComparisonFilePath1;
            }
            else
            {
                do
                {
                    selectionFile = workPath + Path.DirectorySeparatorChar + "endresult_" + index;
                    index++;
                } while (File.Exists(selectionFile));

                using (StreamWriter sw = new StreamWriter(selectionFile))
                {
                    sw.Write(textSelection.Text);
                }
            }

            string clipboardFile;
            if (IsSecondFileReal())
            {
                clipboardFile = ComparisonFilePath2;
            }
            else
            {
                do
                {
                    clipboardFile = workPath + Path.DirectorySeparatorChar + "clipboard_" + index;
                    index++;
                } while (File.Exists(clipboardFile));

                using (StreamWriter sw = new StreamWriter(clipboardFile))
                {
                    sw.Write(clipboardText);
                }
            }

            return new [] { selectionFile, clipboardFile };
        }

        private bool IsFirstFileReal()
        {
            return textSelection == null || string.IsNullOrEmpty(textSelection.Text);
        }

        private bool IsSecondFileReal()
        {
            return clipboardText == null;
        }

        public void OpenComparisonProcess()
        {
            if (log.IsInfoEnabled) log.Info("Start");

            string[] tempFiles = null;
            try
            {
                tempFiles = CreateTempFiles();
                if (log.IsDebugEnabled) log.Debug("Read temp files. File1=" + tempFiles[0] + " file2=" + tempFiles[1]);

                string[] originalFileText = null;
                string originalSelection = null;
                if (!IsFirstFileReal())
                {
                    originalFileText = File.ReadAllLines(ComparisonFilePath1);
                    originalSelection = File.ReadAllText(tempFiles[0]);
                }

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = "C:\\Program Files\\WinMerge\\WinMergeU.exe";
                string leftTitle = "";
                string rightTitle = "";
                if (!IsFirstFileReal())
                {
                    if (log.IsDebugEnabled) log.Debug("Using temp file1 for comparison");

                    leftTitle = "/dl \"Selection (" + Path.GetFileName(ComparisonFilePath1) + ")\"";
                }
                else
                {
                    if (log.IsDebugEnabled) log.Debug("Using real file1 for comparison");
                }
                if (!IsSecondFileReal())
                {
                    if (log.IsDebugEnabled) log.Debug("Using temp file2 for comparison");

                    rightTitle = "/wr /dr \"Clipboard (ReadOnly)\"";
                }
                else
                {
                    if (log.IsDebugEnabled) log.Debug("Using real file2 for comparison");
                }
                proc.StartInfo.Arguments = "/ub " + leftTitle + " " + rightTitle + " \"" +
                                           tempFiles[0] + "\" \"" + tempFiles[1] + "\"";
                if (log.IsDebugEnabled) log.Debug("Using arguments: \"" + proc.StartInfo.Arguments + "\"");

                proc.Start();

                if (log.IsInfoEnabled) log.Info("Waiting for mergetool exit");
                proc.WaitForExit();

                if (!IsFirstFileReal())
                {
                    int topLine = TextSelection.TopPoint.Line;
                    int topColumn = TextSelection.TopPoint.DisplayColumn;
                    int bottomLine = TextSelection.BottomPoint.Line;
                    int bottomColumn = TextSelection.BottomPoint.DisplayColumn;

                    string fileBeginning = GetBeginningOfFile(originalFileText, topLine, topColumn);
                    string resultSelection = File.ReadAllText(tempFiles[0]);
                    string fileEnding = GetEndingOfFile(originalFileText, bottomLine, bottomColumn);

                    if (originalSelection != null && !originalSelection.Equals(resultSelection))
                    {
                        if (log.IsInfoEnabled) log.Info("Changes found");

                        StringBuilder buffer = new StringBuilder();
                        buffer.Append(fileBeginning);
                        buffer.Append(resultSelection);
                        buffer.Append(fileEnding);

                        File.WriteAllText(ComparisonFilePath1, buffer.ToString());

                        if (log.IsInfoEnabled) log.Info("Changes written");
                    }
                    else
                    {
                        if (log.IsInfoEnabled) log.Info("No changes done in selection");
                    }
                }
            }
            catch (Exception e)
            {
                if (tempFiles != null)
                {
                    if (!IsFirstFileReal())
                    {
                        if (log.IsInfoEnabled) log.Info("Deleting temporary file for selection");

                        File.Delete(tempFiles[0]);
                    }
                    if (!IsSecondFileReal())
                    {
                        if (log.IsInfoEnabled) log.Info("Deleting temporary file for clipboard");

                        File.Delete(tempFiles[1]);
                    }
                }

                if (log.IsErrorEnabled) log.Error(e);
                Connect.ShowThreadExceptionDialog(e);
            }
            if (log.IsInfoEnabled) log.Info("End");
        }

        public static string GetBeginningOfFile(string[] originalFileText, int topLine, int topColumn)
        {
            if (log.IsInfoEnabled) log.Info("originalFileText Length=" + originalFileText + " topLine=" + topLine + " topColumn=" + topColumn);

            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < topLine - 1; i++)
            {
                buffer.Append(originalFileText[i] + Environment.NewLine);
            }
            buffer.Append(originalFileText[topLine - LINE_TO_ARRAY_CONVERSION].Substring(0, topColumn - LINE_TO_ARRAY_CONVERSION));

            if (log.IsDebugEnabled) log.Debug(buffer.ToString());

            return buffer.ToString();
        }

        public static string GetEndingOfFile(string[] originalFileText, int bottomLine, int bottomColumn)
        {
            if (log.IsInfoEnabled) log.Info("originalFileText Length=" + originalFileText + " bottomLine=" + bottomLine + " bottomColumn=" + bottomColumn);

            StringBuilder buffer = new StringBuilder();
            if (bottomLine <= originalFileText.Length)
            {
                buffer.Append(originalFileText[bottomLine - LINE_TO_ARRAY_CONVERSION].Substring(bottomColumn - LINE_TO_ARRAY_CONVERSION));
                for (int i = bottomLine; i < originalFileText.Length; i++)
                {
                    buffer.Append(Environment.NewLine + originalFileText[i]);
                }
            }

            if (log.IsDebugEnabled) log.Debug(buffer.ToString());

            return buffer.ToString();
        }
    }
}