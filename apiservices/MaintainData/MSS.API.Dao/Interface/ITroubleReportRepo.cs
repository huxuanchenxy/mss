using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
   public interface ITroubleReportRepo<T>
    {
        Task<TroubleReport> GetByID(int id);
        Task<TroubleReportView> ListPage(TroubleReportParm parm);
    }
}
