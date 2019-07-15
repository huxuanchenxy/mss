using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IEquipmentRepo<T> where T:BaseEntity
    {
        Task<Equipment> Save(Equipment eqp);
        Task<int> Update(Equipment eqp);
        Task<int> Delete(string[] ids, int userID);
        Task<EqpView> GetPageByParm(EqpQueryParm parm);
        Task<Equipment> GetByID(int id);
        Task<Equipment> GetDetailByID(int id);
        Task<List<Equipment>> ListByEqpType(string ids);
        Task<List<UploadFileEqp>> ListByEqp(int id);
        Task<List<Equipment>> ListByPosition(int location, int locationBy);
        Task<List<Equipment>> GetAll();
        Task<List<AllArea>> GetAllArea();
    }
}
