using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace MultiSelector.ViewModels;

public class NestedSelection : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable disposables = new();

    public NestedSelection(ISelectableModel model)
    {
        Model = model;

        model.Children
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(collection => GetSelectionState(collection.Select(selectable => selectable.IsSelected).ToList()))
            .BindTo(model, x => x.IsSelected)
            .DisposeWith(disposables);

        this.WhenAnyValue(x => x.Model.IsSelected)
            .WhereNotNull()
            .Do(isSelected =>
            {
                Toggle(isSelected!.Value);
            })
            .Subscribe();
    }

    private void Toggle(bool isSelected)
    {
        foreach (var item in Model.Children)
        {
            item.IsSelected = isSelected;
        }
    }

    private ISelectableModel Model { get; }

    private static bool? GetSelectionState(IList<bool?> readOnlyCollection)
    {
        bool? selectionState = readOnlyCollection.All(x => x.HasValue && x.Value)
            ? true
            : readOnlyCollection.Any(x => x.HasValue && x.Value || !x.HasValue)
                ? null
                : false;
        return selectionState;
    }

    public void Dispose()
    {
        disposables.Dispose();
    }
}