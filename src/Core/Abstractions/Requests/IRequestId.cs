﻿namespace Core.Abstractions.Requests;

/// <summary>
/// Indicates an object contains <see cref="Guid"/> generated by the client.
/// </summary>
public interface IRequestId
{
    Guid RequestId { get; }
}
