using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using MSS.API.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
public class CustomUserIdProvider : IUserIdProvider
{
    private readonly IDistributedCache _cache;
    private readonly IServiceDiscoveryProvider _consulServiceProvider;
    public CustomUserIdProvider(IDistributedCache cache, IServiceDiscoveryProvider consulServiceProvider)
    {
        _cache = cache;
        _consulServiceProvider = consulServiceProvider;
    }
    public virtual string GetUserId(HubConnectionContext connection)
    {
        string token = connection.GetHttpContext().Request.Query["access_token"];
        if (token != null)
        {
            int userid = int.Parse(_cache.GetString(token));
            // Context.UserIdentifier = userid;
            Task<bool> ret = isSuperUser(userid);
            if (ret.Result)
            {
                connection.Items.Add("isSuper", true);
            }
            connection.Items.Add("userID", userid);
            return userid.ToString();
        }
        else
        {
            return "";
        }
    }

    private async Task<bool> isSuperUser (int userid) {
        var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
        ApiResult result = HttpClientHelper.GetResponse<ApiResult>(_services + "/api/v1/user/" + userid);
        JObject jobj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
        bool isSuperUser = false;
        if ((bool)jobj["is_super"])
        {
            isSuperUser = true;
        }
        return isSuperUser;
    }
}