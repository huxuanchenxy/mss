using System;
using System.Collections.Generic;
using System.Text;

namespace System.API.Service
{
    public class BaseService : IService
    {

        public BaseService(IConfigBigAreaService IConfigBigAreaService,
                               IConfigMidAreaService IConfigMidAreaService
                               )
        {
            this._IConfigBigAreaService = IConfigBigAreaService;
            this._IConfigMidAreaService = IConfigMidAreaService;
           
        } 
        public IConfigBigAreaService _IConfigBigAreaService { get; set; } 
        public IConfigMidAreaService _IConfigMidAreaService { get; set; }
    }
}
