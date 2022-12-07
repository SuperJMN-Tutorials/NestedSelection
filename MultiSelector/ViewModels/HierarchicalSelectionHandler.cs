using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace MultiSelector.ViewModels;

public class HierarchicalSelectionHandler : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();

    private bool canUpdate = true;

    public HierarchicalSelectionHandler(IHierarchicallySelectable model)
    {
        Model = model;

        model.Children
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .Where(_ => canUpdate)
            .ToCollection()
            .Select(GetSelectionState)
            .BindTo(model, x => x.IsSelected)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.Model.IsSelected)
            .WhereNotNull()
            .Do(isSelected => Toggle(isSelected!.Value))
            .Subscribe();
    }

    private void Toggle(bool isSelected)
    {
        canUpdate = false;

        foreach (var item in Model.Children)
        {
            item.IsSelected = isSelected;
        }

        canUpdate = true;
    }

    private IHierarchicallySelectable Model { get; }

    private static bool? GetSelectionState(IReadOnlyCollection<IHierarchicallySelectable> children)
    {
        var selectionCount = children.Count(x => x.IsSelected == true);
        
        bool? selectionState = selectionCount == 0
            ? false
            : children.Count == selectionCount
                ? true
                : null;
        return selectionState;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}