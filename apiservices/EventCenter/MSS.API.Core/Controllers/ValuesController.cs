using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.API.Core.Common;
using MSS.API.Core.EventServer;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using MSS.Common.Consul;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSS.API.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly IWarnningSettingRepo<EarlyWarnningSetting> _warnSettingRepo;
        private readonly GlobalDataManager _globalDataManager;

        public ValuesController(ILogger<InitPidTableJob> logger, IDistributedCache cache,
            IServiceDiscoveryProvider consulServiceProvider, IWarnningSettingRepo<EarlyWarnningSetting> warnSettingRepo,
            GlobalDataManager globalDataManager)
        {
            _logger = logger;
            _cache = cache;
            _consulServiceProvider = consulServiceProvider;
            _warnSettingRepo = warnSettingRepo;
            _globalDataManager = globalDataManager;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetPidTable(int id)
        {
            try
            {
                _logger.LogInformation("开始同步pid表");
                var _services = await _consulServiceProvider.GetServiceAsync("RDBRTDBService");
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.
                    GetSingleItemRequest(_services + "/api/v1/TableInfo");
                if (result.code == Code.Success)
                {
                    // 获取所有设备
                    List<Equipment> list = await _warnSettingRepo.ListAllEquipment();
                    JArray data_table = JsonConvert.DeserializeObject<JArray>(result.data.ToString());
                    foreach (JObject item in data_table)
                    {
                        string table = item["TableName"].ToString();
                        if (table.IndexOf("r_") >= 0)
                        {
                            result = await httpHelper.
                                GetSingleItemRequest(_services + "/api/v1/TableInfo/" + table);
                            // 获取每张表中pid
                            if (result.code == Code.Success)
                            {
                                List<PidTable> pidTableData = new List<PidTable>();
                                JArray data_pids = JsonConvert.DeserializeObject<JArray>(result.data.ToString());
                                foreach (JObject item_pid in data_pids)
                                {
                                    string pid = item_pid["PID"].ToString();
                                    string[] pid_seg = pid.Split('_');
                                    if (pid_seg.Length != 5)
                                    {
                                        _logger.LogInformation("pid:" + pid + "格式不正确");
                                        continue;
                                    }
                                    // 
                                    PidTable newdata = new PidTable();
                                    newdata.pid = pid;
                                    // 根据设备code查mss中设备id
                                    string eqpCode = pid_seg[3];
                                    newdata.EqpID = null;
                                    newdata.StationId = pid_seg[0];
                                    newdata.ExpertId = pid_seg[1];
                                    newdata.DeviceType = pid_seg[2];
                                    newdata.DeviceCode = pid_seg[3];
                                    Equipment eqp = list.Where(c => c.EqpCode.Equals(eqpCode))
                                        .FirstOrDefault();
                                    if (eqp != null)
                                    {
                                        newdata.EqpID = eqp.ID;
                                    }

                                    // 设备属性
                                    newdata.prop = pid_seg[4];
                                    newdata.Des = item_pid["Des"].ToString();
                                    // 判断是否为故障点
                                    if (newdata.Des.IndexOf("故障") >= 0)
                                    {
                                        newdata.PidType = 1;
                                    }


                                    if (item_pid["UP"] != null && item_pid["UP"].Type != JTokenType.Null)
                                    {
                                        newdata.UP = Convert.ToDouble(item_pid["UP"]);
                                    }
                                    if (item_pid["DW"] != null && item_pid["DW"].Type != JTokenType.Null)
                                    {
                                        newdata.DW = Convert.ToDouble(item_pid["DW"]);
                                    }
                                    if (item_pid["UUP"] != null && item_pid["UUP"].Type != JTokenType.Null)
                                    {
                                        newdata.UUP = Convert.ToDouble(item_pid["UUP"]);
                                    }
                                    if (item_pid["DDW"] != null && item_pid["DDW"].Type != JTokenType.Null)
                                    {
                                        newdata.DDW = Convert.ToDouble(item_pid["DDW"]);
                                    }
                                    pidTableData.Add(newdata);
                                }
                                // 删除表中原有记录然后添加
                                if (pidTableData.Count > 0)
                                {
                                    using (TransactionScope scope = new TransactionScope())
                                    {
                                        await _warnSettingRepo.DeletePidTable(pidTableData);
                                        await _warnSettingRepo.SavePidTable(pidTableData);
                                        scope.Complete();
                                    }
                                }
                            }
                            else
                            {
                                _logger.LogError("/api/v1/TableInfo/" + table + "接口调用失败");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError("/api/v1/TableInfo 接口调用失败");
                }
                _logger.LogInformation("同步pid表结束");
                // 发送通知 更新pid内存表
                _globalDataManager.postUpdateEvent(UpdateEventType.InitAllPid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                _logger.LogError(ex.Message);
            }
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
