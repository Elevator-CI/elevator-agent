using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elevator.Agent.Models;
using Git;
using Microsoft.Extensions.Logging;
using Shell;

namespace Elevator.Agent.Services
{
    public class TaskService
    {
        
        public BuildTaskResult BuildTaskResult { get; private set; }

        private const string WorkingDirectoryPath = "C:\\agent\\work";
        private const string ProjectDirectoryPath = "C:\\agent\\work\\project";

        private BuildTask task;
        private readonly StatusService statusService;
        private readonly ILoggerFactory loggerFactory;

        public TaskService(StatusService statusService, ILoggerFactory loggerFactory)
        {
            this.statusService = statusService;
            this.loggerFactory = loggerFactory;
        }

        public async Task StartTask(BuildTask task)
        {
            if (statusService.Status != Status.Free)
                throw new InvalidOperationException("Agent is not free");

            statusService.Status = Status.Working;

            this.task = task;
            BuildTaskResult = new BuildTaskResult
            {
                Logs = ImmutableList<string>.Empty
            };

            await ProcessTask();
        }

        private async Task ProcessTask()
        {
            try
            {
                PrepareWorkingDirectory();
                await CloneProjectAsync();
                foreach (var buildCommand in task.Commands)
                {
                    await ExecuteCommandAsync(buildCommand);
                }
            }
            finally
            {
                statusService.Status = Status.Finished;
            }
            
        }

        private async Task ExecuteCommandAsync(BuildCommand buildCommand)
        {
            var shellRunner = new ShellRunner();
            AddLog($"Executing command '{buildCommand.Command} {buildCommand.Arguments}'");
            var args = ConvertBuildCommandToShellRunnerArgs(buildCommand);
            var executionResult = await shellRunner.RunAsync(args);
            if (!executionResult.IsSuccessful)
            {
                AddLog($"Cannot execute command", executionResult.Error);
                throw new Exception();
            }
            AddLog(await executionResult.Value.Output.ReadToEndAsync());
        }

        private ShellRunnerArgs ConvertBuildCommandToShellRunnerArgs(BuildCommand buildCommand)
        {
            return new(ProjectDirectoryPath, "cmd", false, $"/c \"{buildCommand.Command} {buildCommand.Arguments}\"");
        }

        private async Task CloneProjectAsync()
        {
            var gitProjectInformation = new GitProjectInformation(new Uri(task.ProjectUrl), task.GitToken,
                WorkingDirectoryPath, ProjectDirectoryPath, true);
            var gitProject = new GitProject(gitProjectInformation, loggerFactory, new ShellRunner());
            var gitRepository = await gitProject.CloneAsync();
            if (!gitRepository.IsSuccessful)
            {
                AddLog("Can not clone project", gitRepository.Error);
                throw new Exception();
            }
        }

        private void PrepareWorkingDirectory()
        {
            if (Directory.Exists(WorkingDirectoryPath))
                Directory.Delete(WorkingDirectoryPath, true);
            Directory.CreateDirectory(WorkingDirectoryPath);
        }

        private void AddLog(params string[] logs)
        {
            BuildTaskResult.Logs = BuildTaskResult.Logs.AddRange(logs);
        }
    }
}
