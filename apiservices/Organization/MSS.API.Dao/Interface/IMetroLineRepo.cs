using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using MSS.API.Dao.Common;
namespace MSS.API.Dao.Interface
{
    public interface IMetroLineRepo<T> where T:BaseEntity
    {
        // 保存线路
        Task<MetroLine> SaveLine(MetroLine line);

        Task<bool> CheckExist(MetroLine line);

        Task<MetroLine> UpdateLine(MetroLine line);

        Task<int> DeleteLine(List<MetroLine> line);

        // 根据id查询
        Task<MetroLine> GetMetroLineByID(int id);

        // 分页查询
        Task<PageData<MetroLine>> ListMetroLineByPage(MetroLineParam param);

        // 所有地铁线
        Task<List<MetroLine>> ListAllMetroLine();
    }
}
