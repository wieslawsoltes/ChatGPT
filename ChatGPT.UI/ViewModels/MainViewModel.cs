using System.Collections.ObjectModel;

namespace ChatGPT.UI.ViewModels;

public class MainViewModel : BaseViewModel
{
    private ObservableCollection<string> _choices = new ObservableCollection<string>();
    private string _currentChoice;

    public ObservableCollection<string> Choices
    {
        get => _choices;
        set
        {
            _choices = value;
            OnPropertyChanged();
        }
    }

    public string CurrentChoice
    {
        get => _currentChoice;
        set
        {
            _currentChoice = value;
            OnPropertyChanged();
        }
    }
}
