using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.ViewModels;

public class ActionsViewModel : ObservableObject
{
    [JsonIgnore]
    public Func<Task>? New { get; set; }

    [JsonIgnore]
    public Func<Task>? Open { get; set; }

    [JsonIgnore]
    public Func<Task>? Save { get; set; }

    [JsonIgnore]
    public Func<Task>? Export { get; set; }

    [JsonIgnore]
    public Action? Exit { get; set; }
}
