namespace API.Model;

public record MailRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required ContactType ContactType { get; set; }
    public required string Message { get; set; }
}

