using Microsoft.AspNetCore.Components;

namespace Core.Abstractions;

public interface IComponentWithAttributes
{
    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object?>? Attributes { get; set; }
}
