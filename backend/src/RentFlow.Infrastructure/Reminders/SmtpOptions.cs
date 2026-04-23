namespace RentFlow.Infrastructure.Reminders;

public class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromEmail { get; set; } = "no-reply@rentflow.in";
    public string FromName { get; set; } = "RentFlow";
    public bool EnableSsl { get; set; } = true;
}
