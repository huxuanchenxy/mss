using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
public class CustomUserIdProvider : IUserIdProvider
{
    private readonly IDistributedCache _cache;
    public CustomUserIdProvider(IDistributedCache cache)
    {
        _cache = cache;
    }
    public virtual string GetUserId(HubConnectionContext connection)
    {
        string token = connection.GetHttpContext().Request.Query["access_token"];
        if (token != null)
        {
            int userid = int.Parse(_cache.GetString(token));
            // Context.UserIdentifier = userid;
            connection.Items.Add("userID", userid);
            return userid.ToString();
        }
        else
        {
            return "";
        }
    }
}