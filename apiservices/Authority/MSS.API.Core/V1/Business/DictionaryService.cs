using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using static MSS.API.Common.Const;

namespace MSS.API.Core.V1.Business
{
    public class DictionaryService: IDictionaryService
    {
        //private readonly ILogger<DictionaryService> _logger;
        private readonly IDictionaryRepo<Dictionary> _DictionaryRepo;

        public DictionaryService(IDictionaryRepo<Dictionary> DictionaryRepo)
        {
            //_logger = logger;
            _DictionaryRepo = DictionaryRepo;
        }
        public async Task<MSSResult<DictionaryView>> GetPageByParm(DictionaryQueryParm parm)
        {
            MSSResult<DictionaryView> mRet = new MSSResult<DictionaryView>();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows= parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                mRet = await _DictionaryRepo.GetPageByParm(parm);
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
                mRet.data = await _DictionaryRepo.GetByID(id);
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

        public async Task<MSSResult> Add(Dictionary Dictionary)
        {
            MSSResult mRet = new MSSResult();
            try
            { 
                DateTime dt = DateTime.Now;
                Dictionary.updated_time = dt;
                Dictionary.created_time = dt;
                mRet.data = await _DictionaryRepo.Add(Dictionary);
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

        public async Task<MSSResult> Update(Dictionary Dictionary)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                Dictionary.updated_time = DateTime.Now;
                mRet.data = await _DictionaryRepo.Update(Dictionary);
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
                mRet.data = await _DictionaryRepo.Delete(ids.Split(','));
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

        public async Task<MSSResult> GetSubByCode(string code)
        {
            MSSResult mRet = new MSSResult();
            try
            {
                mRet.data = await _DictionaryRepo.GetSubByCode(code);
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
