using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace MultiSelector.ViewModels;

public class NestedSelection : ReactiveObject, IDisposable
{
    private bool canUpdate = true;
    private readonly CompositeDisposable disposables = new();

    public NestedSelection(ISelectableModel model)
    {
        var childrenSelectionStates = model.Children
            .ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(collection => GetSelectionState(collection.Select(selectable => selectable.IsSelected).ToList()))
            .Where(_ => canUpdate);

        childrenSelectionStates.BindTo(model, x => x.IsSelected)
            .DisposeWith(disposables);

        ToggleSelection = ReactiveCommand.Create(() => { UpdateCheckedStateCascade(model, model.IsSelected ?? false); })
            .DisposeWith(disposables);
    }

    private void UpdateCheckedStateCascade(ISelectableModel model, bool isChecked)
    {
        canUpdate = false;

        model.IsSelected = isChecked;

        foreach (var child in model.Children)
        {
            UpdateCheckedStateCascade(child, isChecked);
        }

        canUpdate = true;
    }

    public ReactiveCommand<Unit, Unit> ToggleSelection { get; }

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