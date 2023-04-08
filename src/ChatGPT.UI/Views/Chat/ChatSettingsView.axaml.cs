using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace ChatGPT.Views.Chat;

public partial class ChatSettingsView : UserControl
{
    public ChatSettingsView()
    {
        InitializeComponent();

        this.GetObservable(BoundsProperty)
            .Subscribe(new AnonymousObserver<Rect>(_ =>
            {
                if (SettingsSelectingItemsControl.Items is ItemCollection itemCollection)
                {
                    var focused = itemCollection.FirstOrDefault(x => x is Control {IsKeyboardFocusWithin: true});

                    if (focused is Control control)
                    {
                        control.BringIntoView();
                    }
                }
            }));
    }
}
