using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;

namespace MultiSelector.ViewModels;

public class ViewModel : ViewModelBase, ISelectableModel
{
    public ViewModel(ObservableCollection<ViewModel> viewModels)
    {
        Children = viewModels;
        Selector = new NestedSelection(this);
    }

    public NestedSelection Selector { get; }

    public ObservableCollection<ViewModel> Children { get; }

    [Reactive] public bool? IsSelected { get; set; } = false;
}