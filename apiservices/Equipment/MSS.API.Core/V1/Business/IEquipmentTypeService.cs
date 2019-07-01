﻿using MSS.API.Model.Data;
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
        Task<ApiResult> Save(EquipmentType eqpType, List<IFormFile> file);
        Task<ApiResult> Update(EquipmentType eqpType, List<IFormFile> file);
        Task<ApiResult> Delete(string ids, int userID);
        Task<ApiResult> GetPageByParm(EqpTypeQueryParm parm);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetAll();
    }
}
