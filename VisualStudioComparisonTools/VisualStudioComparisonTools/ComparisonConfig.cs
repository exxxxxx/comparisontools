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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
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
        public bool ShowInVS2012 = false;
        public bool ShowInVS2013 = false;

        private string configFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar + "Visual Studio Comparison Tools" + Path.DirectorySeparatorChar + "config.xml";

        public ComparisonConfig()
        {
        }

        public void Load()
        {
            log.Debug("Loading config");

            if (!File.Exists(configFile))
            {
                log.Debug("Config file did not exist. Creating new.");
                Save();
            }
            DataSet config = new DataSet();
            if (File.Exists(configFile))
            {
                log.Debug("Found config file. Reading xml");
                config.ReadXml(configFile);
            }
            if (config.Tables["Configuration"] != null &&
                config.Tables["Configuration"].Rows.Count > 0)
            {
                log.Debug("Configuration table found");
                if (config.Tables["Configuration"].Columns.IndexOf("ComparisonToolPath") >= 0 && 
                    config.Tables["Configuration"].Rows[0]["ComparisonToolPath"] != null)
                {
                    log.Debug("Comparisontoolpath found");
                    ComparisonToolPath = config.Tables["Configuration"].Rows[0]["ComparisonToolPath"].ToString();
                    log.Debug("Comparisontoolpath is " + ComparisonToolPath);
                    if (!File.Exists(ComparisonToolPath))
                    {
                        log.Debug("The file in comparison toolpath not found! Checking if winmerge exists.");
                        var winMergeKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\WinMergeU.exe");
                        if (winMergeKey != null)
                        {
                            log.Debug("Winmerge found from registry");
                            var path = winMergeKey.GetValue("") as string;
                            if (path != null && File.Exists(path))
                            {
                                log.Debug("Winmerge exe path found from file system. Using that");
                                ComparisonToolPath = path;
                            }
                        }

                        if (!File.Exists(ComparisonToolPath))
                        {
                            log.Debug("Still could not find comparison toolpath exe from file system. Try to change program files x86 to no x86");
                            ComparisonToolPath = ComparisonToolPath.Replace(":\\Program Files (x86)\\", ":\\Program Files\\");
                            if (!File.Exists(ComparisonToolPath))
                            {
                                log.Debug("Nope. Try the other way around");
                                ComparisonToolPath = ComparisonToolPath.Replace(":\\Program Files\\", ":\\Program Files (x86)\\");
                            }
                        }
                    }

                    log.Debug("Resulting comparisontoolpath is " + ComparisonToolPath);
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
                if (config.Tables["Configuration"].Columns.IndexOf("ShowInVS2012") >= 0 &&
                    config.Tables["Configuration"].Rows[0]["ShowInVS2012"] != null)
                {
                    UseGlobalTempFolder = config.Tables["Configuration"].Rows[0]["ShowInVS2012"].ToString().StartsWith("t");
                }
                if (config.Tables["Configuration"].Columns.IndexOf("ShowInVS2013") >= 0 &&
                    config.Tables["Configuration"].Rows[0]["ShowInVS2013"] != null)
                {
                    UseGlobalTempFolder = config.Tables["Configuration"].Rows[0]["ShowInVS2013"].ToString().StartsWith("t");
                }
            }

            log.Debug("Saving config ");
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
            table.Columns.Add("ShowInVS2012");
            table.Columns.Add("ShowInVS2013");
            table.Rows.Add(new object[] { ComparisonToolPath, ComparisonToolArguments, ComparisonToolClipboardTitle, ComparisonToolSelectionTitle, UseGlobalTempFolder ? "true" : "false", ShowInVS2012 ? "true" : "false", ShowInVS2013 ? "true" : "false" });

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
