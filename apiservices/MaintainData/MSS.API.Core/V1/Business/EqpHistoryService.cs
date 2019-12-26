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
    public class EqpHistoryService : IEqpHistoryService
    {
        private readonly IEqpHistoryRepo<EqpHistory> _eqpHistoryRepo;

        public EqpHistoryService(IEqpHistoryRepo<EqpHistory> eqpHistoryRepo)
        {
            _eqpHistoryRepo = eqpHistoryRepo;
        }


        public async Task<ApiResult> ListByEqp(int id,bool isHide)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<EqpHistory> ehs = await _eqpHistoryRepo.ListByEqp(id, isHide);
                if (ehs != null && ehs.Count() > 0)
                {
                    ret.data = ListToTimeLine(ehs);
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> ListByType(string types)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _eqpHistoryRepo.ListByType(types.Split(','));
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        private List<object> ListToTimeLine(List<EqpHistory> ehs)
        {
            List<object> objs = new List<object>();
            IEnumerable<IGrouping<string, EqpHistory>> groups = ehs.GroupBy(a => a.CreatedDate);
            foreach (IGrouping<string, EqpHistory> group in groups)
            {
                IEnumerable<IGrouping<int, EqpHistory>> groupTypes = group.GroupBy(a => a.Type);
                // 默认显示时间轴节点和日期
                int stage = 2;
                string tag = group.Key;
                // 相同日期
                int i = 0;
                foreach (var groupType in groupTypes)
                {
                    if ((int)MyDictionary.EqpHistoryType.Expiration == groupType.Key)
                    {
                        stage = 3;
                    }
                    if (i != 0)
                    {
                        // 相同日期不同类型中，如果不是第一个则不显示节点和日期
                        stage = 0;
                        tag = "";
                    }
                    List<object> children = new List<object>();
                    string content = "";
                    // 相同类型
                    foreach (EqpHistory e in groupType)
                    {
                        children.Add(new { id = e.WorkingOrder,name=e.ShowName });
                        content = e.TypeName;
                    }
                    int detailType = 1;
                    switch (groupType.Key)
                    {
                        case (int)MyDictionary.EqpHistoryType.TroublePM:
                            detailType = 2;
                            break;
                        case (int)MyDictionary.EqpHistoryType.Maintenance:
                            detailType = 3;
                            break;
                        case (int)MyDictionary.EqpHistoryType.Install:
                        case (int)MyDictionary.EqpHistoryType.Expiration:
                        case (int)MyDictionary.EqpHistoryType.FirstWork:
                        case (int)MyDictionary.EqpHistoryType.MajorPM:
                        case (int)MyDictionary.EqpHistoryType.MediumPM:
                        case (int)MyDictionary.EqpHistoryType.SecondWork:
                            detailType = 1;
                            break;
                    }
                    objs.Add(new
                    {
                        detailType,
                        tag,
                        stage,
                        content,
                        children
                    });
                    i++;
                }
            }
            return objs;
        }
    }
}
