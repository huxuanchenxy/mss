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
    public interface IEquipmentService
    {
        Task<ApiResult> Import(IFormFile file);
        Task<ApiResult> Save(Equipment eqp);
        Task<ApiResult> Update(Equipment eqp);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetPageByParm(EqpQueryParm parm);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetDetailByID(int id);
        Task<ApiResult> ListByPosition(int location, int locationBy, int eqpType);
        Task<ApiResult> ListByTopOrg(int topOrg, int line, int location = 0, int locationBy = 0);
        Task<ApiResult> ListByEqpType(string ids);
        Task<ApiResult> ListByTeam(int id);
        Task<ApiResult> GetAll();
        Task<ApiResult> CountAllEqp();

        // 
        Task<ApiResult> ListEqpByIDs(EqpQueryByIDParm parm);
    }
}
