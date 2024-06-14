namespace Core.Blazor.Reactive.Forms.Primitives;

/// <summary>
/// A control can have several different statuses.
/// </summary>
public enum ControlStatus
{
    /// <summary>
    /// Reports that a control is valid, meaning that no errors exist in the input value.
    /// </summary>
    Valid,

    /// <summary>
    /// Reports that a control is invalid, meaning that an error exists in the input value.
    /// </summary>
    Invalid,

    /// <summary>
    /// Reports that a control is pending, meaning that async validation is 
    /// occurring and errors are not yet available for the input value.
    /// </summary>
    Pending,

    /// <summary>
    /// Reports that a control is disabled, meaning that the control is exempt from ancestor calculations of validity or value.
    /// </summary>
    Disabled = -1
}
