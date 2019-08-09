using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Core.Common;
using MSS.API.Model.DTO;
using MSS.API.Common;

namespace MSS.API.Core.V1.Business
{
    public interface IEmergencyPlanService
    {
        Task<ApiResult> Save(EmergencyPlan ePlan);
        Task<ApiResult> Update(EmergencyPlan ePlan);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetPageByParm(EPlanQueryParm parm);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetAll();
    }
}
