using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System.Threading.Tasks;
using static MSS.API.Model.Const;

namespace MSS.API.Operlog.V1.Business
{
    public interface IUserOperationLogService
    {
        Task<MSSResult<UserOperationLogView>> GetPageByParm(UserOperationLogParm parm);

    }
}
