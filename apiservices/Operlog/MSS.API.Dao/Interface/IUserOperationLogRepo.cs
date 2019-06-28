using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MSS.API.Model.Const;

namespace MSS.API.Dao.Interface
{
    public interface IUserOperationLogRepo<T> where T:BaseEntity
    {
        Task<MSSResult<UserOperationLogView>> GetPageByParm(UserOperationLogParm parm);
        

        Task<int> Add(UserOperationLog obj);

    }
}
