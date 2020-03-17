using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
   public interface ITroubleReportRepo<T>
    {
        Task<List<TroubleReport>> ListAll();
        Task<List<TroubleReport>> ListByStatus(int status);
        Task<TroubleReport> GetByID(int id);
        Task<TroubleReportView> ListPage(TroubleReportParm parm);
        Task<int> UpdateStatus(string[] ids, int userID, TroubleStatus status,TroubleOperation operation);
        Task<TroubleReport> Save(TroubleReport troubleReport);
        Task<string> GetLineCodeByID(int id);
        Task<string> GetNodeCodeByID(int id);
        Task<string> GetLastNumByDate(DateTime dt);
        Task<int> GetOrgNodeByUser(int id);

        Task<int> SaveTroubleEqp(TroubleEqp troubleEqp);
        Task<List<TroubleEqp>> ListEqpByTrouble(int trouble,int topOrg=0, int orgNode = 0);
        Task<List<TroubleEqp>> ListEqpIDByTroubles(string[] troubles);
        Task<int> UpdateTroubleEqp(List<TroubleEqp> troubleEqp);
        Task<List<int>> ListOrgTopByTrouble(int trouble);
        Task<List<TroubleHistory>> ListOperationByTroubles(IEnumerable<int> trouble);

        Task<int> Update(TroubleReport troubleReport, TroubleHistory troubleHistory);
        Task<int> SaveHistory(TroubleHistory troubleHistory);
        Task<int> SaveHistory(List<TroubleHistory> troubleHistory);
        Task<List<TroubleHistory>> ListHistoryByTrouble(int id);
        Task<int> GetNoByStatus(AttandenceStatus attandenceStatus);

        Task<TroubleDeal> SaveDeal(TroubleDeal troubleDeal);
        Task<int> UpdateDeal(TroubleDeal troubleDeal);
        Task<List<TroubleDeal>> ListDealByTrouble(int trouble, int topOrg = 0);
        Task<int> UpdateDealResult(int id, int userID, TroubleStatus status, TroubleOperation operation, string unpassReason = "");

        Task<int> SaveUploadFile(UploadFileRelation uploadFileRelation);
    }
}
