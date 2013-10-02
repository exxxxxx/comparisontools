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
using System.Linq;
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

        public ComparisonWorkerProcess(DTE2 applicationObject, ComparisonConfig config)
        {
            _applicationObject = applicationObject;
            Config = config;
        }

        private const int LINE_TO_ARRAY_CONVERSION = 1;

        public ComparisonConfig Config {get;set;}

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
            string workDirPath = GetWorkingDirectoryPath();

            DirectoryInfo di = new DirectoryInfo(workDirPath);
            if (!di.Exists)
            {
                di.Create();
            }

            string selectionFile;
            if (IsFirstFileReal())
            {
                selectionFile = ComparisonFilePath1;
            }
            else
            {
                selectionFile = workDirPath + Path.DirectorySeparatorChar + "endresult_" + Guid.NewGuid() + ".txt";
                File.WriteAllText(selectionFile, textSelection.Text);
            }

            string clipboardFile;
            if (IsSecondFileReal())
            {
                clipboardFile = ComparisonFilePath2;
            }
            else
            {
                clipboardFile = workDirPath + Path.DirectorySeparatorChar + "clipboard_" + Guid.NewGuid() + ".txt";
                File.WriteAllText(clipboardFile, clipboardText);
            }

            return new [] { selectionFile, clipboardFile };
        }

        public void ExecuteCleanup()
        {
            string workDirPath = GetWorkingDirectoryPath();
            
            log.Info("Cleaning working directory \"" + workDirPath + "\"");

            DirectoryInfo di = new DirectoryInfo(workDirPath);
            if (di.Exists)
            {
                List<FileInfo> fileInfos;
                fileInfos = di.GetFiles("endresult_*").Where(fi => fi.LastWriteTimeUtc < DateTime.UtcNow.AddDays(-7)).ToList();
                fileInfos.ForEach(fi =>
                {
                    log.Info("Removing \"" + fi.FullName + "\"");
                    fi.Delete();
                });
                fileInfos = di.GetFiles("clipboard_*").Where(fi => fi.LastWriteTimeUtc < DateTime.UtcNow.AddDays(-7)).ToList();
                fileInfos.ForEach(fi =>
                {
                    log.Info("Removing \"" + fi.FullName + "\"");
                    fi.Delete();
                });
            }
        }

        private string GetWorkingDirectoryPath()
        {
            string workDirPath = "";
            if (!Config.UseGlobalTempFolder && _applicationObject != null && _applicationObject.Solution != null &&
                !string.IsNullOrEmpty(_applicationObject.Solution.FileName))
            {
                workDirPath = Path.GetDirectoryName(_applicationObject.Solution.FileName);
            }
            if (Config.UseGlobalTempFolder || string.IsNullOrEmpty(workDirPath))
            {
                workDirPath = Path.GetTempPath();
            }
            return workDirPath + Path.DirectorySeparatorChar + "_VisualStudioComparisonTools";;
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
            log.Info("Start");

            try
            {
                ExecuteCleanup();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            string[] tempFiles = null;
            try
            {
                tempFiles = CreateTempFiles();
                log.Debug("Read temp files. File1=" + tempFiles[0] + " file2=" + tempFiles[1]);

                string[] originalFileText = null;
                string originalSelection = null;
                if (!IsFirstFileReal())
                {
                    originalFileText = File.ReadAllLines(ComparisonFilePath1);
                    originalSelection = File.ReadAllText(tempFiles[0]);

                    log.Debug("First file not \"real\": originalFileText=" + ComparisonFilePath1 + " tempFiles[0]=" + tempFiles[0]);
                }

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = Config.ComparisonToolPath;

                if (!File.Exists(Config.ComparisonToolPath))
                {
                    log.Fatal("Could not find the comparison tool from path: " + Config.ComparisonToolPath);
                    throw new Exception("Could not find the comparison tool from path: \"" + Config.ComparisonToolPath + "\" \nWinMerge can be downloaded from \"http://winmerge.org\"!");
                }
                else
                {
                    log.Debug("Found comparison tool from path: \"" + Config.ComparisonToolPath);
                }

                string leftTitle = "";
                string rightTitle = "";
                if (!IsFirstFileReal())
                {
                    log.Debug("Using temp file1 for comparison");

                    leftTitle = Config.ComparisonToolSelectionTitle.Replace(ComparisonConfig.REPLACE_FILENAME, Path.GetFileName(ComparisonFilePath1));
                }
                else
                {
                    log.Debug("Using real file1 for comparison");
                }
                if (!IsSecondFileReal())
                {
                    log.Debug("Using temp file2 for comparison");

                    rightTitle = Config.ComparisonToolClipboardTitle;
                }
                else
                {
                    log.Debug("Using real file2 for comparison");
                }
                
                proc.StartInfo.Arguments = Config.ComparisonToolArguments.Replace(ComparisonConfig.REPLACE_SELECTION_TITLE, leftTitle).Replace(ComparisonConfig.REPLACE_CLIPBOARD_TITLE, rightTitle).Replace(ComparisonConfig.REPLACE_FILE_PARAM1, tempFiles[0]).Replace(ComparisonConfig.REPLACE_FILE_PARAM2, tempFiles[1]);
                log.Debug("Using arguments: \"" + proc.StartInfo.Arguments + "\"");

                if (File.Exists(tempFiles[0]) && File.Exists(tempFiles[1]) ||
                    Directory.Exists(tempFiles[0]) && Directory.Exists(tempFiles[1]))
                {
                    proc.Start();

                    log.Info("Waiting for mergetool exit");
                    proc.WaitForExit();
                }
                else
                {
                    log.Fatal("Could not find files to compare!");
                }

                if (!IsFirstFileReal())
                {
                    int topLine = TextSelection.TopPoint.Line;
                    int topColumn = TextSelection.TopPoint.LineCharOffset;
                    int bottomLine = TextSelection.BottomPoint.Line;
                    int bottomColumn = TextSelection.BottomPoint.LineCharOffset;

                    string fileBeginning = GetBeginningOfFile(originalFileText, topLine, topColumn);
                    string resultSelection = File.ReadAllText(tempFiles[0]);
                    string fileEnding = GetEndingOfFile(originalFileText, bottomLine, bottomColumn);

                    if (originalSelection != null && !originalSelection.Equals(resultSelection))
                    {
                        log.Info("Changes found");

                        StringBuilder buffer = new StringBuilder();
                        buffer.Append(fileBeginning);
                        buffer.Append(resultSelection);
                        buffer.Append(fileEnding);

                        File.WriteAllText(ComparisonFilePath1, buffer.ToString());

                        log.Info("Changes written");
                    }
                    else
                    {
                        log.Info("No changes done in selection");
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);

                if (tempFiles != null)
                {
                    if (!IsFirstFileReal())
                    {
                        log.Info("Deleting temporary file for selection");

                        File.Delete(tempFiles[0]);
                    }
                    if (!IsSecondFileReal())
                    {
                        log.Info("Deleting temporary file for clipboard");

                        File.Delete(tempFiles[1]);
                    }
                }
                Connect.ShowThreadExceptionDialog(e);
            }
            log.Info("End");
        }

        public static string GetBeginningOfFile(string[] originalFileText, int topLine, int topColumn)
        {
            log.Info("GetBeginningOfFile: originalFileText Length=" + originalFileText.Length + " topLine=" + topLine + " topColumn=" + topColumn);
            log.Debug("GetBeginningOfFile: originalFileText=" + string.Join("" + Environment.NewLine, originalFileText));
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < topLine - 1; i++)
            {
                buffer.Append(originalFileText[i] + Environment.NewLine);
            }
            buffer.Append(originalFileText[topLine - LINE_TO_ARRAY_CONVERSION].Substring(0, topColumn - LINE_TO_ARRAY_CONVERSION));

            log.Debug(buffer.ToString());

            return buffer.ToString();
        }

        public static string GetEndingOfFile(string[] originalFileText, int bottomLine, int bottomColumn)
        {
            log.Info("GetEndingOfFile: originalFileText Length=" + originalFileText.Length + " bottomLine=" + bottomLine + " bottomColumn=" + bottomColumn);
            log.Debug("GetEndingOfFile: originalFileText=" + string.Join("" + Environment.NewLine, originalFileText));

            StringBuilder buffer = new StringBuilder();
            if (bottomLine <= originalFileText.Length)
            {
                log.Debug("GetEndingOfFile originalFileText.Length=" + originalFileText.Length + " index=" + (bottomLine - LINE_TO_ARRAY_CONVERSION) + " startindex=" + (bottomColumn - LINE_TO_ARRAY_CONVERSION));
                string substring = originalFileText[bottomLine - LINE_TO_ARRAY_CONVERSION].Substring(bottomColumn - LINE_TO_ARRAY_CONVERSION);
                log.Debug("GetEndingOfFile substring=" + substring);
                buffer.Append(substring);
                for (int i = bottomLine; i < originalFileText.Length; i++)
                {
                    log.Debug("GetEndingOfFile originalFileText.Length=" + originalFileText.Length + " index=" + (bottomLine - LINE_TO_ARRAY_CONVERSION) + " startindex=" + (bottomColumn - LINE_TO_ARRAY_CONVERSION) + " substring=" + originalFileText[i]);
                    buffer.Append(Environment.NewLine + originalFileText[i]);
                }
            }

            log.Debug(buffer.ToString());

            return buffer.ToString();
        }
    }
}