using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace System.API.Service 
{
    public interface IService
    {
        IConfigBigAreaService _IConfigBigAreaService { get; set; }
        IConfigMidAreaService _IConfigMidAreaService { get; set; }
       
    }
}
