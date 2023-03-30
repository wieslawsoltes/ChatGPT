using System.CommandLine;
using System.CommandLine.Invocation;
using ChatGPT;
using ChatGPT.CLI;

Defaults.ConfigureDefaultServices();

await CreateRootCommand().InvokeAsync(args);

RootCommand CreateRootCommand()
{
    var optionInputFiles = new Option(
        new[] { "--inputFiles", "-f" },
        "The relative or absolute path to the input files")
    {
        Argument = new Argument<FileInfo[]?>(getDefaultValue: () => null)
    };

    var optionInputDirectory = new Option(
        new[] { "--inputDirectory", "-d" },
        "The relative or absolute path to the input directory")
    {
        Argument = new Argument<DirectoryInfo?>(getDefaultValue: () => null)
    };

    var optionOutputDirectory = new Option(
        new[] { "--outputDirectory", "-o" },
        "The relative or absolute path to the output directory")
    {
        Argument = new Argument<DirectoryInfo?>(getDefaultValue: () => null)
    };

    var optionOutputFiles = new Option(
        new[] { "--outputFiles" },
        "The relative or absolute path to the output files")
    {
        Argument = new Argument<FileInfo[]?>(getDefaultValue: () => null)
    };

    var optionPattern = new Option(
        new[] { "--pattern", "-p" },
        "The search string to match against the names of files in the input directory")
    {
        Argument = new Argument<string[]?>(getDefaultValue: () => new[] { "*.*" })
    };

    var optionRecursive = new Option(
        new[] { "--recursive", "-r" },
        "Recurse into subdirectories of input directory search")
    {
        Argument = new Argument<bool>(getDefaultValue: () => true)
    };

    var optionFormat = new Option(
        new[] { "--extension", "-e" },
        "The output file extension")
    {
        Argument = new Argument<string>(getDefaultValue: () => "txt")
    };

    var settingsFileDirectory = new Option(
        new[] { "--settingsFile", "-s" },
        "The relative or absolute path to the settings file")
    {
        Argument = new Argument<FileInfo?>(getDefaultValue: () => null)
    };

    var optionTemperature = new Option(
        new[] { "--temperature" },
        "What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.")
    {
        Argument = new Argument<decimal>(getDefaultValue: () => 0.7m)
    };
    
    var optionTopP = new Option(
        new[] { "--topP" },
        "An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.")
    {
        Argument = new Argument<decimal>(getDefaultValue: () => 0.7m)
    };
    
    var optionPresencePenalty = new Option(
        new[] { "--presencePenalty" },
        "Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.")
    {
        Argument = new Argument<decimal>(getDefaultValue: () => 0.7m)
    };
    
    var optionFrequencyPenalty = new Option(
        new[] { "--frequencyPenalty" },
        "Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.")
    {
        Argument = new Argument<decimal>(getDefaultValue: () => 0.7m)
    };
    
    var optionMaxTokens = new Option(
        new[] { "--maxTokens" },
        "The maximum number of tokens to generate in the chat completion.")
    {
        Argument = new Argument<int>(getDefaultValue: () => 2000)
    };
    
    var optionApiKey = new Option(
        new[] { "--apiKey" },
        "Override OpenAI api key. By default OPENAI_API_KEY environment variable is used.")
    {
        Argument = new Argument<string?>(getDefaultValue: () => null)
    };
    
    var optionModel = new Option(
        new[] { "--model" },
        "ID of the model to use. See the model endpoint compatibility table for details on which models work with the Chat API.")
    {
        Argument = new Argument<string?>(getDefaultValue: () => "gpt-3.5-turbo")
    };
    
    var optionDirections = new Option(
        new[] { "--directions" },
        "The system message (directions) helps set the behavior of the assistant. Typically, a conversation is formatted with a system message first, followed by alternating user and assistant messages.")
    {
        Argument = new Argument<string?>(getDefaultValue: () => "You are a helpful assistant.")
    };

    var optionThreads = new Option(
        new[] { "--threads", "-t" },
        "The number of parallel job threads")
    {
        Argument = new Argument<int>(getDefaultValue: () => 1)
    };

    var optionQuiet = new Option(new[] { "--quiet" }, "Set verbosity level to quiet")
    {
        Argument = new Argument<bool>()
    };

    var rootCommand = new RootCommand { Description = "An .NET ChatGPT tool." };

    rootCommand.AddOption(optionInputFiles);
    rootCommand.AddOption(optionInputDirectory);
    rootCommand.AddOption(optionOutputDirectory);
    rootCommand.AddOption(optionOutputFiles);
    rootCommand.AddOption(optionPattern);
    rootCommand.AddOption(optionRecursive);
    rootCommand.AddOption(optionFormat);

    rootCommand.AddOption(settingsFileDirectory);
    rootCommand.AddOption(optionTemperature);
    rootCommand.AddOption(optionTopP);
    rootCommand.AddOption(optionPresencePenalty);
    rootCommand.AddOption(optionFrequencyPenalty);
    rootCommand.AddOption(optionMaxTokens);
    rootCommand.AddOption(optionApiKey);
    rootCommand.AddOption(optionModel);
    rootCommand.AddOption(optionDirections);

    rootCommand.AddOption(optionThreads);
    rootCommand.AddOption(optionQuiet);

    static async Task Execute(CliSettings settings) => await Chat.Run(settings);

    rootCommand.Handler = CommandHandler.Create(Execute);

    return rootCommand;
}
