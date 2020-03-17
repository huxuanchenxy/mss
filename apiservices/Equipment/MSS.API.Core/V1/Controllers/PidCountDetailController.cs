using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using System.Threading.Tasks;

// Coded By admin 2020/2/19 10:01:42
namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PidCountDetailController : ControllerBase
    {
        private readonly IPidCountDetailService _service;

        public PidCountDetailController(IPidCountDetailService service)
        {
            _service = service;
        }

        [HttpGet("GetPageList")]
        public async Task<ActionResult<ApiResult>> GetPageList([FromQuery] PidCountDetailParm parm)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.GetPageList(parm);

            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "获取分页数据PidCountDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpPut]
        public async Task<ActionResult<ApiResult>> Update(PidCountDetail obj)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.Update(obj);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "更新数据PidCountDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpDelete("{ids}")]
        public async Task<ActionResult<ApiResult>> Delete(string ids)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.Delete(ids);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "删除数据PidCountDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult>> Save(PidCountDetail obj)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.Save(obj);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "新增数据PidCountDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult>> GetByID(int id)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.GetByID(id);

            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "获取单个数据PidCountDetail失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }
    }
}



