using Core.Blazor.Reactive.Forms.Abstract;
using Core.Blazor.Reactive.Forms.Events;
using Core.Blazor.Reactive.Forms.Primitives;
using R3;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Core.Blazor.Reactive.Forms;

public sealed class FormGroup : AbstractControl, IDictionary<string, AbstractControl>, IReadOnlyDictionary<string, AbstractControl>
{
    private readonly ValueChangeEvent cachedValueChanged;
    private readonly Subject<ControlEvent> aggregatedEventsSubject = new();
    private readonly Dictionary<string, AbstractControlEntry> controls = [];

    public override object? RawValue => this;

    public override bool HasValue { get; } = true;

    public FormGroup()
    {
        cachedValueChanged = new ValueChangeEvent(this);
        aggregatedEventsSubject.Subscribe(@event => _ = @event switch
        {
            ValueChangeEvent => SendEvent(cachedValueChanged),
            PristineChangeEvent => SetPristine(controls.Values.All(x => x.Control.Pristine)),
            TouchedChangeEvent => SetTouched(controls.Values.Any(x => x.Control.Touched)),
            StatusChangeEvent => SetStatus(controls.Values.Aggregate((a, c) => a.Control.Status > c.Control.Status ? a : c).Control.Status),
            _ => Unit.Default
        });
    }

    protected override IDisposable? ValidateImpl()
    {
        foreach (var (_, entry) in controls)
        {
            entry.Control.Validate();
        }
        return Disposable.Create(() =>
        {
            foreach (var (_, entry) in controls)
            {
                entry.Control.CancelValidation();
            }
        });
    }

    #region FormGroup

    public int Count => controls.Count;

    public AbstractControl this[string key]
    {
        get => controls[key].Control;
        set => Set(key, value);
    }

    public AbstractControl? Get(string id)
    {
        return controls.TryGetValue(id, out var v) ? v.Control : null;
    }

    public bool TryGet(string id, [MaybeNullWhen(false)] out AbstractControl? control)
    {
        control = Get(id);
        return control is { };
    }

    public T? Get<T>(string id) where T : AbstractControl
    {
        return controls.TryGetValue(id, out var v) ? (T)v.Control : null;
    }

    public bool TryGet<T>(string id, [MaybeNullWhen(false)] out T? control) where T : AbstractControl
    {
        control = Get<T>(id);
        return control is { };
    }

    /// <summary>
    /// Registers a control with the group's list of controls. In a strongly-typed group, the control
    /// must be in the group's type (possibly as an optional key).
    /// </summary>
    /// <param name="id">The control id to register in the collection.</param>
    /// <param name="control">Provides the control for the given id.</param>
    public void Register(string id, AbstractControl control)
    {
        controls.Add(id, new(control, this, control.Events.Subscribe(aggregatedEventsSubject.AsObserver())));
    }

    /// <summary>
    /// Add a control to this group. In a strongly-typed group, the control must
    /// be in the group's type (possibly as an optional key).
    /// </summary>
    /// <param name="id">The control id to add to the collection.</param>
    /// <param name="control">The control for the given id.</param>
    /// <remarks>This method will also call validate.</remarks>
    public void Add(string id, AbstractControl control)
    {
        Register(id, control);
        control.Validate();
    }

    /// <summary>
    /// Replace an existing control. In a strongly-typed group, the control must be in the group's type (possibly as an optional key).
    /// </summary>
    /// <remarks>
    /// If a control with a given id does not exist in this 'FormGroup', it will be added.
    /// </remarks>
    /// <param name="id">The control id to replace in the collection.</param>
    /// <param name="control">Provides the control for the given id.</param>
    public void Set(string id, AbstractControl control)
    {
        Remove(id);
        Add(id, control);
    }

    /// <summary>
    /// Remove a control from this group. In a strongly-typed group, required controls cannot be removed.
    /// </summary>
    /// <param name="id">The control id to remove from the collection.</param>
    public bool Remove(string id)
    {
        if (controls.TryGetValue(id, out var entry))
        {
            controls.Remove(id);
            entry.Dispose();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Clear all controls from the group.
    /// </summary>
    public void Clear()
    {
        var entries = new AbstractControlEntry[controls.Count];
        controls.Values.CopyTo(entries, 0);
        controls.Clear();

        foreach (var entry in entries)
        {
            entry.Dispose();
        }
    }

    /// <summary>
    /// Check whether there is a control with the given key in the group.
    /// </summary>
    /// <param name="key">The control key to check for existence in the collection.</param>
    /// <returns><see langword="false"/> if not in the group, <see langword="true"/> otherwise.</returns>
    public bool Contains(string key)
    {
        return controls.ContainsKey(key);
    }

    /// <summary>
    /// Check whether there is an enabled control with the given name in the group.
    /// </summary>
    /// <param name="id">The control id to check for existence in the collection.</param>
    /// <returns><see langword="false"/> if not in the group, <see langword="true"/> otherwise.</returns>
    public bool ContainsEnabled(string id)
    {
        return Get(id) is { Enabled: true };
    }

    #endregion

    #region IDictionary/IReadonlyDictionary

    public bool IsReadOnly { get; }

    public IEnumerable<string> Keys => controls.Keys.AsEnumerable();

    public IEnumerable<AbstractControl> Values => controls.Values.Select(x => x.Control);

    ICollection<string> IDictionary<string, AbstractControl>.Keys => controls.Keys;

    ICollection<AbstractControl> IDictionary<string, AbstractControl>.Values => [.. Values];

    bool IReadOnlyDictionary<string, AbstractControl>.TryGetValue(string key, [MaybeNullWhen(false)] out AbstractControl value)
    {
        value = Get(key);
        return value is { };
    }

    bool IReadOnlyDictionary<string, AbstractControl>.ContainsKey(string key)
    {
        return Contains(key);
    }

    bool IDictionary<string, AbstractControl>.TryGetValue(string key, [MaybeNullWhen(false)] out AbstractControl value)
    {
        value = Get(key);
        return value is { };
    }

    bool IDictionary<string, AbstractControl>.ContainsKey(string key)
    {
        return Contains(key);
    }

    bool ICollection<KeyValuePair<string, AbstractControl>>.Contains(KeyValuePair<string, AbstractControl> item)
    {
        return Get(item.Key) is { } entry && entry == item.Value;
    }

    void ICollection<KeyValuePair<string, AbstractControl>>.Add(KeyValuePair<string, AbstractControl> item)
    {
        Set(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<string, AbstractControl>>.Remove(KeyValuePair<string, AbstractControl> item)
    {
        return Remove(item.Key);
    }

    void ICollection<KeyValuePair<string, AbstractControl>>.CopyTo(KeyValuePair<string, AbstractControl>[] array, int arrayIndex)
    {
        foreach (var entry in (ICollection<KeyValuePair<string, AbstractControlEntry>>)controls)
        {
            array[arrayIndex++] = KeyValuePair.Create(entry.Key, entry.Value.Control);
        }
    }

    public IEnumerator<KeyValuePair<string, AbstractControl>> GetEnumerator()
    {
        return controls.Select(kv => KeyValuePair.Create(kv.Key, kv.Value.Control)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    /// <inheritdoc/>
    public override void Dispose()
    {
        aggregatedEventsSubject?.OnCompleted();
        aggregatedEventsSubject?.Dispose();
        base.Dispose();
    }
}
