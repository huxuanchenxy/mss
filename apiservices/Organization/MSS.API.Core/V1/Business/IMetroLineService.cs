using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
namespace MSS.API.Core.V1.Business
{
    public interface IMetroLineService
    {
        Task<ApiResult> SaveLine(MetroLine line);

        Task<ApiResult> UpdateLine(MetroLine line);

        Task<ApiResult> DeleteLine(List<MetroLine> lines);

        // 根据ID查询
        Task<ApiResult> GetMetroLineByID(int id);

        // 分页查询
        Task<ApiResult> ListMetroLineByPage(MetroLineParam param);

        // 所有地铁线
        Task<ApiResult> ListAllMetroLine();
        Task<ApiResult> GetMetroStation();
    }
}
