using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using static MSS.API.Utility.Const;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSS.API.Core.V1.Business
{
    public class ActionService: IActionService
    {
        //private readonly ILogger<ActionService> _logger;
        private readonly IActionRepo<ActionInfo> _ActionRepo;

        public ActionService(IActionRepo<ActionInfo> actionRepo)
        {
            //_logger = logger;
            _ActionRepo = actionRepo;
        }
        public async Task<MSSResult<ActionView>> GetPageByParm(ActionQueryParm parm)
        {
            MSSResult<ActionView> mRet = new MSSResult<ActionView>();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows= parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                mRet = await _ActionRepo.GetPageByParm(parm);
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
        public async Task<MSSResult> GetByID(int id)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                if (id==0)
                {
                    mRet.code = (int)ErrType.ErrParm;
                    mRet.msg = "参数不正确，id不可为0";
                    return mRet;
                }
                mRet.data = await _ActionRepo.GetByID(id);
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

        public async Task<MSSResult> Add(ActionInfo action)
        {
            MSSResult mRet = new MSSResult();
            try
            { 
                DateTime dt = DateTime.Now;
                action.updated_time = dt;
                action.created_time = dt;
                mRet.data = await _ActionRepo.Add(action);
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

        public async Task<MSSResult> Update(ActionInfo action)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                action.updated_time = DateTime.Now;
                mRet.data = await _ActionRepo.Update(action);
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
                mRet.data = await _ActionRepo.Delete(ids.Split(','));
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
                mRet.data = await _ActionRepo.GetAll();
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

        public async Task<MSSResult> GetMenu()
        {
            MSSResult mRet = new MSSResult();
            try
            {
                mRet.data = await _ActionRepo.GetMenu();
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

        /// <summary>
        /// 非超级用户可配置的所有权限
        /// </summary>
        /// <returns></returns>
        public async Task<MSSResult<ActionTree>> GetActionTree()
        {
            MSSResult<ActionTree> mRet = new MSSResult<ActionTree>();
            try
            {
                List<ActionAll> laa = new List<ActionAll>();
                //允许勾选
                laa = await _ActionRepo.GetActionAll();
                mRet.data = ActionHelper.GetActionTree(laa.Where(
                    a => a.Level == (int)ACTION_LEVEL.AllowSelection).ToList());
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
    }
}
