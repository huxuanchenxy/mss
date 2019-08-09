using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
   public interface IEmergencyPlanRepo<T>
    {
        Task<EmergencyPlan> Save(EmergencyPlan ePlan);
        Task<int> Update(EmergencyPlan ePlan);
        Task<int> Delete(string[] ids);
        Task<EPlanView> GetPageByParm(EPlanQueryParm parm);
        Task<EmergencyPlan> GetByID(int id);
        Task<List<EmergencyPlan>> GetAll();

    }
}
