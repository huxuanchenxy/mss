using MSS.API.Operlog.Model.Data;
using MSS.API.Operlog.Model.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MSS.API.Operlog.Model.Const;

namespace MSS.API.Operlog.Dao.Interface
{
    public interface IUserOperationLogRepo<T> where T:BaseEntity
    {
        Task<MSSResult<UserOperationLogView>> GetPageByParm(UserOperationLogParm parm);
        

        Task<int> Add(UserOperationLog obj);

    }
}
