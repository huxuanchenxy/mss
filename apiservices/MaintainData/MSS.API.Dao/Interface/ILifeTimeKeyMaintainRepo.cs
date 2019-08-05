using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface ILifeTimeKeyMaintainRepo<T> where T : BaseEntity
    {
        Task<List<LifeTimeKeyMaintainInfo>> GetListByPage(string strWhere, string sort, string orderby, int page, int size);
        Task<List<LocatioInfo>> GetdeviceList(int level ,int location, string strWhere);
        Task<List<LocationDeviceInfo>> GetlocationList();
    }
}
