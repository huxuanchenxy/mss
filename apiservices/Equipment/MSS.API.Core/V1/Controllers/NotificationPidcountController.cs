using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using System.Threading.Tasks;

// Coded By admin 2020/2/20 14:58:52
namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationPidcountController : ControllerBase
    {
        private readonly INotificationPidcountService _service;

        public NotificationPidcountController(INotificationPidcountService service)
        {
            _service = service;
        }

        [HttpGet("GetPageList")]
        public async Task<ActionResult<ApiResult>> GetPageList([FromQuery] NotificationPidcountParm parm)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.GetPageList(parm);

            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "获取分页数据NotificationPidcount失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpPut]
        public async Task<ActionResult<ApiResult>> UpdateStatus(NotificationPidcount obj)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.UpdateStatus(obj);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "更新数据NotificationPidcount失败, 异常信息:{0}",
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
                    "删除数据NotificationPidcount失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResult>> Save(NotificationPidcount obj)
        {
            ApiResult ret = new ApiResult { code = Code.Failure };
            try
            {
                ret = await _service.Save(obj);
            }
            catch (System.Exception ex)
            {
                ret.msg = string.Format(
                    "新增数据NotificationPidcount失败, 异常信息:{0}",
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
                    "获取单个数据NotificationPidcount失败, 异常信息:{0}",
                    ex.Message);
            }
            return ret;
        }
    }
}



