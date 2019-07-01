using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace MSS.API.Operlog.Common
{
    public class GlobalActionFilter: IActionFilter
    {
        private  IConfiguration Configuration { get; }
        public GlobalActionFilter(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            if (context.HttpContext.Response.StatusCode == 200)
            {
                string controllername = context.RouteData.Values["Controller"].ToString();
                string actionname = context.RouteData.Values["Action"].ToString();
                string methodname = context.HttpContext.Request.Method.ToString();
                string header = context.HttpContext.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(header) && (methodname != "GET"))
                {
                    string token = string.Empty;
                    if (header.IndexOf("Bearer") >= 0)
                    {
                        token = header.Replace("Bearer", "").Trim();
                        UserTokenResponse response = new UserTokenResponse();
                        //AuthHelper au = new AuthHelper();
                        //int userid = au.GetUserId(context.HttpContext);

                        RedisHelper.Initialization(new CSRedis.CSRedisClient(this.Configuration["redis:ConnectionString"]));
                        //RedisMSSHelper.Init(this.Configuration["redis:ConnectionString"]);
                        int userid = int.Parse(RedisHelper.Get(token));
                        response = JsonConvert.DeserializeObject<UserTokenResponse>(RedisHelper.Get(userid.ToString()));
                        string localIPAddress = LocalIPAddress;
                        string localMacAddress = LocalMacAddress;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            dic.Add("controller_name", controllername);
                            dic.Add("action_name", actionname);
                            dic.Add("method_name", methodname);
                            dic.Add("acc_name", response.acc_name);
                            dic.Add("user_name", response.user_name);
                            dic.Add("ip", localIPAddress);
                            dic.Add("mac_add", localMacAddress);

                            FormUrlEncodedContent data = new FormUrlEncodedContent(dic);
                            string httpurl = Configuration["operlog:posturl"];
                            //string httpurl = "http://10.89.36.204:8003/api/v1/UserOperationLog/Add";
                            client.PostAsync(httpurl, data);
                        }
                    }
                    
                }
            }


        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }

        public static string LocalIPAddress
        {
            get
            {
                UnicastIPAddressInformation mostSuitableIp = null;
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var network in networkInterfaces)
                {
                    if (network.OperationalStatus != OperationalStatus.Up)
                        continue;
                    var properties = network.GetIPProperties();
                    if (properties.GatewayAddresses.Count == 0)
                        continue;

                    foreach (var address in properties.UnicastAddresses)
                    {
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;
                        if (IPAddress.IsLoopback(address.Address))
                            continue;
                        return address.Address.ToString();
                    }
                }

                return mostSuitableIp != null
                    ? mostSuitableIp.Address.ToString()
                    : "";
            }
        }


        public static string LocalMacAddress
        {
            get
            {
                string macadd = string.Empty;

                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    PhysicalAddress address = adapter.GetPhysicalAddress();
                    byte[] bytes = address.GetAddressBytes();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        // Display the physical address in hexadecimal.
                        //Console.Write("{0}", bytes[i].ToString("X2"));
                        macadd += bytes[i].ToString("X2");
                        // Insert a hyphen after each byte, unless we are at the end of the 
                        // address.
                        if (i != bytes.Length - 1)
                        {
                            //Console.Write("-");
                            macadd += "-";
                        }
                    }
                    break;//TODO
                }
                return macadd;
            }
        }

    }
}
