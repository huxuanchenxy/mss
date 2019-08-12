using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Model.DTO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;

namespace MSS.API.Core.V1.Business
{
    public class WarnningSettingService : IWarnningSettingService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IWarnningSettingRepo<EarlyWarnningSetting> _warnRepo;

        public WarnningSettingService(IWarnningSettingRepo<EarlyWarnningSetting> warnRepo)
        {
            //_logger = logger;
            _warnRepo = warnRepo;
        }

        public async Task<ApiResult> SaveWarnningSetting(EarlyWarnningSetting setting)
        {
            ApiResult ret = new ApiResult();
            try {
                using (TransactionScope scope = new TransactionScope())
                {
                    // bool isExist = await _warnRepo.CheckWarnExist(setting);
                    bool isExist = false; // 设备类型和预警参数不做唯一检查
                    if (!isExist)
                    {
                        var data = await _warnRepo.SaveWarnningSetting(setting);

                        //保存扩展属性
                        if (setting.SettingEx != null && setting.SettingEx.Count > 0)
                        {
                            bool propSavedOk = await _saveSettingEx(data);
                            if (!propSavedOk)
                            {
                                throw new Exception("存储扩展属性失败");
                            }
                        }
                        ret.code = Code.Success;
                        ret.data = data;
                    }
                    else
                    {
                        ret.code = Code.DataIsExist;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            
            return ret;
        }

        private async Task<bool> _saveSettingEx(EarlyWarnningSetting setting)
        {
            foreach (EarlyWarnningSettingEx prop in setting.SettingEx)
            {
                prop.EarlyWarnningID = setting.ID;
            }
            await _warnRepo.SaveWarnningSettingEx(setting.SettingEx);
            return true;
        }

        public async Task<ApiResult> UpdateWarnningSetting(EarlyWarnningSetting setting)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var data = await _warnRepo.UpdateWarnningSetting(setting);
                    //删除已有属性
                    await _warnRepo.DeleteWarnningSettingEx(setting);
                    //保存扩展属性
                    if (setting.SettingEx != null && setting.SettingEx.Count > 0)
                    {
                        bool propSavedOk = await _saveSettingEx(data);
                        if (!propSavedOk)
                        {
                            throw new Exception("存储节点属性失败");
                        }
                    }
                    ret.code = Code.Success;
                    ret.data = data;
                    
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> DeleteWarnningSetting(EarlyWarnningSetting setting)
        {
            ApiResult ret = new ApiResult();
            try
            {
                await _warnRepo.DeleteWarnningSetting(setting);
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }
        public async Task<ApiResult> DeleteListWarnningSetting(List<EarlyWarnningSetting> settings)
        {
            ApiResult ret = new ApiResult();
            try
            {
                foreach (EarlyWarnningSetting setting in settings)
                {
                    await _warnRepo.DeleteWarnningSetting(setting);
                }
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListWarnningSettingByPage(int page, int size, string sort, string order,
            int? eqpTypeID, string paramID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    int count = await _warnRepo.Count(eqpTypeID, paramID);
                    var list = await _warnRepo.ListWarnningSettingByPage(page, size, sort, order, eqpTypeID, paramID);
                    ret.code = Code.Success;
                    ret.data = new {
                        total = count,
                        rows = list
                    };
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }
        public async Task<ApiResult> ListWarnningSettingExType()
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListEarlyWarnningExType();
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetWarnningSettingByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.GetWarnningSettingByID(id);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListEqpPropByEqpTypeID(int eqpTypeID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListEqpPropByEqpTypeID(eqpTypeID);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }
    }
}
