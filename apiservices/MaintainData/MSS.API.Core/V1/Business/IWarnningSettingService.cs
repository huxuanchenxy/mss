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
        Task<ApiResult> ListWarnningSettingByPage(int page, int size, string sort, string order);
    }
}
