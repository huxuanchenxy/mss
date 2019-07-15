using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using static MSS.API.Utility.Const;
using MSS.API.Utility;
using MSS.API.Common.Utility;
using MSS.API.Common;

namespace MSS.API.Core.V1.Business
{
    public class UserService : IUserService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IUserRepo<User> _UserRepo;
        private readonly IActionRepo<ActionInfo> _ActionRepo;

        private readonly int userID;

        public UserService(IUserRepo<User> userRepo, IActionRepo<ActionInfo> actionRepo, IAuthHelper auth)
        {
            //_logger = logger;
            _UserRepo = userRepo;
            _ActionRepo = actionRepo;
            userID = auth.GetUserId();
        }
        public async Task<MSSResult<UserView>> GetPageByParm(UserQueryParm parm)
        {
            MSSResult<UserView> mRet = new MSSResult<UserView>();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                mRet = await _UserRepo.GetPageByParm(parm);
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }
        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult mRet = new ApiResult();
            try
            {
                //if (id == 0)
                //{
                //    mRet.code = ErrType.ErrParm;
                //    mRet.msg = "参数不正确，id不可为0";
                //    return mRet;
                //}
                mRet.data = await _UserRepo.GetByID(id);
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = Code.Failure;
                mRet.msg = ex.Message;
                return mRet;
            }
        }
        public async Task<MSSResult> Add(User user)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                User u =await _UserRepo.GetByAcc(user.acc_name);
                if (u==null)
                {
                    Encrypt en = new Encrypt();
                    int r = new Random().Next(1, PWD_RANDOM_MAX);
                    user.password = en.DoEncrypt(INIT_PASSWORD,r);
                    user.random_num = r;
                    DateTime dt = DateTime.Now;
                    user.updated_time = dt;
                    user.created_time = dt;
                    user.created_by = userID;
                    user.updated_by = userID;
                    mRet.data = await _UserRepo.Add(user);
                    mRet.code = (int)ErrType.OK;
                }
                else
                {
                    mRet.code = (int)ErrType.Repeat;
                    mRet.msg = "登录账号重复";
                }
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<MSSResult> Update(User user)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                user.updated_time = DateTime.Now;
                user.updated_by = userID;
                mRet.data = await _UserRepo.Update(user);
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<MSSResult> Delete(string ids)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                mRet.data = await _UserRepo.Delete(ids.Split(','),userID);
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<MSSResult> GetAll()
        {
            MSSResult mRet = new MSSResult();
            try
            {
                mRet.data = await _UserRepo.GetAll();
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<MSSResult> ChangePwd(int userID,string oldPwd, string newPwd)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                User u = await _UserRepo.GetByID(userID);
                if (u != null)
                {
                    Encrypt en = new Encrypt();
                    if (en.DoEncrypt(oldPwd,u.random_num)!=u.password)
                    {
                        mRet.code = (int)ErrType.ErrPwd;
                        mRet.msg = "密码错误";
                    }
                    int r = new Random().Next(1, PWD_RANDOM_MAX);
                    u.password = en.DoEncrypt(newPwd,r);
                    u.random_num = r;
                    DateTime dt = DateTime.Now;
                    u.updated_time = dt;
                    u.updated_by = userID;
                    mRet.data = await _UserRepo.ChangePwd(u);
                    mRet.code = (int)ErrType.OK;
                }
                else
                {
                    mRet.code = (int)ErrType.Repeat;
                    mRet.msg = "登录账号重复";
                }
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<MSSResult> ResetPwd(string ids)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                mRet.data = await _UserRepo.ResetPwd(ids.Split(','),userID);
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<MSSResult<MenuTree>> CheckUserLogin(string acc, string pwd)
        {
            MSSResult<MenuTree> mRet = new MSSResult<MenuTree>();
            try
            {
                User ui = await _UserRepo.GetByAcc(acc);
                if (ui != null)
                {
                    Encrypt encrypt = new Encrypt();
                    string strPwd = encrypt.DoEncrypt(pwd, ui.random_num);
                    if (ui.password != strPwd)
                    {
                        mRet.code = (int)ErrType.ErrPwd;
                        mRet.msg = "密码错误";
                        //return mRet;
                    }
                    //else
                    //{
                    //    //获取此人对应的权限
                    //    if (ui.is_super)
                    //    {
                    //        mRet =await GetMenu();
                    //    }
                    //    else
                    //    {
                    //        mRet = await GetMenu(ui.id);
                    //    }
                    //    mRet.relatedData = ui;
                    //    return mRet;
                    //}
                }
                else
                {
                    mRet.code =(int)ErrType.NoRecord;
                    mRet.msg = "账号错误";
                }
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<MSSResult<MenuTree>> GetMenu()
        {
            User u = await _UserRepo.GetByID(userID);
            MSSResult<MenuTree> mRet = new MSSResult<MenuTree>();
            try
            {
                List<ActionAll> laa = new List<ActionAll>();
                //允许勾选和不对外开放的且为菜单的url
                if (u.is_super)
                {
                    laa = await _ActionRepo.GetActionAll();
                    mRet.data = ActionHelper.GetMenuTree(laa.Where(
                        a => (a.Level == (int)ACTION_LEVEL.AllowSelection ||
                        a.Level == (int)ACTION_LEVEL.NotAllowAll) && a.GroupID > 0).ToList());
                }
                //根据用户ID获取对应菜单权限
                else
                {
                    laa = await _ActionRepo.GetActionByUser(userID);
                    mRet.data = ActionHelper.GetMenuTree(laa.Where(a => a.GroupID > 0).ToList());
                }
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }

        public async Task<ApiResult> GetActionByUser()
        {
            User u = await _UserRepo.GetByID(userID);
            ApiResult mRet = new ApiResult();
            try
            {
                List<ActionAll> laa = new List<ActionAll>();
                if (u.is_super)
                {
                    mRet.data = await _ActionRepo.GetActionAll();
                }
                //根据用户ID获取对应所有url权限
                else
                {
                    mRet.data = await _ActionRepo.GetActionByUser(userID);
                }
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = Code.Failure;
                mRet.msg = ex.Message;
                return mRet;
            }
        }
    }
}
