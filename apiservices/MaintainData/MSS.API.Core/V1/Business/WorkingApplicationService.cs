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
using MSS.API.Core.Common;

namespace MSS.API.Core.V1.Business
{
    public class WorkingApplicationService : IWorkingApplicationService
    {
        private readonly IWorkingApplicationRepo<WorkingApplication> _WorkingApplicationRepo;

        public WorkingApplicationService(IWorkingApplicationRepo<WorkingApplication> WorkingApplicationRepo)
        {
            _WorkingApplicationRepo = WorkingApplicationRepo;
        }


        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                WorkingApplication wa = await _WorkingApplicationRepo.GetByID(id);
                List<WorkingApplicationManager> wams= await _WorkingApplicationRepo.GetByWorkingApplication(id);
                if (wams!=null)
                {
                    wa.ManagerName = string.Join(',', wams.Select(a => a.ManagerName));
                    wa.ManagerMobile = string.Join(',', wams.Select(a => a.Mobile));
                }
                ret.data = wa;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }
    }
}
