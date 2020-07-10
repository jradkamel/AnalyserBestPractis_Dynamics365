//------------------------------------------------------------------------------
// <copyright file="AnalyserBP.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using EnvDTE80;


namespace AnalyserBestPractis_Dynamics365
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AnalyserBP
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b69e5d6a-a264-4c81-8f0e-d37c82560c1e");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyserBP"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AnalyserBP(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
                errorListProvider = AnalyserRules.createErrorList(this.package);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AnalyserBP Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new AnalyserBP(package);
           
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        public void MenuItemCallback(object sender, EventArgs e)
        {
            IVsStatusbar statusBar = (IVsStatusbar)ServiceProvider.GetService(typeof(SVsStatusbar));
            uint cookie = 0;
            string label = "SILVERPROD Best practis analysing";

            object icon = (short)Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_Build;

            // Display the icon in the Animation region.
            statusBar.Animation(1, ref icon);
            statusBar.SetText(label);
            statusBar.Progress(ref cookie, 1, label, 5, 10);
           
            DTE dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            Project activeProject = null;
            Array activeSolutionProjects = GetActiveProject(dte);
            Boolean error = false;
            String Spd = Properties.ModelList.Default["Trigramme"].ToString();
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;

               
                errorListProvider.Tasks.Clear();
                SPD spd = new SPD();
                spd.ShowDialog();
                error = AnalyserRules.TryByProjectItem(activeProject.ProjectItems, errorListProvider, Spd, dte, activeProject.FullName);
                errorListProvider.Show();
                TeamServeceConnect.getListWorkitem(TeamServeceConnect.getURI(dte.Solution.FullName));
                if (!error)
                {
                    if (TeamServeceConnect.getURI(dte.Solution.FullName) != "")
                    {
                        string _ret = "The solution is not configured with DevOps/VSTS";
                        AnalyserRules.addMessageErrorList(dte, errorListProvider, _ret, dte.Solution.FullName, "", 0, 0);
                    }
                    if (TeamServeceConnect.getListTeamProjects(dte.Solution.FullName).Contains(activeProject.FullName.Split('\\')[activeProject.FullName.Split('\\').Length - 1].Replace(".rnrproj", "")))
                        System.Windows.Forms.MessageBox.Show("true");
                    System.Windows.Forms.MessageBox.Show(TeamServeceConnect.getListTeamProjects(dte.Solution.FullName)[TeamServeceConnect.getListTeamProjects(dte.Solution.FullName).IndexOf(activeProject.FullName.Split('\\')[activeProject.FullName.Split('\\').Length - 1].Replace(".rnrproj", ""))].ToString());
                }

            }
            label = "SILVERPROD Best practis analysed";
            statusBar.Progress(ref cookie, 0, "", 0, 0);
            statusBar.SetText(label);
        }
        public Array GetActiveProject(DTE _dte)
        {
            return _dte.ActiveSolutionProjects as Array;
        }
        ErrorListProvider errorListProvider;




    }
}
