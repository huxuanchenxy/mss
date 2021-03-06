﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Operlog.Model.Data
{
    public class UserOperationLog : BaseEntity
    {
        public string controller_name { get; set; }
        public string action_name { get; set; }

        public string method_name { get; set; }

        public string acc_name { get; set; }

        public string user_name { get; set; }

        public string ip { get; set; }

        public string mac_add { get; set; }
        public string response_description { get; set; }
        public string request_description { get; set; }
        public int user_id { get; set; }

    }
}
