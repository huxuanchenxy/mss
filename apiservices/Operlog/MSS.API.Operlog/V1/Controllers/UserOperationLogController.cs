using Microsoft.AspNetCore.Mvc;
using MSS.API.Operlog.Model.DTO;
using MSS.API.Operlog.V1.Business;
using static MSS.API.Operlog.Model.Const;

namespace MSS.API.Operlog.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserOperationLogController : ControllerBase
    {
        private readonly IUserOperationLogService _userOperationLogService;
        public UserOperationLogController(IUserOperationLogService userOperationLogService)
        {
            _userOperationLogService = userOperationLogService;

        }
        [HttpGet("QueryList")]
        public ActionResult GetPageByParm([FromQuery] UserOperationLogParm parm)
        {
            //ActionQueryParm parm = new ActionQueryParm();

            var ret = _userOperationLogService.GetPageByParm(parm).Result;
            if (ret.code==(int)ErrType.OK)
            {
                var data = new { rows = ret.data, total = ret.relatedData };
                var resp = new { code = ret.code, data = data };
                return Ok(resp);
            }
            else
            {
                var resp = new { code = ret.code, msg = ret.msg };
                return Ok(resp);
            }
        }

        [HttpPost("Add")]
        public ActionResult Add()
        {
            return Ok("ok");
        }
        
    }
}