using System.Collections.ObjectModel;
using ReactiveUI.Fody.Helpers;

namespace MultiSelector.ViewModels;

public class ViewModel : ViewModelBase, IHierarchicallySelectable
{
    public ViewModel(ObservableCollection<ViewModel> viewModels)
    {
        Children = viewModels;
        SelectionHandler = new HierarchicalSelectionHandler(this);
    }

    public HierarchicalSelectionHandler SelectionHandler { get; }

    public ObservableCollection<ViewModel> Children { get; }

    [Reactive] public bool? IsSelected { get; set; } = false;
}