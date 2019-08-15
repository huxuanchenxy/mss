using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Model.DTO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;

namespace MSS.API.Core.V1.Business
{
    public class WarnningSettingService : IWarnningSettingService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IWarnningSettingRepo<EarlyWarnningSetting> _warnRepo;

        public WarnningSettingService(IWarnningSettingRepo<EarlyWarnningSetting> warnRepo)
        {
            //_logger = logger;
            _warnRepo = warnRepo;
        }

        
    }
}
