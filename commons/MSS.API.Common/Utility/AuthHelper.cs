using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MSS.API.Common.Utility
{
    public class AuthHelper
    {
        private static readonly IConfigurationBuilder ConfigurationBuilder = new ConfigurationBuilder();
        private static IConfigurationRoot _configuration;

        public AuthHelper()
        {
            _configuration = ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(cfg =>
                {
                    cfg.Path = "appsettings.json";
                    cfg.ReloadOnChange = true;
                    cfg.Optional = false;
                })
                //Build方法的调用要在AddJsonFile之后，否则生成的IConfigurationRoot实例的
                //Providers属性不包含任何元素而导致无法读取文件中的信息
                .Build();
        }
        public int GetUserId(HttpContext context)
        {
            int userid = 0;
            
            var _redisconfig = _configuration["redis:connectionString"];
            RedisMSSHelper.Init(_redisconfig);
            string token = string.Empty;
            var head = context.Request.Headers["Authorization"].ToString();
            if (head.IndexOf("Bearer") >= 0)
            {
                token = head.Replace("Bearer", "").Trim();
                try
                {
                    userid = int.Parse(RedisMSSHelper.Get(token));
                }
                catch (Exception ex)
                {

                }
                
            }
            
            return userid;
        }
    }
}
