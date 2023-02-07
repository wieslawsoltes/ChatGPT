using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChatGPT.UI.ViewModels;

public class ActionsViewModel : ObservableObject
{
    public Func<Task>? New { get; set; }

    public Func<Task>? Open { get; set; }

    public Func<Task>? Save { get; set; }

    public Func<Task>? Export { get; set; }

    public Action? Exit { get; set; }
}
