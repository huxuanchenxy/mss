using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.Data
{
    public class LifeTimeKeyMaintainInfo : BaseEntity
    {
        //public int id { get; set; }
        public string device_code { get; set; }
        public int device_type_id { get; set; }
        public string device_type_name { get; set; }
        public int device_id { get; set; }
        public string device_name { get; set; }
        public int team_group_id { get; set; }
        public string team_group_name { get; set; }
        public string team_group_path { get; set; }
       
        public string setup_date { get; set; } 

        public string life_time { get; set; }

        public string next_maintain_type { get; set; }

        public string next_maintain_date { get; set; }
        
        public string updated_name { get; set; }  

        public string cur_state { get; set; }
    }
}