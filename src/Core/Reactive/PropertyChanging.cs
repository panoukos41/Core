using System.ComponentModel;

namespace Core.Reactive;

/// <summary>
/// Provides data for the <see cref="IWhenPropertyChanging"/> interface.
/// </summary>
public sealed class PropertyChanging : PropertyChangingEventArgs
{
    public object? Sender { get; set; }

    public PropertyChanging(object? sender, string? propertyName) : base(propertyName)
    {
        Sender = sender;
    }
}
