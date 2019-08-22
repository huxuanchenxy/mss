using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Core.Common;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
namespace MSS.API.Core.EventServer
{
    public class WarningJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly IWarnningRepo<EarlyWarnning> _warnSetting;
        private readonly GlobalDataManager _globalDataManager;
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;
        private readonly EventQueues _queues;
        private TimeSpan? _lastTime = null;
        public WarningJob(ILogger<WarningJob> logger, IWarnningRepo<EarlyWarnning> warnSetting,
            GlobalDataManager globalDataManager, IConfiguration config, IDistributedCache cache,
            EventQueues queues)
        {
            _logger = logger;
            _warnSetting = warnSetting;
            _globalDataManager = globalDataManager;
            _config = config;
            _cache = cache;
            _queues = queues;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            string server = _config.GetValue<string>("influxdb:server");
            string db = _config.GetValue<string>("influxdb:db");
            string query = "http://" + server + "/query?db=" + db;

            while (true)
            {
                try
                {
                    // 如果pid数据没有加载完毕，等待。。
                    if (_globalDataManager.AllPID.Count ==0)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    TimeSpan sTime;
                    TimeSpan eTime = DateTime.UtcNow - DateTime.Parse("1970-1-1");
                    // TimeSpan eTime = Convert.ToDateTime("2019-08-22 02:36:50") - DateTime.Parse("1970-1-1");

                    if (_lastTime == null)
                    {
                        sTime = eTime - TimeSpan.FromMinutes(5);
                    }
                    else
                    {
                        sTime = (TimeSpan)_lastTime;
                    }
                    _lastTime = eTime;
                    string sql = "&q=SELECT * FROM one_day.testlog where"
                        + " time > " + (long)sTime.TotalMilliseconds + "ms and"
                        + " time <= " + (long)eTime.TotalMilliseconds + "ms";
                    // string sql = "&q=SELECT * FROM one_day.testlog where"
                    //     + " time > " + DateTime.Now.AddHours(-11).Millisecond + "ms and"
                    //     + " time <= " + DateTime.Now.Millisecond + "ms";
                    // string sql = "&q=SELECT * FROM testlog where"
                    //     + " PID='r_s01_t1_a_0393' and time > " + 1560846493559 + "ms and"
                    //     + " time <= " + 1560866593559 + "ms";
                    IHttpClientHelper<InfluxdbResult> httpHelper = new HttpClientHelper<InfluxdbResult>();
                    InfluxdbResult result = await httpHelper.
                        GetSingleItemRequest(query + sql);
                    if (result.results[0].series != null)
                    {
                        int pidIdx = -1, valueIdx = -1, timeIdx = -1;
                        for (int i = 0; i < result.results[0].series[0].columns.Count; ++i)
                        {
                            string col = result.results[0].series[0].columns[i];
                            switch (col)
                            {
                                case "PID":
                                    pidIdx = i;
                                    break;
                                case "Value":
                                    valueIdx = i;
                                    break;
                                case "time":
                                    timeIdx = i;
                                    break;
                            }
                        }
                        if (pidIdx == -1 || valueIdx == -1)
                        {
                            _logger.LogError("不存在数据不正确，不存在pid或value字段");
                            continue;
                        }
                        for (int i = 0; i < result.results[0].series[0].values.Count; ++i)
                        {
                            List<object> row = result.results[0].series[0].values[i];
                            if (row[pidIdx] != null && row[valueIdx] != null)
                            {
                                string pid = row[pidIdx].ToString();
                                double value = Convert.ToDouble(row[valueIdx]);
                                _checkRules(pid, value);
                            }
                        }

                    }
                    else
                    {
                        // sleep
                        Thread.Sleep(2000);
                    }
                    
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
                    Thread.Sleep(30000);
                }
            }
        }

        private void _checkRules(string pid, double value)
        {
            PidTable pidInfo = null;
            if (_globalDataManager.AllPID.ContainsKey(pid))
            {
                pidInfo = _globalDataManager.AllPID[pid];
            }

            if (pidInfo != null)
            {
                int eqpType = pidInfo.EqpTypeID;
                string param = pidInfo.prop;
                double? up = pidInfo.UP;
                double? dw = pidInfo.DW;
                // 如果是扩展属性的值信息，则更新
                if (_globalDataManager.ExRulesValue.ContainsKey(pidInfo.pid))
                {
                    _globalDataManager.ExRulesValue[pidInfo.pid] = value;
                }

                // 判断是否符合规则
                List<EarlyWarnningSetting> rules = _globalDataManager.Rules.Where(c => 
                    c.EquipmentTypeID == eqpType && c.ParamID.Equals(param)).ToList();
                foreach (EarlyWarnningSetting setting in rules)
                {
                    List<EarlyWarnningSettingEx> exConditions = setting.SettingEx;
                    bool conform = true;
                    // 当前不符合的附加条件
                    EarlyWarnningSettingEx curConNotConform = null;
                    double curValue = 0;
                    foreach ( EarlyWarnningSettingEx excon in exConditions)
                    {
                        if (_globalDataManager.ExRulesValue.ContainsKey(excon.pid) &&
                            _globalDataManager.ExRulesValue[excon.pid] != null)
                        {
                            double exValue = (double)_globalDataManager.ExRulesValue[excon.pid];
                            switch (excon.ParamLimitType)
                            {
                                case 1: // 大于
                                    if (!(exValue > excon.ParamLimitValue))
                                    {
                                        conform = false;
                                    }
                                    break;
                                case 2: // 等于
                                    if (!(exValue == excon.ParamLimitValue))
                                    {
                                        conform = false;
                                    }
                                    break;
                                case 3: // 小于
                                    if (!(exValue < excon.ParamLimitValue))
                                    {
                                        conform = false;
                                    }
                                    break;
                            }
                            // 条件不符合 则跳出循环，不再判断其他附加条件
                            if (!conform)
                            {
                                curConNotConform = excon;
                                curValue = exValue;
                                break;
                            }
                        }
                        else
                        {
                            conform = false;
                        }
                    }

                    // 附加条件符合后 再判断是否在预警区间
                    if (conform)
                    {
                        bool needWarn = false;
                        string warnContent = "";
                        if (up != null)
                        {
                            double upLimit = (double)up * (1 + setting.ParamLimitUpper/100.0);
                            if (value > upLimit)
                            {
                                needWarn = true;
                                warnContent = pidInfo.Des+ "当前值为" + value  + ", 超过预警上限值（"+upLimit+"）";
                                
                            }
                        }
                        if (dw != null)
                        {
                            double dwLimit = (double)dw * (1 - setting.ParamLimitLower / 100.0);
                            if (value < dwLimit)
                            {
                                // 报警
                                needWarn = true;
                                warnContent = pidInfo.Des + "当前值为" + value + ", 低于预警下限值（" + dwLimit + "）";
                            }
                        }
                        // redis中查找看是否有预警存在
                        string prefix = RedisKeyPrefix.Warn;
                        string warnID = _cache.GetString(prefix + pid);
                        if (needWarn)
                        {
                            if (warnID == null) // redis中不存在记录则发送，存在的话不需要再次发送
                            {
                                // 考虑await?
                                _addWarning(pidInfo, value, warnContent);
                            }
                        }
                        else
                        {
                            // redis中存在预警 则取消预警
                            if (warnID != null)
                            {
                                _deleteWarning(Convert.ToInt32(warnID), prefix + pid, (int)pidInfo.EqpID);
                            }
                        }
                        
                    }
                    else
                    {
                        // 报警 附加条件不符合?
                    }
                }
            }
        }

        private async void _addWarning (PidTable pidInfo, double curValue, string content)
        {
            EarlyWarnning warn = new EarlyWarnning();
            warn.Pid = pidInfo.pid;
            warn.CurValue = curValue;
            warn.EqpID = (int)pidInfo.EqpID;
            warn.Content = content;
            warn.Status = 0;
            warn.CreatedTime = DateTime.Now;
            EarlyWarnning ret = await _warnSetting.SaveWarnning(warn);
            // 插入redis
            string prefix = RedisKeyPrefix.Warn;
            _cache.SetString(prefix + warn.Pid, ret.ID.ToString());

            // 发送预警
            _sendWarn(warn.EqpID, "on");
        }
        private async void _deleteWarning(int id, string redisKey, int eqpID)
        {
            EarlyWarnning warn = new EarlyWarnning();
            warn.ID = id;
            warn.Status = 1;
            warn.UpdatedTime = DateTime.Now;
            EarlyWarnning ret = await _warnSetting.UpdateWarnning(warn);
            // 删除redis
            _cache.Remove(redisKey);
            // 发送预警恢复
            _sendWarn(eqpID, "off");
        }

        private void _sendWarn(int eqpID, string content)
        {
            MssEventMsg msg = new MssEventMsg();
            msg.msg = content;
            msg.type = MssEventType.Warnning;
            msg.eqp = new Equipment()
            {
                ID = eqpID
            };
            _queues.AlarmQueue.Enqueue(msg);
        }
    }
}