﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elevator.Agent.Models;
using Git;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shell;

namespace Elevator.Agent.Services
{
    public class ExecuteService: IHostedService
    {
        private const string WorkingDirectoryPath = "C:\\agent\\work";
        private const string ProjectDirectoryPath = "C:\\agent\\work\\project";


        private readonly StatusService statusService;
        private readonly ILoggerFactory loggerFactory;
        private readonly TaskService taskService;

        private Timer timer;

        private bool inProgress;

        public ExecuteService(StatusService statusService, ILoggerFactory loggerFactory, TaskService taskService)
        {
            this.statusService = statusService;
            this.loggerFactory = loggerFactory;
            this.taskService = taskService;
        }

        private async Task ProcessTask()
        {
            if (inProgress || taskService.Task == null || statusService.Status == Status.Finished)
                return;

            inProgress = true;
            try
            {
                PrepareWorkingDirectory();
                await CloneProjectAsync();
                foreach (var buildCommand in taskService.Task.Commands)
                {
                    await ExecuteCommandAsync(buildCommand);
                }

                taskService.BuildTaskResult.Status = Models.TaskStatus.Success;
            }
            catch
            {
                taskService.BuildTaskResult.Status = Models.TaskStatus.Failed;
            }
            finally
            {
                statusService.Status = Status.Finished;
                inProgress = false;
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
            var gitProjectInformation = new GitProjectInformation(new Uri(taskService.Task.ProjectUrl), taskService.Task.GitToken,
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
            taskService.BuildTaskResult.Logs = taskService.BuildTaskResult.Logs.AddRange(logs);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(_ => ProcessTask(), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            return Task.CompletedTask;
        }
    }
}