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
    public interface IEquipmentConfigService
    {
        Task<ApiResult> Save(EquipmentConfig obj);
        Task<ApiResult> Update(EquipmentConfig obj);
        Task<ApiResult> Delete(string ids, int userID);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetAll();

    }
}
