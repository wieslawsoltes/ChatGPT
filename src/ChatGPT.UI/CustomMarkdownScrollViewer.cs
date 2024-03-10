using Avalonia.Controls;
using Avalonia.Input;
using Markdown.Avalonia;

namespace AI
{

    public class CustomMarkdownScrollViewer : MarkdownScrollViewer
    {
        public CustomMarkdownScrollViewer()
        {
            Plugins = new MdAvPlugins();
            AddHandler(RequestBringIntoViewEvent, OnRequestBringIntoView, handledEventsToo: true);
        }

        private void OnRequestBringIntoView(object? sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }


}
