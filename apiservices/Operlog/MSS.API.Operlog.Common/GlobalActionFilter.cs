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
using System.Text;

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
                        //response.acc_name = "houtai111";
                        //response.user_name = "houtaitest111";

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            string httpurl = Configuration["operlog:posturl"];
                            UserOperationLog parmobj = new UserOperationLog()
                            {
                                controller_name = controllername,
                                action_name = actionname,
                                method_name = methodname,
                                acc_name = response.acc_name,
                                user_name = response.user_name,
                                ip = localIPAddress,
                                mac_add = localMacAddress
                            };
                            var content = new StringContent(JsonConvert.SerializeObject(parmobj), Encoding.UTF8, "application/json");
                            //var ret = client.PostAsync(httpurl, content).ContinueWith(responseTask => {
                            //    var result = responseTask.Result;
                            //});
                            HttpResponseMessage httpresp = client.PostAsync(httpurl, content).Result;
                            var temp = httpresp.Content.ReadAsStringAsync().Result;
                        }
                    }
                    
                }
            }


        }

        public static string HttpPost(string url, string postData = null, string contentType = null, int timeOut = 30, Dictionary<string, string> headers = null)
        {
            postData = postData ?? "";
            using (HttpClient client = new HttpClient())
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
                using (HttpContent httpContent = new StringContent(postData, Encoding.UTF8))
                {
                    if (contentType != null)
                        httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }


        public class UserOperationLog
        {
            public string controller_name { get; set; }
            public string action_name { get; set; }

            public string method_name { get; set; }

            public string acc_name { get; set; }

            public string user_name { get; set; }

            public string ip { get; set; }

            public string mac_add { get; set; }
            public int id { get; set; }
            public DateTime created_time { get; set; }
            public int created_by { get; set; }
            public DateTime updated_time { get; set; }
            public int updated_by { get; set; }
            public int is_del { get; set; }

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
