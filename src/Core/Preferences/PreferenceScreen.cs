using Core.Preferences.Abstract;
using Core.Preferences.Builders;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

namespace Core.Preferences;

public delegate object? IconTransformer(string? icon);

public delegate string TitleTransformer(string value);

public delegate string? DescriptionTransformer(string? value);

public delegate string SummaryTransformer(string value);

public sealed class PreferenceScreen : PreferenceCollectionBase
{
    private IDisposable? subscription;

    /// <summary>
    /// The configuration used to read and write values to.
    /// </summary>
    public IConfiguration? Configuration { get; private set; }

    public IconTransformer? IconTransformer { get; set; }

    public TitleTransformer? TitleTransformer { get; set; }

    public DescriptionTransformer? DescriptionTransformer { get; set; }

    public SummaryTransformer? SummaryTransformer { get; set; }

    /// <summary>
    /// Initialize a new instance of <see cref="PreferenceScreen"/>.
    /// </summary>
    internal PreferenceScreen(PreferenceScreenBuilder builder) : base(builder)
    {
    }

    /// <summary>
    /// Try and get the 'page preference' for the specified path.
    /// </summary>
    /// <param name="path">The path of the 'page preference' to get.</param>
    /// <returns>The 'page preference' for the specified path.</returns>
    /// <exception cref="KeyNotFoundException">When the path doesn't exist.</exception>
    public bool TryGetPagePreference(string path, [NotNullWhen(true)] PagePreference? page)
    {
        page = Preferences.Where(p => p is PagePreference page && page.Path == path).FirstOrDefault()?.As<PagePreference>();
        return page is not null;
    }

    /// <summary>
    /// Initialize all preference values. Optionally passing the configuration to be used.
    /// </summary>
    /// <remarks>If no configuration has been provided ever nothing will happen.</remarks>
    public PreferenceScreen Initialize(IConfiguration? configuration = null)
    {
        Configuration = configuration ??= Configuration;

        if (Configuration is null)
            return this;

        foreach (var preference in Values.Where(p => p.Persist && string.IsNullOrWhiteSpace(p.Key) is false))
        {
            if (Configuration[preference.Key] is { } item)
            {
                preference.Value = item;
            }
        }

        subscription?.Dispose();
        subscription = WhenAnyPreferenceValueChanged()
            .Where(p => p.Persist && string.IsNullOrWhiteSpace(p.Key) is false)
            .Subscribe(p => Configuration[p.Key] = p.Value);

        return this;
    }

    /// <summary>
    /// Opposite of <see cref="Initialize(IConfiguration?)"/>.
    /// Will remove Configuration and disable component notifications.
    /// </summary>
    public void Teardown()
    {
        Configuration = null;
        subscription?.Dispose();
        subscription = null;
    }
}
