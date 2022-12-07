using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MultiSelector.ViewModels;

public interface IHierarchicallySelectable : INotifyPropertyChanged
{
    ObservableCollection<ViewModel> Children { get; }
    public bool? IsSelected { get; set; }
}