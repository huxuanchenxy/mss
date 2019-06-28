using MSS.API.Operlog.Model.Data;
using MSS.API.Operlog.Model.DTO;
using System.Threading.Tasks;
using static MSS.API.Operlog.Model.Const;

namespace MSS.API.Operlog.V1.Business
{
    public interface IUserOperationLogService
    {
        Task<MSSResult<UserOperationLogView>> GetPageByParm(UserOperationLogParm parm);

        Task<int> Add(UserOperationLog obj);

    }
}
