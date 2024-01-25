using Microsoft.Deployment.WindowsInstaller;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLeecher.Setup.Gui.Services;

namespace TwitchLeecher.Setup.Gui.ViewModels
{
    internal class CustomizeDlgVM : DlgBaseVM
    {
        #region Constructors

        public CustomizeDlgVM(SetupApplication bootstrapper, IGuiService guiService)
            : base(bootstrapper, guiService)
        { }

        #endregion Constructors

        #region Properties

        public string InstallDir
        {
            get
            {
                return _bootstrapper.InstallDir;
            }
            set
            {
                _bootstrapper.InstallDir = value;
                FirePropertyChanged("InstallDir");
            }
        }
        public string ErrorInfo
        {
            get
            {
                    string errorstring = "";
                    foreach (string error in GetErrors("InstallDir"))
                    {
                        errorstring += error;
                    }
                    return errorstring;
            }
        }
        public string FeatureTLSize
        {
            get
            {
                return _bootstrapper.FeatureTLSize;
            }
        }

        #endregion Properties

        #region Methods

        protected override void Validate(string propertyName = null)
        {
            base.Validate(propertyName);

            string currentProperty = "InstallDir";

            if (string.IsNullOrWhiteSpace(propertyName) || propertyName == currentProperty)
            {
                string installDir = _bootstrapper.InstallDir;

                bool pathOk = false;
                bool pathEmpty = false;

                if (!string.IsNullOrWhiteSpace(installDir))
                {
                    try
                    {
                        bool  isokchinese= IsValidPath(installDir);
                        if (!isokchinese)
                        {
                            AddError(currentProperty, "The path must only contain a-zA-Z():\\ .");
                            return;
                        }

                        Path.GetFullPath(installDir);

                        if (Path.IsPathRooted(installDir))
                        {
                            pathOk = true;

                            if (Directory.Exists(installDir))
                            {
                                pathEmpty = !Directory.EnumerateFileSystemEntries(installDir).Any();
                            }
                            else
                            {
                                pathEmpty = true;
                            }
                        }
                    }
                    catch
                    {
                        // Parse error
                    }
                }

                if (!pathOk)
                {
                    AddError(currentProperty, "Please provide a valid path!");
                }

                if (!pathEmpty)
                {
                    AddError(currentProperty, "The specified folder is not empty!");
                }
            }
        }

        static bool IsValidPath(string path)
        {
            // 使用正则表达式进行匹配
            string pattern = @"^[a-zA-Z():\\ ]+$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(path);
        }

        #endregion Methods
    }
}