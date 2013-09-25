using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using Microsoft.Win32;

namespace VisualStudioComparisonTools
{
    public class ComparisonConfig
    {
        public const string REPLACE_FILENAME = "[%SELECTION_FILENAME%]";
        public const string REPLACE_FILE_PARAM1 = "[%File1%]";
        public const string REPLACE_FILE_PARAM2 = "[%File2%]";
        public const string REPLACE_SELECTION_TITLE = "[%SELECTION_TITLE%]";
        public const string REPLACE_CLIPBOARD_TITLE = "[%CLIPBOARD_TITLE%]";

        public string ComparisonToolPath = "C:\\Program Files (x86)\\WinMerge\\WinMergeU.exe";
        public string ComparisonToolArguments = "/ub " + REPLACE_SELECTION_TITLE + " " + REPLACE_CLIPBOARD_TITLE + " \"" + REPLACE_FILE_PARAM1 + "\" \"" + REPLACE_FILE_PARAM2 + "\"";
        public string ComparisonToolSelectionTitle = "/dl \"Selection (" + REPLACE_FILENAME + ")\"";
        public string ComparisonToolClipboardTitle = "/wr /dr \"Clipboard (ReadOnly)\"";
        public bool UseGlobalTempFolder = true;

        private string configFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + "Visual Studio Comparison Tools" + Path.DirectorySeparatorChar + "config.xml";

        public ComparisonConfig()
        {
        }

        public void Load()
        {
            if (!File.Exists(configFile))
            {
                Save();
            }
            DataSet config = new DataSet();
            if (File.Exists(configFile))
            {
                config.ReadXml(configFile);
            }
            if (config.Tables["Configuration"] != null &&
                config.Tables["Configuration"].Rows.Count > 0)
            {
                if (config.Tables["Configuration"].Columns.IndexOf("ComparisonToolPath") >= 0 && 
                    config.Tables["Configuration"].Rows[0]["ComparisonToolPath"] != null)
                {
                    ComparisonToolPath = config.Tables["Configuration"].Rows[0]["ComparisonToolPath"].ToString();
                    if (!File.Exists(ComparisonToolPath))
                    {
                        var winMergeKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\WinMergeU.exe");
                        if (winMergeKey != null)
                        {
                            var path = winMergeKey.GetValue("") as string;
                            if (path != null && File.Exists(path))
                            {
                                ComparisonToolPath = path;
                            }
                        }

                        if (!File.Exists(ComparisonToolPath))
                        {
                            ComparisonToolPath = ComparisonToolPath.Replace(":\\Program Files (x86)\\", ":\\Program Files\\");
                            if (!File.Exists(ComparisonToolPath))
                            {
                                ComparisonToolPath = ComparisonToolPath.Replace(":\\Program Files\\", ":\\Program Files (x86)\\");
                            }
                        }
                    }
                }
                if (config.Tables["Configuration"].Columns.IndexOf("ComparisonToolArguments") >= 0 && 
                    config.Tables["Configuration"].Rows[0]["ComparisonToolArguments"] != null)
                {
                    ComparisonToolArguments = config.Tables["Configuration"].Rows[0]["ComparisonToolArguments"].ToString();
                }
                if (config.Tables["Configuration"].Columns.IndexOf("ComparisonToolClipboardTitle") >= 0 && 
                    config.Tables["Configuration"].Rows[0]["ComparisonToolClipboardTitle"] != null)
                {
                    ComparisonToolClipboardTitle = config.Tables["Configuration"].Rows[0]["ComparisonToolClipboardTitle"].ToString();
                }
                if (config.Tables["Configuration"].Columns.IndexOf("ComparisonToolSelectionTitle") >= 0 && 
                    config.Tables["Configuration"].Rows[0]["ComparisonToolSelectionTitle"] != null)
                {
                    ComparisonToolSelectionTitle = config.Tables["Configuration"].Rows[0]["ComparisonToolSelectionTitle"].ToString();
                }
                if (config.Tables["Configuration"].Columns.IndexOf("UseGlobalTempFolder") >= 0 && 
                    config.Tables["Configuration"].Rows[0]["UseGlobalTempFolder"] != null)
                {
                    UseGlobalTempFolder = config.Tables["Configuration"].Rows[0]["UseGlobalTempFolder"].ToString().StartsWith("t");
                }
            }

            Save();
        }

        public void Save()
        {
            DataSet config = new DataSet("VisualStudioComparisonTools");
            DataTable table = config.Tables.Add("Configuration");
            table.Columns.Add("ComparisonToolPath");
            table.Columns.Add("ComparisonToolArguments");
            table.Columns.Add("ComparisonToolClipboardTitle");
            table.Columns.Add("ComparisonToolSelectionTitle");
            table.Columns.Add("UseGlobalTempFolder");
            table.Rows.Add(new object[] { ComparisonToolPath, ComparisonToolArguments, ComparisonToolClipboardTitle, ComparisonToolSelectionTitle, UseGlobalTempFolder ? "true" : "false" });

            string configDirectory = Path.GetDirectoryName(configFile);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            if (File.Exists(configFile))
            {
                File.Delete(configFile);
            }
            config.WriteXml(configFile);
        }
    }
}
