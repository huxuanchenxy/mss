using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Operlog.Common
{
    public class UserTokenResponse
    {
        public string acc_name { get; set; }
        public string user_name { get; set; }

        public string ip { get; set; }
        public string mac { get; set; }
    }
}
