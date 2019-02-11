using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using VisualStudioComparisonTools;

[assembly: XmlConfigurator(ConfigFileExtension = "log4net", Watch = true)]

namespace VSCompTools
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSCompToolsPkgString)]
    public sealed class VSCompToolsPackage : Package
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ComparisonConfig config = new ComparisonConfig();


        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VSCompToolsPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            var log4netconfig = config.ConfigPath + Path.DirectorySeparatorChar + "VisualStudioComparisonTools.dll.log4net";
            if (File.Exists(log4netconfig))
            {
                var configFile = new FileInfo(log4netconfig);
                XmlConfigurator.Configure(configFile);
            }
            else
            {
                XmlConfigurator.Configure();
            }
            
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            DTE _applicationObject = GetService(typeof(SDTE)) as DTE;

            try
            {
                log.Debug("Loading config");
                config.Load(_applicationObject.FullName);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                var menuCommandID = new CommandID(GuidList.guidVSCompToolsCmdSet, (int)PkgCmdIDList.cmdidCompareSelected);
                //var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                mcs.AddCommand( menuItem );

                var menuCommandIDFolder = new CommandID(GuidList.guidVSCompToolsCmdSet, (int)PkgCmdIDList.cmdidCompareSelectedFolder);
                //var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                var menuItemFolder = new OleMenuCommand(MenuItemCallback, menuCommandIDFolder);
                menuItemFolder.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                mcs.AddCommand(menuItemFolder);

                var menuCommandIDEditor = new CommandID(GuidList.guidVSCompToolsCmdSet, (int)PkgCmdIDList.cmdidCompareSelectedEditor);
                //var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                var menuItemEditor = new OleMenuCommand(MenuItemCallback, menuCommandIDEditor);
                menuItemEditor.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                mcs.AddCommand(menuItemEditor);

                var menuCommandIDMultiProj = new CommandID(GuidList.guidVSCompToolsCmdSet, (int)PkgCmdIDList.cmdidCompareSelectedMultiProj);
                //var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                var menuItemMultiProj = new OleMenuCommand(MenuItemCallback, menuCommandIDMultiProj);
                menuItemMultiProj.BeforeQueryStatus += menuItem_BeforeQueryStatus;
                mcs.AddCommand(menuItemMultiProj);
            }
        }
        #endregion


        void menuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            var myCommand = sender as OleMenuCommand;
            if (myCommand != null)
            {
                //DTE dte = GetService(typeof(SDTE)) as DTE;

                ////One file -> Compare with clipboard
                //myCommand.Text = "Compare with clipboard";
                ////One folder -> Hidden
                ////myCommand.Text = "New Text";
                ////Two files -> Compare files
                //myCommand.Text = "Compare files";
                ////Two folders -> Compare folders
                //myCommand.Text = "Compare folders";
                
            }
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            DTE _applicationObject = GetService(typeof(SDTE)) as DTE;

            // Show a Message Box to prove we were here
            //IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            //EnvDTE.
            //(UIHierarchy)uiShell.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object;
            
            //Guid clsid = Guid.Empty;
            //int result;
            //Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
            //           0,
            //           ref clsid,
            //           "VSComparisonTools",
            //           string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.ToString()),
            //           string.Empty,
            //           0,
            //           OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
            //           OLEMSGICON.OLEMSGICON_INFO,
            //           0,        // false
            //           out result));

            //log.Debug("Command found (" + commandName + ")");
            log.Debug("_applicationObject.SelectedItems " + _applicationObject.SelectedItems.Count);

            //if (_applicationObject.SelectedItems.Count > 0)
            {
                log.Debug("Saving all documents");
                _applicationObject.Documents.SaveAll();
                log.Debug("Saved all documents");

                string filePath1 = "";
                if (_applicationObject.SelectedItems.Count >= 1)
                {
                    log.Debug("_applicationObject=" + _applicationObject);
                    log.Debug("_applicationObject.SelectedItems=" + _applicationObject.SelectedItems);
                    log.Debug("_applicationObject.SelectedItems.Item(1)=" + _applicationObject.SelectedItems.Item(1));
                    log.Debug("_applicationObject.SelectedItems.Item(1).ProjectItem=" + _applicationObject.SelectedItems.Item(1).ProjectItem);
                    log.Debug("_applicationObject.SelectedItems.Item(1).ProjectItem.FileNames[1]=" + _applicationObject.SelectedItems.Item(1).ProjectItem.FileNames[1]);
                    if (_applicationObject.SelectedItems.Item(1).ProjectItem != null)
                    {
                        filePath1 = _applicationObject.SelectedItems.Item(1).ProjectItem.FileNames[1];
                    }
                    log.Debug("Document1 found. Comparing " + filePath1 + " to clipboard");
                }

                string filePath2 = "";
                if (_applicationObject.SelectedItems.MultiSelect && _applicationObject.SelectedItems.Count >= 2)
                {
                    log.Debug("_applicationObject=" + _applicationObject);
                    log.Debug("_applicationObject.SelectedItems=" + _applicationObject.SelectedItems);
                    log.Debug("_applicationObject.SelectedItems.Item(2)=" + _applicationObject.SelectedItems.Item(2));
                    log.Debug("_applicationObject.SelectedItems.Item(2).Name=" + _applicationObject.SelectedItems.Item(2).Name);
                    log.Debug("_applicationObject.SelectedItems.Item(2).Project=" + _applicationObject.SelectedItems.Item(2).Project);
                    log.Debug("_applicationObject.SelectedItems.Item(2).Project.FileName=" + (_applicationObject.SelectedItems.Item(2).Project != null ? _applicationObject.SelectedItems.Item(2).Project.FileName : ""));
                    log.Debug("_applicationObject.SelectedItems.Item(2).ProjectItem=" + _applicationObject.SelectedItems.Item(2).ProjectItem);
                    log.Debug("_applicationObject.SelectedItems.Item(2).ProjectItem.FileNames[1]=" + (_applicationObject.SelectedItems.Item(2).ProjectItem != null ? _applicationObject.SelectedItems.Item(2).ProjectItem.FileNames[1] : ""));
                    if (_applicationObject.SelectedItems.Item(2).ProjectItem != null)
                    {
                        filePath2 = _applicationObject.SelectedItems.Item(2).ProjectItem.FileNames[1];
                    }
                    log.Debug("Document2 found. Comparing " + filePath2 + " to clipboard");
                }

                var workerProcess = new ComparisonWorkerProcess(_applicationObject, config);


                if (!_applicationObject.SelectedItems.MultiSelect)
                {
                    var myCommand = sender as OleMenuCommand;
                    if (myCommand != null && myCommand.CommandID.Guid == GuidList.guidVSCompToolsCmdSet &&
                        myCommand.CommandID.ID == (int) PkgCmdIDList.cmdidCompareSelectedEditor)
                    {
                        workerProcess.TextSelection = GetActiveTextSelection(_applicationObject);
                        workerProcess.ComparisonFilePath1 = GetActiveDocumentFilePath(_applicationObject);
                    }
                    else
                    {
                        workerProcess.ComparisonFilePath1 = filePath1;
                    }

                    bool isFileOnClipboard;
                    string clipboardText = GetClipboard(out isFileOnClipboard);
                    if (!isFileOnClipboard || !File.Exists(clipboardText))
                    {
                        workerProcess.ClipboardText = clipboardText;
                    }
                    else
                    {
                        workerProcess.ComparisonFilePath2 = clipboardText;
                    }
                }
                else
                {
                    if (Directory.Exists(filePath1) && Directory.Exists(filePath2) ||
                        File.Exists(filePath1) && File.Exists(filePath2))
                    {
                        workerProcess.ComparisonFilePath1 = filePath1;
                        workerProcess.ComparisonFilePath2 = filePath2;
                    }
                    else
                    {
                        throw new Exception("Either one of selected items don't exist for some reason!");
                    }
        }

                log.Debug("Starting comparison process");

                var workerThread = new System.Threading.Thread(workerProcess.OpenComparisonProcess);
                workerThread.Start();

            }
        }

        public TextSelection GetActiveTextSelection(DTE _applicationObject)
        {
            return (TextSelection)_applicationObject.ActiveDocument.Selection;
        }

        public string GetActiveDocumentFilePath(DTE _applicationObject)
        {
            return _applicationObject.ActiveDocument.FullName;
        }

        public string GetClipboard(out bool isFile)
        {
            log.Debug("Getting Clipboard: " + Clipboard.GetDataObject());
            isFile = false;

            if (Clipboard.ContainsFileDropList())
            {
                log.Debug("Found files in clipboard");
                StringCollection fileDropList = Clipboard.GetFileDropList();
                if (fileDropList != null && fileDropList.Count > 0)
                {
                    string fileDrop = fileDropList[0];
                    log.Debug("There are total of " + fileDropList.Count + " files in clipboard. Selecting first: " + fileDrop);
                    isFile = true;
                    return fileDrop;
                }
            }
            if (Clipboard.ContainsText())
            {
                log.Debug("Using text representation of clipboard");
                return Clipboard.GetText();
            }
            log.Debug("Nothing interesting in the clipboard");
            return "";
        }
    }
}
