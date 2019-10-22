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
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.EventServer;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using MSS.API.Core.Models.Ex;
using Microsoft.Extensions.Configuration;

namespace MSS.API.Core.V1.Business
{
    public class MetroLineService : IMetroLineService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IMetroLineRepo<MetroLine> _lineRepo;
        private readonly IDistributedCache _cache;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly IConfiguration _configuration;

        public MetroLineService(IMetroLineRepo<MetroLine> lineRepo, IDistributedCache cache,
            IServiceDiscoveryProvider consulServiceProvider
            , IConfiguration configuration)
        {
            //_logger = logger;
            _lineRepo = lineRepo;
            _cache = cache;
            _consulServiceProvider = consulServiceProvider;
            _configuration = configuration;
        }

        public async Task<ApiResult> SaveLine(MetroLine line)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool exist = await _lineRepo.CheckExist(line);
                    if (!exist)
                    {
                        line = await _lineRepo.SaveLine(line);
                        ret.code = Code.Success;
                        ret.data = line;
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

        public async Task<ApiResult> UpdateLine(MetroLine line)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool exist = await _lineRepo.CheckExist(line);
                    if (!exist)
                    {
                        line = await _lineRepo.UpdateLine(line);
                        ret.code = Code.Success;
                        ret.data = line;
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

        public async Task<ApiResult> DeleteLine(List<MetroLine> lines)
        {
            ApiResult ret = new ApiResult();
            try
            {
                await _lineRepo.DeleteLine(lines);
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetMetroLineByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _lineRepo.GetMetroLineByID(id);
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

        public async Task<ApiResult> ListMetroLineByPage(MetroLineParam param)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _lineRepo.ListMetroLineByPage(param);
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

        public async Task<ApiResult> ListAllMetroLine()
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _lineRepo.ListAllMetroLine();
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

        public async Task<ApiResult> GetMetroStation()
        {
            ApiResult ret = new ApiResult();
            try
            {
                string url = _configuration["MetroStation"];
                var list =  HttpClientHelper.GetResponse<MetroLineObj>(url);

                List<locations> data = new List<locations>();
                foreach (var obj in list.levels[0].locations)
                {
                    if (data.Where(c=>c.title == obj.title).FirstOrDefault() == null)
                    {
                        data.Add(obj);
                    }
                }
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

        public class MetroLineObj
        {
            public List<levels> levels { get; set; }
        }

        public class levels
        {
            public List<locations> locations { get; set; }
        }
        public class locations
        {
            public string id { get; set; }
            public string title { get; set; }
        }

    }
}
