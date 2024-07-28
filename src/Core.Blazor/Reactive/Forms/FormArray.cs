using Core.Blazor.Reactive.Forms.Abstract;
using Core.Blazor.Reactive.Forms.Events;
using Core.Blazor.Reactive.Forms.Primitives;
using System.Collections;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Core.Blazor.Reactive.Forms;

public class FormArray : AbstractControl, IList<AbstractControl>, IReadOnlyCollection<AbstractControl>
{
    private readonly ValueChangeEvent cachedValueChanged;
    private readonly Subject<ControlEvent> aggregatedEventsSubject = new();
    private readonly List<AbstractControlEntry> controls = [];

    public override object? RawValue => this;

    public override bool HasValue { get; } = true;

    public FormArray()
    {
        cachedValueChanged = new ValueChangeEvent(this);
        aggregatedEventsSubject.Subscribe(@event => _ = @event switch
        {
            ValueChangeEvent => SendEvent(cachedValueChanged),
            PristineChangeEvent => SetPristine(controls.All(x => x.Control.Pristine)),
            TouchedChangeEvent => SetTouched(controls.Any(x => x.Control.Touched)),
            StatusChangeEvent => SetStatus(controls.Aggregate((a, c) => a.Control.Status > c.Control.Status ? a : c).Control.Status),
            _ => Unit.Default
        });
    }

    protected override IDisposable? ValidateImpl()
    {
        foreach (var entry in controls)
        {
            entry.Control.Validate();
        }
        return Disposable.Create(() =>
        {
            foreach (var entry in controls)
            {
                entry.Control.CancelValidation();
            }
        });
    }

    #region FormArray

    public int Count => controls.Count;

    public AbstractControl this[int index]
    {
        get => controls[index].Control;
        set => Set(index, value);
    }

    public AbstractControl? Get(int index)
    {
        return controls.ElementAtOrDefault(index) is { } entry ? entry.Control : null;
    }

    public T? Get<T>(int index) where T : AbstractControl
    {
        return controls.ElementAtOrDefault(index) is { Control: T control } ? control : null;
    }

    /// <summary>
    /// Add a new 'AbstractControl' at the end of the array.
    /// </summary>
    /// <param name="control">The control to be inserted.</param>
    public void Add(AbstractControl control)
    {
        controls.Add(new(control, this, control.Events.Subscribe(aggregatedEventsSubject)));
        control.Validate();
    }

    /// <summary>
    /// Insert a new 'AbstractControl' at the given 'index' in the array.
    /// </summary>
    /// <param name="index">Index in the array to insert the control.</param>
    /// <param name="control">The control to be inserted.</param>
    public void Insert(int index, AbstractControl control)
    {
        controls.Insert(index, new(control, this, control.Events.Subscribe(aggregatedEventsSubject)));
        control.Validate();
    }

    /// <summary>
    /// Replace an existing control.
    /// </summary>
    /// <param name="index">The control id to replace in the collection.</param>
    /// <param name="control">Provides the control for the given id.</param>
    public void Set(int index, AbstractControl control)
    {
        // todo: Needs to behave more like angulars.
        RemoveAt(index);
        Insert(index, control);
    }

    /// <summary>
    /// Remove the given control.
    /// </summary>
    /// <param name="control">The control to remove.</param>
    public bool Remove(AbstractControl control)
    {
        var entry = new AbstractControlEntry(control, this, Disposable.Empty);

        if (!controls.Contains(entry)) return false;

        controls.Remove(entry);
        entry.Dispose();
        return true;
    }

    /// <summary>
    /// Remove the control at the given 'index' in the array.
    /// </summary>
    /// <param name="index">Index in the array to remove the control.</param>
    public void RemoveAt(int index)
    {
        var entry = controls[index];
        controls.RemoveAt(index);

        entry.Dispose();
    }

    /// <summary>
    /// Returns <see langword="true"/> of the control exists otherwise <see langword="false"/>.
    /// </summary>
    /// <param name="control">The control to search for.</param>
    /// <returns></returns>
    public bool Contains(AbstractControl control)
    {
        var entry = new AbstractControlEntry(control, this, Disposable.Empty);
        return controls.Contains(entry);
    }

    /// <summary>
    /// Remove all controls from the 'FormArray'.
    /// </summary>
    public void Clear()
    {
        var entries = new AbstractControlEntry[controls.Count];
        controls.CopyTo(entries, 0);
        controls.Clear();

        foreach (var entry in entries)
        {
            entry.Dispose();
        }
    }

    #endregion

    #region IList/IReadonlyCollection

    bool ICollection<AbstractControl>.IsReadOnly { get; }

    public int IndexOf(AbstractControl item)
    {
        var entry = new AbstractControlEntry(item, this, Disposable.Empty);
        return controls.IndexOf(entry);
    }

    public void CopyTo(AbstractControl[] array, int arrayIndex)
    {
        foreach (var entry in controls)
        {
            array[arrayIndex++] = entry.Control;
        }
    }

    public IEnumerator<AbstractControl> GetEnumerator()
    {
        return GetEnumerable().GetEnumerator();

        IEnumerable<AbstractControl> GetEnumerable()
        {
            foreach (var entry in controls)
            {
                yield return entry.Control;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    /// <inheritdoc/>
    public override void Dispose()
    {
        // Clear();
        aggregatedEventsSubject?.OnCompleted();
        aggregatedEventsSubject?.Dispose();
        base.Dispose();
    }
}
