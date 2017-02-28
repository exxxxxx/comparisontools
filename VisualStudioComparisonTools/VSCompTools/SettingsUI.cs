using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace VSCompTools
{
    public class SettingsUI : DialogPage
    {
        private string _ComparisonToolPath;
        private bool _UseGlobalTempFolder;

        public event EventHandler ComparisonToolPathChanged;
        public event EventHandler UseGlobalTempFolderChanged;

        [Category("Settings")]
        [DisplayName("Comparison tool path")]
        [Description("Absolute path to the comparison utility")]
        [DefaultValue("C:\\Program Files (x86)\\WinMerge\\WinMergeU.exe")]
        public string ComparisonToolPath
        {
            get { return _ComparisonToolPath; }
            set
            {
                var changed = _ComparisonToolPath != value;
                _ComparisonToolPath = value;

                if (changed && ComparisonToolPathChanged != null)
                    ComparisonToolPathChanged(this, EventArgs.Empty);
            }
        }

        [Category("Settings")]
        [DisplayName("Use global temp folder")]
        [Description("Use global temp folder instead of the local project folder")]
        [DefaultValue(true)]
        public bool UseGlobalTempFolder
        {
            get { return _UseGlobalTempFolder; }
            set
            {
                var changed = _UseGlobalTempFolder != value;
                _UseGlobalTempFolder = value;

                if (changed && UseGlobalTempFolderChanged != null)
                    UseGlobalTempFolderChanged(this, EventArgs.Empty);
            }
        }
    }
}