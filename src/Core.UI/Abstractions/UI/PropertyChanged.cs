using System.ComponentModel;

namespace Core.Abstractions.UI;

/// <summary>
/// Provides data for the <see cref="IWhenPropertyChanged"/> interface.
/// </summary>
public sealed class PropertyChanged : PropertyChangedEventArgs
{
    public object? Sender { get; set; }

    public PropertyChanged(object? sender, string? propertyName) : base(propertyName)
    {
        Sender = sender;
    }
}
