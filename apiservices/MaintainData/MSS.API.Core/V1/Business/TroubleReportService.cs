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
using MSS.API.Core.Common;

namespace MSS.API.Core.V1.Business
{
    public class TroubleReportService : ITroubleReportService
    {
        private readonly ITroubleReportRepo<TroubleReport> _TroubleReportRepo;

        public TroubleReportService(ITroubleReportRepo<TroubleReport> TroubleReportRepo)
        {
            _TroubleReportRepo = TroubleReportRepo;
        }


        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _TroubleReportRepo.GetByID(id);
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
