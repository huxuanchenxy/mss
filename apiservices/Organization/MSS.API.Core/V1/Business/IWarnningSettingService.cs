using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
namespace MSS.API.Core.V1.Business
{
    public interface IWarnningSettingService
    {
        Task<ApiResult> SaveWarnningSetting(EarlyWarnningSetting setting);
        Task<ApiResult> UpdateWarnningSetting(EarlyWarnningSetting setting);

        Task<ApiResult> DeleteWarnningSetting(EarlyWarnningSetting setting);
        Task<ApiResult> DeleteListWarnningSetting(List<EarlyWarnningSetting> settings);
        Task<ApiResult> ListWarnningSettingByPage(int page, int size, string sort, string order,
            int? eqpTypeID, string paramID);
        Task<ApiResult> ListWarnningSettingExType();
        Task<ApiResult> GetWarnningSettingByID(int id);

        // 根据设备类型获取其下所有设备属性
        Task<ApiResult> ListEqpPropByEqpTypeID(int eqpTypeID);
    }
}
