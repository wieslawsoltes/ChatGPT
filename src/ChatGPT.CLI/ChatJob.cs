using ChatGPT.ViewModels.Chat;

namespace ChatGPT.CLI;

internal record ChatJob(string InputPath, string OutputPath, ChatSettingsViewModel Settings, bool Quiet);
