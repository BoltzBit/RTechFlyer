namespace API.AWS;

public record AwsOptions
{
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string Region { get; set; }
}

public record EmailOptions
{
    public required string Sender { get; set; }
    public required string To { get; set; }
}