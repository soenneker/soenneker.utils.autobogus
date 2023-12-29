using System.Net;

namespace Soenneker.Utils.AutoBogus.Tests.Playground.Dtos;

public sealed class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public IPAddress Location { get; set; }
}