using System;
using System.Text;
using Kudu.Contracts.Settings;
using Kudu.Core.Infrastructure;

namespace Kudu.Core.Deployment.Generator
{
    public class DotNetConsoleBuilder : BaseConsoleBuilder
    {
        private readonly string _solutionPath;

        public DotNetConsoleBuilder(IEnvironment environment, IDeploymentSettingsManager settings, IBuildPropertyProvider propertyProvider, string sourcePath, string projectPath, string solutionPath)
            : base(environment, settings, propertyProvider, sourcePath, projectPath)
        {
            _solutionPath = solutionPath;
        }

        protected override string Command
        {
            get { return base.Command ?? VsHelper.GetProjectExecutableName(ProjectPath); }
        }

        protected override string ScriptGeneratorCommandArguments
        {
            get
            {
                StringBuilder commandArguments = new StringBuilder();
                commandArguments.AppendFormat("--dotNetConsole \"{0}\"", ProjectPath);

                if (!String.IsNullOrEmpty(_solutionPath))
                {
                    commandArguments.AppendFormat(" --solutionFile \"{0}\"", _solutionPath);
                }
                else
                {
                    commandArguments.AppendFormat(" --no-solution", _solutionPath);
                }

                return commandArguments.ToString();
            }
        }

        public override string ProjectType
        {
            get { return ".NET CONSOLE WORKER"; }
        }
    }
}
