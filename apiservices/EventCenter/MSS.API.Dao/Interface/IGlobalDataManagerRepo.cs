using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.DTO;

namespace MSS.API.Dao.Interface
{
    public interface IGlobalDataManagerRepo
    {
        Task<List<StatisticsDimension>> ListEqpDimension();
        Task<List<AllArea>> GetAllArea();

        // 插入StatisticsDimension表
        Task<int> SaveStatisticsDimension(List<StatisticsDimension> data);
        // 删除StatisticsDimension
        Task<List<StatisticsDimension>> ListStatisticsDimension();
    }
}
