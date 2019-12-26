using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Core.Common;
using MSS.API.Model.DTO;
using MSS.API.Common;

namespace MSS.API.Core.V1.Business
{
    public interface IEqpHistoryService
    {
        Task<ApiResult> ListByEqp(int id,bool isHide);
        Task<ApiResult> ListByType(string types);
    }
}
