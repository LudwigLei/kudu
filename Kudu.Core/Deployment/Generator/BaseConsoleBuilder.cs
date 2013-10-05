using System.IO;
using System.Threading.Tasks;
using Kudu.Contracts.Settings;
using Kudu.Core.Infrastructure;

namespace Kudu.Core.Deployment.Generator
{
    public abstract class BaseConsoleBuilder : GeneratorSiteBuilder
    {
        protected BaseConsoleBuilder(IEnvironment environment, IDeploymentSettingsManager settings, IBuildPropertyProvider propertyProvider, string sourcePath, string projectPath)
            : base(environment, settings, propertyProvider, sourcePath)
        {
            ProjectPath = projectPath;
        }

        protected virtual string Command
        {
            get { return DeploymentSettings.GetValue(SettingsKeys.WorkerCommand); }
        }

        protected string ProjectPath { get; private set; }

        public override async Task Build(DeploymentContext context)
        {
            string destinationPath = context.BuildTempPath;
            string consoleWorkerPath = Path.Combine(Environment.ScriptPath, "ConsoleWorker");

            CopyFile(consoleWorkerPath, destinationPath, "global.asax");
            CopyFile(consoleWorkerPath, destinationPath, "web.config");

            string scriptDirectoryPath = Path.Combine(destinationPath, "bin");
            string scriptFilePath = Path.Combine(scriptDirectoryPath, "run_worker.cmd");
            string scriptContent = "@echo off\n{0}\n".FormatInvariant(Command);

            OperationManager.Attempt(() =>
            {
                FileSystemHelpers.EnsureDirectory(scriptDirectoryPath);
                File.WriteAllText(scriptFilePath, scriptContent);
            });

            await base.Build(context);
        }

        private static void CopyFile(string sourcePath, string destinationPath, string fileName)
        {
            sourcePath = Path.Combine(sourcePath, fileName + ".template");
            destinationPath = Path.Combine(destinationPath, fileName);
            OperationManager.Attempt(() => File.Copy(sourcePath, destinationPath, overwrite: true));
        }
    }
}