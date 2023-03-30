using System.Diagnostics;
using System.Text.Json;
using ChatGPT.ViewModels;
using ChatGPT.ViewModels.Chat;

namespace ChatGPT.CLI;

internal static class Chat
{
    private static readonly Action<object>? s_log = Console.WriteLine;

    private static async Task RunJob(ChatJob job, CancellationToken token)
    {
        try
        {
            if (!job.Quiet)
            {
                s_log?.Invoke($"Executing: {job.OutputPath}");
            }

            var input = await File.ReadAllTextAsync(job.InputPath, token);
            var chat = new ChatViewModel(job.Settings);
            chat.AddSystemMessage(job.Settings.Directions);
            chat.AddUserMessage(input);
            var result = await chat.SendAsync(chat.CreateChatMessages(), token);
            await File.WriteAllTextAsync(job.OutputPath, result?.Message, token);
        }
        catch (Exception ex)
        {
            if (!job.Quiet)
            {
                s_log?.Invoke($"Error: {job.InputPath}");
                s_log?.Invoke(ex);
            }
        }
    }

    public static async Task Run(CliSettings cliSettings)
    {
        var paths = GetPaths(cliSettings);
        if (paths == null || paths.Count == 0)
        {
            return;
        }

        if (cliSettings.OutputFiles is { })
        {
            if (paths.Count > 0 && paths.Count != cliSettings.OutputFiles.Length)
            {
                if (!cliSettings.Quiet)
                {
                    s_log?.Invoke("Error: The number of the output files must match the number of the input files.");
                }

                return;
            }
        }

        if (cliSettings.OutputDirectory is { } && !string.IsNullOrEmpty(cliSettings.OutputDirectory.FullName))
        {
            if (!Directory.Exists(cliSettings.OutputDirectory.FullName))
            {
                Directory.CreateDirectory(cliSettings.OutputDirectory.FullName);
            }
        }

        var sw = Stopwatch.StartNew();

        var jobs = GetJobs(cliSettings, paths);

        var cts = new CancellationTokenSource();

        await Parallel.ForEachAsync(
            jobs, 
            new ParallelOptions
            {
                MaxDegreeOfParallelism = cliSettings.Threads, 
                CancellationToken = cts.Token
            }, 
            async (job, token) => await RunJob(job, token));

        sw.Stop();

        if (!cliSettings.Quiet)
        {
            s_log?.Invoke($"Done: {sw.Elapsed}");
        }
    }

    private static List<FileInfo>? GetPaths(CliSettings cliSettings)
    {
        var paths = new List<FileInfo>();

        if (cliSettings.InputFiles is { })
        {
            paths.AddRange(cliSettings.InputFiles);
        }

        if (cliSettings.InputDirectory is { })
        {
            var directory = cliSettings.InputDirectory;
            var patterns = cliSettings.Pattern;
            if (patterns.Length == 0)
            {
                if (!cliSettings.Quiet)
                {
                    s_log?.Invoke("Error: The pattern can not be empty.");
                }
                return null;
            }

            foreach (var pattern in patterns)
            {
                GetFiles(directory, pattern, paths, cliSettings.Recursive);
            }
        }

        return paths;
    }

    private static List<ChatJob> GetJobs(CliSettings cliSettings, List<FileInfo> paths)
    {
        var jobs = new List<ChatJob>();

        var fileSettings = default(ChatSettingsViewModel);

        if (cliSettings.SettingsFile is { })
        {
            using var stream = File.OpenRead(cliSettings.SettingsFile.FullName);
            fileSettings = JsonSerializer.Deserialize(
                stream,
                MainViewModelJsonContext.s_instance.ChatSettingsViewModel);
        }

        var chatSettings = fileSettings ?? new ChatSettingsViewModel
        {
            Temperature = cliSettings.Temperature,
            TopP = cliSettings.TopP,
            PresencePenalty = cliSettings.PresencePenalty,
            FrequencyPenalty = cliSettings.FrequencyPenalty,
            MaxTokens = cliSettings.MaxTokens,
            ApiKey = cliSettings.ApiKey,
            Model = cliSettings.Model,
            Directions = cliSettings.Directions,
        };

        for (var i = 0; i < paths.Count; i++)
        {
            var inputPath = paths[i];
            var outputFile = cliSettings.OutputFiles?[i];
            string outputPath;

            if (outputFile is { })
            {
                outputPath = outputFile.FullName;
            }
            else
            {
                outputPath =  Path.ChangeExtension(inputPath.FullName, cliSettings.Extension.ToLower());
                if (cliSettings.OutputDirectory is { } && !string.IsNullOrEmpty(cliSettings.OutputDirectory.FullName))
                {
                    outputPath = Path.Combine(cliSettings.OutputDirectory.FullName, Path.GetFileName(outputPath));
                }
            }

            jobs.Add(new ChatJob(inputPath.FullName, outputPath, chatSettings, cliSettings.Quiet));
        }

        return jobs;
    }

    private static void GetFiles(DirectoryInfo directory, string pattern, List<FileInfo> paths, bool recursive)
    {
        var files = Directory.EnumerateFiles(directory.FullName, pattern);

        paths.AddRange(files.Select(path => new FileInfo(path)));

        if (!recursive)
        {
            return;
        }

        foreach (var subDirectory in directory.EnumerateDirectories())
        {
            GetFiles(subDirectory, pattern, paths, recursive);
        }
    }
}
