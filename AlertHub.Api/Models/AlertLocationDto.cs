namespace AlertHub.Api.Models;

public sealed record AlertLocationDto(string Id, string Title, string Description, DateTimeOffset Timestamp)
{ 
    static public AlertLocationDto Create(string id, string title, string description, DateTimeOffset timestamp)
    {
        return new AlertLocationDto(id, title, description, timestamp);
    }
}
