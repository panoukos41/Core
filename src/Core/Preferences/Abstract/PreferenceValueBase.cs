using Core.Preferences.Builders;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

namespace Core.Preferences.Abstract;

/// <summary>
/// A delegate to get summaries for values.
/// </summary>
/// <param name="value">The value to get a summary for.</param>
/// <returns>A summary of the current value.</returns>
public delegate string SummaryProvider(string value);

/// <summary>
/// Base preference class.
/// </summary>
public abstract class PreferenceValueBase : PreferenceBase
{
    private string? _value;
    private bool _enabled;
    private bool _visible;

    /// <summary>
    /// A default <see cref="Abstract.SummaryProvider"/> that returns the value itself.
    /// </summary>
    public static readonly SummaryProvider DefaultSummaryProvider = static (v) => v;

    /// <summary>
    /// A value that represents the key that is used to persist and identity the Preference
    /// and it's associated value if Persisted.
    /// You must set a key for each Preference in your hierarchy.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// A boolean value that indicates whether users can interact with the Preference.
    /// When this value is false, the Preference should appear grayed out,
    /// and users cannot interact with it. The default value is true.
    /// </summary>
    public bool Enabled { get => _enabled; set => SetAndRaise(ref _enabled, value); }

    /// <summary>
    /// A boolean value that indicates whether a Preference is visible.
    /// </summary>
    public bool Visible { get => _visible; set => SetAndRaise(ref _visible, value); }

    /// <summary>
    /// Indicates if the preference should be persisted or not. Defaults to true.
    /// </summary>
    public bool Persist { get; }

    /// <summary>
    /// Represents the default value for a Preference.
    /// This value is used when no other persisted value for this Preference is found.
    /// The value type depends on the associated Preference.
    /// </summary>
    public string DefaultValue { get; }

    /// <summary>
    /// Represents the value for a Preference.
    /// This value is persisted.
    /// The value type depends on the associated Preference.
    /// </summary>
    public virtual string Value
    {
        get => _value is null ? DefaultValue : _value;
        set => SetAndRaise(ref _value, value);
    }

    /// <summary>
    /// Get a delegate that given a value will provide the correct summary to display.
    /// </summary>
    public SummaryProvider SummaryProvider { get; set; }

    public IObservable<PreferenceValueBase> WhenChanged => WhenPropertyChanged
        .Where(x => x.PropertyName is nameof(Value))
        .Select(_ => this);

    public IObservable<string> WhenValueChanged => WhenPropertyChanged
        .Where(x => x.PropertyName is nameof(Value))
        .Select(_ => Value);

    [SetsRequiredMembers]
    protected PreferenceValueBase(PreferenceValueBuilderBase builder) : base(builder)
    {
        Key = builder.Key;
        Enabled = !builder.Disabled;
        Visible = !builder.Invisible;
        Persist = builder.Persist;
        DefaultValue = builder.DefaultValue;
        SummaryProvider = builder.SummaryProvider ?? DefaultSummaryProvider;
    }
}

/// <summary>
/// Base preference class with generic value instead of string.
/// </summary>
/// <remarks>
/// The <typeparamref name="TValue"/> must implement <see cref="IParsable{TSelf}"/> and <see cref="object.ToString"/> will be used.
/// If these throw then an exception will be thrown here too.
/// </remarks>
/// <typeparam name="TValue">The type of the value that will be stored.</typeparam>
public abstract class PreferenceValueBase<TValue> : PreferenceValueBase
    where TValue : notnull, IParsable<TValue>, new()
{
    /// <summary>
    /// Represents the default value for a Preference.
    /// This value is used when no other persisted value for this Preference is found.
    /// The value type depends on the associated Preference.
    /// </summary>
    public new TValue DefaultValue
        => TValue.Parse(base.DefaultValue, null);

    /// <summary>
    /// Represents the value for a Preference.
    /// This value is persisted.
    /// The value type depends on the associated Preference.
    /// </summary>
    public new virtual TValue Value
    {
        get => TValue.Parse(base.Value, null);
        set => base.Value = value.ToString() ?? throw new NullReferenceException("Value should not be null");
    }

    [SetsRequiredMembers]
    protected PreferenceValueBase(PreferenceValueBuilderBase builder) : base(builder)
    {
    }
}
