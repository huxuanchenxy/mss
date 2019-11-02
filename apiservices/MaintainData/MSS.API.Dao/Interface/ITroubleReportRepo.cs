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
        Task<int> UpdateStatus(string[] ids, int userID, TroubleStatus status);
        Task<TroubleReport> Save(TroubleReport troubleReport);
        Task<string> GetLineCodeByID(int id);
        Task<string> GetNodeCodeByID(int id);
        Task<string> GetLastNumByDate(DateTime dt);
        Task<List<TroubleEqp>> ListEqpByTrouble(int trouble);
        Task<int> Update(TroubleReport troubleReport);
    }
}
