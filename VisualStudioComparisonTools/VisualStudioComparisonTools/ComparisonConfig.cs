﻿using System;
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
        public const string ComparisonToolPath_BCOMP = "C:\\Program Files (x86)\\Beyond Compare 3\\BComp.exe";
        public const string ComparisonToolArguments_BCOMP = "" + REPLACE_SELECTION_TITLE + " " + REPLACE_CLIPBOARD_TITLE + " \"" + REPLACE_FILE_PARAM1 + "\" \"" + REPLACE_FILE_PARAM2 + "\"";
        public const string ComparisonToolSelectionTitle_BCOMP = "/lefttitle=\"Selection (" + REPLACE_FILENAME + ")\"";
        public const string ComparisonToolClipboardTitle_BCOMP = "/rro /righttitle=\"Clipboard (ReadOnly)\"";
        public const string ComparisonToolPath_WinMerge = "C:\\Program Files (x86)\\WinMerge\\WinMergeU.exe";
        public const string ComparisonToolArguments_WinMerge = "/u " + REPLACE_SELECTION_TITLE + " " + REPLACE_CLIPBOARD_TITLE + " \"" + REPLACE_FILE_PARAM1 + "\" \"" + REPLACE_FILE_PARAM2 + "\"";
        public const string ComparisonToolSelectionTitle_WinMerge = "/dl \"Selection (" + REPLACE_FILENAME + ")\"";
        public const string ComparisonToolClipboardTitle_WinMerge = "/wr /dr \"Clipboard (ReadOnly)\"";
        public const string ComparisonToolPath_VSDiff = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\Common7\\IDE\\devenv.exe";
        public const string ComparisonToolArguments_VSDiff = "/diff \"" + REPLACE_FILE_PARAM1 + "\" \"" + REPLACE_FILE_PARAM2 + "\" " + REPLACE_SELECTION_TITLE + " " + REPLACE_CLIPBOARD_TITLE + "";
        public const string ComparisonToolSelectionTitle_VSDiff = "\"Selection (" + REPLACE_FILENAME + ")\"";
        public const string ComparisonToolClipboardTitle_VSDiff = "\"Clipboard (ReadOnly)\"";

        public string ComparisonToolPath = ComparisonToolPath_BCOMP;
        public string ComparisonToolArguments = ComparisonToolArguments_BCOMP;
        public string ComparisonToolSelectionTitle = ComparisonToolSelectionTitle_BCOMP;
        public string ComparisonToolClipboardTitle = ComparisonToolClipboardTitle_BCOMP;
        public bool UseGlobalTempFolder = true;
        public bool ShowInVS2012 = false;
        public bool ShowInVS2013 = false;

        public readonly string ConfigPath;
        public readonly string ConfigFile;

        public ComparisonConfig()
        {
            ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Visual Studio Comparison Tools";
            ConfigFile = ConfigPath + Path.DirectorySeparatorChar + "config.xml";
        }

        public void Load(string executingVSPath)
        {
            log.Debug("Loading config");

            if (!File.Exists(ConfigFile))
            {
                log.Debug("Config file did not exist. Creating new.");
                Save();
            }
            DataSet config = new DataSet();
            if (File.Exists(ConfigFile))
            {
                log.Debug("Found config file. Reading xml");
                config.ReadXml(ConfigFile);
            }
            if (config.Tables["Configuration"] != null &&
                config.Tables["Configuration"].Rows.Count > 0)
            {
                bool compToolChanged = false;
                log.Debug("Configuration table found");
                if (config.Tables["Configuration"].Columns.IndexOf("ComparisonToolPath") >= 0 && 
                    config.Tables["Configuration"].Rows[0]["ComparisonToolPath"] != null)
                {
                    log.Debug("Comparisontoolpath found");
                    ComparisonToolPath = config.Tables["Configuration"].Rows[0]["ComparisonToolPath"].ToString();
                    log.Debug("Comparisontoolpath is " + ComparisonToolPath);
                    if (!File.Exists(ComparisonToolPath))
                    {
                        compToolChanged = true;

                        log.Debug("The file in comparison toolpath not found! Checking if beyond compare exists.");
                        var bcompareKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\BCompare.exe");
                        if (bcompareKey != null)
                        {
                            log.Debug("Beyond Compare found from registry");
                            var path = bcompareKey.GetValue("") as string;
                            if (path != null && File.Exists(path))
                            {
                                log.Debug("Beyond Compare exe path found from file system. Using that");
                                path = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + "BComp.exe";
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

                        if (!File.Exists(ComparisonToolPath))
                        {
                            ComparisonToolPath = ComparisonToolPath_BCOMP;

                            log.Debug("Still could not find comparison toolpath exe from file system. Try to change program files x86 to no x86");
                            ComparisonToolPath = ComparisonToolPath.Replace(":\\Program Files (x86)\\", ":\\Program Files\\");
                            if (!File.Exists(ComparisonToolPath))
                            {
                                log.Debug("Nope. Try the other way around");
                                ComparisonToolPath = ComparisonToolPath.Replace(":\\Program Files\\", ":\\Program Files (x86)\\");
                            }
                        }
                        
                        ComparisonToolArguments = ComparisonToolArguments_BCOMP;
                        ComparisonToolSelectionTitle = ComparisonToolSelectionTitle_BCOMP;
                        ComparisonToolClipboardTitle = ComparisonToolClipboardTitle_BCOMP;

                        if (!File.Exists(ComparisonToolPath))
                        {
                            ComparisonToolPath = ComparisonToolPath_WinMerge;

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
                            ComparisonToolArguments = ComparisonToolArguments_WinMerge;
                            ComparisonToolSelectionTitle = ComparisonToolSelectionTitle_WinMerge;
                            ComparisonToolClipboardTitle = ComparisonToolClipboardTitle_WinMerge;
                        }

                        if (!File.Exists(ComparisonToolPath))
                        {
                            log.Debug("The file in comparison toolpath not found! Checking if devenv exists.");

                            ComparisonToolPath = executingVSPath;
                            if (!File.Exists(ComparisonToolPath))
                            {
                                var devenvKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\devenv.exe");
                                if (devenvKey != null)
                                {
                                    log.Debug("DevEnv found from registry");
                                    var path = devenvKey.GetValue("") as string;
                                    if (path != null && File.Exists(path))
                                    {
                                        log.Debug("DevEnv exe path found from file system. Using that");
                                        path = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar + "devenv.exe";
                                        ComparisonToolPath = path;
                                    }
                                }
                            }

                            ComparisonToolArguments = ComparisonToolArguments_VSDiff;
                            ComparisonToolSelectionTitle = ComparisonToolSelectionTitle_VSDiff;
                            ComparisonToolClipboardTitle = ComparisonToolClipboardTitle_VSDiff;
                        }

                    }

                    log.Debug("Resulting comparisontoolpath is " + ComparisonToolPath);
                }
                if (!compToolChanged)
                {
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
                }
                if (config.Tables["Configuration"].Columns.IndexOf("UseGlobalTempFolder") >= 0 &&
                    config.Tables["Configuration"].Rows[0]["UseGlobalTempFolder"] != null)
                {
                    UseGlobalTempFolder = config.Tables["Configuration"].Rows[0]["UseGlobalTempFolder"].ToString().StartsWith("t");
                }
                if (config.Tables["Configuration"].Columns.IndexOf("ShowInVS2012") >= 0 &&
                    config.Tables["Configuration"].Rows[0]["ShowInVS2012"] != null)
                {
                    ShowInVS2012 = config.Tables["Configuration"].Rows[0]["ShowInVS2012"].ToString().StartsWith("t");
                }
                if (config.Tables["Configuration"].Columns.IndexOf("ShowInVS2013") >= 0 &&
                    config.Tables["Configuration"].Rows[0]["ShowInVS2013"] != null)
                {
                    ShowInVS2013 = config.Tables["Configuration"].Rows[0]["ShowInVS2013"].ToString().StartsWith("t");
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

            string configDirectory = Path.GetDirectoryName(ConfigFile);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            if (File.Exists(ConfigFile))
            {
                File.Delete(ConfigFile);
            }
            config.WriteXml(ConfigFile);
        }
    }
}
