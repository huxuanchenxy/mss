using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


// Coded By admin 2019/11/29 17:49:11
namespace MSS.API.Core.V1.Business
{


    public class StockOperationDetailService : IStockOperationDetailService
    {
        private readonly IStockOperationDetailRepo<StockOperationDetail> _repo;
        private readonly IAuthHelper _authhelper;
        private readonly int _userID;

        public StockOperationDetailService(IStockOperationDetailRepo<StockOperationDetail> repo, IAuthHelper authhelper)
        {
            _repo = repo;
            _authhelper = authhelper;
            _userID = _authhelper.GetUserId();
        }

        public async Task<ApiResult> GetStockOperationChart(StockOperationDetailParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.StockOperationType = 68;//物资接收
                parm.SparePartsType = 60;//耗材
                var data = await _repo.GetByParm(parm);
                var data1 = await _repo.GetSpareParts(parm.legendData);
                List<string> legendData = new List<string>();
                foreach (var leg in data1)
                {
                    if (!legendData.Contains(leg.name))
                    {
                        legendData.Add(leg.name);
                    }
                }
                if (parm.dateType == 1)//按日统计
                {
                    List<string> xAxisData = new List<string>();
                    foreach (var d in data)
                    {
                        var curdate = d.CreatedTime.ToString("yyyy-MM-dd");
                        if (!xAxisData.Contains(curdate))
                        {
                            xAxisData.Add(curdate);
                        }
                    }

                    List<dynamic> seariesdata = new List<dynamic>();
                    foreach (var d in data1)
                    {
                        List<int> curdata = new List<int>();
                        foreach (var x in xAxisData)
                        {
                            var currows = data.Where(c => c.SparePartsId == d.id && c.CreatedTime.ToString("yyyy-MM-dd") == x);
                            int curCount = 0;
                            foreach (var c in currows)
                            {
                                curCount += c.CountNo;
                            }
                            curdata.Add(curCount);
                        }
                        seariesdata.Add(new { name = d.name, data = curdata, type = "bar" });
                    }
                    dynamic retdata = new { legendData = legendData, xAxisData = xAxisData, seariesData = seariesdata };
                    ret.data = retdata;
                }
                if (parm.dateType == 2)//按月统计
                {
                    List<string> xAxisData = new List<string>();
                    foreach (var d in data)
                    {
                        var curdate = d.CreatedTime.ToString("yyyy-MM");
                        if (!xAxisData.Contains(curdate))
                        {
                            xAxisData.Add(curdate);
                        }
                    }
                    List<dynamic> seariesdata = new List<dynamic>();
                    foreach (var d in data1)
                    {
                        
                        List<int> curdata = new List<int>();
                        foreach (var x in xAxisData)
                        {
                            var currows = data.Where(c => c.SparePartsId == d.id && c.CreatedTime.ToString("yyyy-MM") == x);
                            int curCount = 0;
                            foreach (var c in currows)
                            {
                                curCount += c.CountNo;
                            }
                            curdata.Add(curCount);
                        }
                        seariesdata.Add(new { name = d.name, data = curdata, type = "bar" });
                    }
                    dynamic retdata = new { legendData = legendData, xAxisData = xAxisData, seariesData = seariesdata };
                    ret.data = retdata;
                }


                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> Save(StockOperationDetail obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                obj.UpdatedTime = dt;
                obj.CreatedTime = dt;
                obj.UpdatedBy = _userID;
                obj.CreatedBy = _userID;
                ret.data = await _repo.Save(obj);
                ret.code = Code.Success;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(StockOperationDetail obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                StockOperationDetail et = await _repo.GetByID(obj.Id);
                if (et != null)
                {
                    DateTime dt = DateTime.Now;
                    obj.UpdatedTime = dt;
                    obj.UpdatedBy = _userID;
                    ret.data = await _repo.Update(obj);
                    ret.code = Code.Success;
                }
                else
                {
                    ret.code = Code.DataIsnotExist;
                    ret.msg = "所要修改的数据不存在";
                }
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Delete(string ids)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _repo.Delete(ids.Split(','), _userID);
                ret.code = Code.Success;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                StockOperationDetail obj = await _repo.GetByID(id);
                ret.data = obj;
                ret.code = Code.Success;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
    }
}



