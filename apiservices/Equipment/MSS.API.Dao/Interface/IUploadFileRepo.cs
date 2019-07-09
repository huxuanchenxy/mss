using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IUploadFileRepo<T>
    {
        Task<UploadFile> Save(UploadFile uploadFile);
        Task<int> Delete(int id);
        Task<UploadFile> GetByID(int id);
        Task<List<UploadFile>> ListByForeignAndType(int id, int type);
        Task<List<UploadFile>> ListAll();
    }
}
