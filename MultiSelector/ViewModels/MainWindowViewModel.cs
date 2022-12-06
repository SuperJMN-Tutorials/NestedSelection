using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;

namespace MultiSelector.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        var viewModels = new[]
        {
            new ViewModel(new ObservableCollection<ViewModel>(Enumerable.Empty<ViewModel>())),
            new ViewModel(new ObservableCollection<ViewModel>(Enumerable.Empty<ViewModel>())),
            new ViewModel(new ObservableCollection<ViewModel>(new[]
            {
                new ViewModel(new ObservableCollection<ViewModel>(Enumerable.Empty<ViewModel>())),
                new ViewModel(new ObservableCollection<ViewModel>(Enumerable.Empty<ViewModel>())),
                new ViewModel(new ObservableCollection<ViewModel>(Enumerable.Empty<ViewModel>()))
            }))
        };

        var observableCollection = new ObservableCollection<ViewModel>(new ObservableCollection<ViewModel>(viewModels));

        var viewModel = new ViewModel(observableCollection);

        Observable.Timer(TimeSpan.FromSeconds(4), RxApp.MainThreadScheduler).Select(_ => new ViewModel(new ObservableCollection<ViewModel>(Enumerable.Empty<ViewModel>())))
            .Do(x => observableCollection.Add(x))
            .Subscribe();

        Tree = new[]
        {
            viewModel
        };
    }

    public ViewModel[] Tree { get; }
}