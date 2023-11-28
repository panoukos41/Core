using System.Diagnostics.CodeAnalysis;

namespace Core.Commons.Requests;

public sealed record CreatedResponse
{
    public required string Id { get; set; }

    public CreatedResponse()
    {
    }

    [SetsRequiredMembers]
    public CreatedResponse(string id)
    {
        Id = id;
    }

    [SetsRequiredMembers]
    public CreatedResponse(Uuid id)
    {
        Id = id.ToString();
    }
}
