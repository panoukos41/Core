using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;

namespace Core.Preferences;

/// <summary>
/// Represents a mutable preferences object.
/// </summary>
/// <remarks>
/// It is both an <see cref="IConfigurationBuilder"/> and an <see cref="IConfigurationRoot"/>.
/// As sources are added, it updates its current view of preferences.
/// </remarks>
[DebuggerDisplay("{DebuggerToString(),nq}")]
public sealed class PreferenceManager : IConfigurationManager, IConfigurationRoot, IDisposable
{
    private readonly ConfigurationManager manager = new();

    public string? this[string key]
    {
        get => manager.To<IConfiguration>()[key];
        set => manager.To<IConfiguration>()[key] = value;
    }

    /// <inheritdoc/>
    public IDictionary<string, object> Properties => ((IConfigurationBuilder)manager).Properties;

    /// <inheritdoc/>
    public IList<IConfigurationSource> Sources => manager.Sources;

    /// <inheritdoc/>
    public IEnumerable<IConfigurationProvider> Providers => ((IConfigurationRoot)manager).Providers;

    /// <inheritdoc/>
    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return manager.GetChildren();
    }

    /// <inheritdoc/>
    public IConfigurationSection GetSection(string key)
    {
        return manager.GetSection(key);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        manager.Dispose();
    }

    /// <inheritdoc/>
    string? IConfiguration.this[string key]
    {
        get => manager.To<IConfiguration>()[key];
        set => manager.To<IConfiguration>()[key] = value;
    }

    /// <inheritdoc/>
    IEnumerable<IConfigurationProvider> IConfigurationRoot.Providers => manager.To<IConfigurationRoot>().Providers;

    /// <inheritdoc/>
    IChangeToken IConfiguration.GetReloadToken()
    {
        return manager.To<IConfiguration>().GetReloadToken();
    }

    /// <inheritdoc/>
    IConfigurationBuilder IConfigurationBuilder.Add(IConfigurationSource source)
    {
        return manager.To<IConfigurationBuilder>().Add(source);
    }

    /// <inheritdoc/>
    IConfigurationRoot IConfigurationBuilder.Build()
    {
        return manager.To<IConfigurationBuilder>().Build();
    }

    /// <inheritdoc/>
    void IConfigurationRoot.Reload()
    {
        manager.To<IConfigurationRoot>().Reload();
    }

    /// <inheritdoc/>
    IConfigurationSection IConfiguration.GetSection(string key)
    {
        return manager.To<IConfigurationRoot>().GetSection(key);
    }

    /// <inheritdoc/>
    IEnumerable<IConfigurationSection> IConfiguration.GetChildren()
    {
        return manager.To<IConfigurationRoot>().GetChildren();
    }
}
