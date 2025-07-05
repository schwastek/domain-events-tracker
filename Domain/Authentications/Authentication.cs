using Domain.Common;

namespace Domain.Authentications;

public class Authentication : IIdentity<long>
{
    public long Id { get; private set; }
    public string Username { get; private set; }

    private Authentication() { }

    public Authentication(string username)
    {
        Username = username;
    }

    public void SetUsername(string username)
    {
        Username = username;
    }

    public override string ToString()
    {
        return $"Authentication [Username: {Username}]";
    }
}
