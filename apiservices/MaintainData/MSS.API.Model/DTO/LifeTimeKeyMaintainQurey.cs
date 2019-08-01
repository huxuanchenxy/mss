using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public   class LifeTimeKeyMaintainQurey : BasePageParm
    {
        public string maintain_type { get; set; }

        public string device_id { get; set; }

        public string device_type { get; set; }

     
    }
}