using MSS.API.Common;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MSS.API.Utility.Const;

namespace MSS.API.Core.V1.Business
{
    public interface IUserService
    {
        Task<MSSResult<UserView>> GetPageByParm(UserQueryParm parm);
        Task<ApiResult> GetByID(int id);

        Task<MSSResult> Add(User User);
        Task<MSSResult> Update(User User);
        Task<MSSResult> Delete(string ids);
        Task<MSSResult> GetAll();
        Task<MSSResult> ChangePwd(int userID, string oldPwd, string newPwd);
        Task<MSSResult> ResetPwd(string ids);

        Task<ApiResult> CheckUserLogin(string acc,string password);

        Task<MSSResult<MenuTree>> GetMenu();//int? userID = null
        Task<ApiResult> GetActionByUser();
    }
}
