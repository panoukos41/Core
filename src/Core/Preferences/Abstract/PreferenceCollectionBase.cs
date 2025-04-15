using Core.Preferences.Builders;
using R3;
using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Core.Preferences.Abstract;

/// <summary>
/// Base implementation of a preference control that includes other preferences.
/// </summary>
public abstract class PreferenceCollectionBase : PreferenceBase, IReadOnlyCollection<PreferenceBase>
{
    private readonly ImmutableArray<PreferenceBase> container;
    private readonly Subject<PreferenceValueBase> notifications = new();

    /// <inheritdoc/>
    public int Count => container.Length;

    /// <summary>
    /// Return all (including children of children) preferences.
    /// </summary>
    public IEnumerable<PreferenceBase> Preferences
        => container.SelectMany(static p => p is PreferenceCollectionBase col ? col.Preferences : [p]);

    /// <summary>
    /// Return all (including children of children) value based preferences.
    /// </summary>
    public IEnumerable<PreferenceValueBase> Values
        => container.SelectMany(static p => p is PreferenceCollectionBase col ? col.Values : p is PreferenceValueBase v ? [v] : []);

    /// <summary>
    /// Get the 'value preference' for the specified key.
    /// </summary>
    /// <param name="key">The key of the 'value preference' to get.</param>
    /// <returns>The 'value preference' for the specified key.</returns>
    /// <exception cref="KeyNotFoundException">When the key doesn't exist.</exception>
    public PreferenceValueBase GetValuePreference(string key)
        => Values.FirstOrDefault(p => p.Key == key) ?? throw new KeyNotFoundException($"Preference with key '{key}' could not be found.");

    /// <summary>
    /// Try and get the 'value preference' for the specified key.
    /// </summary>
    /// <param name="key">The key of the 'value preference' to get.</param>
    /// <param name="value">The 'value preference' if found.</param>
    /// <returns><see langword="true"/> when the value is found otherwise <see langword="false"/>.</returns>
    public bool TryGetValuePreference(string key, [MaybeNullWhen(false)] out PreferenceValueBase? value)
    {
        value = Values.FirstOrDefault(p => p.Key == key);
        return value is not null;
    }

    /// <summary>
    /// Get notified when a specific preference changes.
    /// </summary>
    /// <param name="key">The key of the preference.</param>
    public Observable<PreferenceValueBase> WhenValuePreferenceChanged(string key)
        => notifications.Where(args => args.Key == key);

    /// <summary>
    /// Get notified when any preference changes and receive it to inspect its values etc.
    /// </summary>
    /// <param name="key">The key of the preference.</param>
    public Observable<PreferenceValueBase> WhenAnyPreferenceValueChanged()
        => notifications.AsObservable();

    protected PreferenceCollectionBase(PreferenceCollectionBuilderBase builder) : base(builder)
    {
        container = [
            .. builder.Preferences
                .Select(preference => preference.Build())
                .Do(preference =>
                {
                    var disposable = preference switch
                    {
                        PreferenceValueBase value => value.WhenChanged.Subscribe(notifications.AsObserver()),
                        PreferenceCollectionBase collection => collection.WhenAnyPreferenceValueChanged().Subscribe(notifications.AsObserver()),
                        _ => Disposable.Empty,
                    };
                    if (disposable != Disposable.Empty)
                    {
                        disposable.DisposeWith(this);
                    }
                })
        ];
    }

    /// <inheritdoc/>
    public IEnumerator<PreferenceBase> GetEnumerator()
    {
        return container.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
