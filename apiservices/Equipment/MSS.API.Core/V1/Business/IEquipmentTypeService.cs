using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
using Microsoft.AspNetCore.Http;

namespace MSS.API.Core.V1.Business
{
    public interface IEquipmentTypeService
    {
        Task<ApiResult> Save(EquipmentType eqpType);
        Task<ApiResult> Update(EquipmentType eqpType);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetPageByParm(EqpTypeQueryParm parm);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetAll();

        // 插入模拟数据
        Task<ApiResult> MockData();
    }
}
