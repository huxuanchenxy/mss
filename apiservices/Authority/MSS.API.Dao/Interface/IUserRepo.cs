using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MSS.API.Utility.Const;

namespace MSS.API.Dao.Interface
{
    public interface IUserRepo<T> where T : BaseEntity
    {
        Task<MSSResult<UserView>> GetPageByParm(UserQueryParm parm);
        Task<User> GetByID(int Id);

        Task<User> GetByAcc(string acc);
        Task<int> GetUserCountByRole(string[] ids);

        Task<int> Add(User user);

        Task<int> Update(User user);

        Task<int> Delete(string[] ids,int userID);
        Task<bool> IsInOrg(string[] ids);
        Task<List<User>> GetAll();
        Task<List<User>> GetAllContainSuper();
        Task<int> ChangePwd(User user);
        Task<int> ResetPwd(string[] ids, int userID);
    }
}
