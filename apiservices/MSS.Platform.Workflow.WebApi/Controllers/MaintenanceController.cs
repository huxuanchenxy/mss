using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Common;
using MSS.Platform.Workflow.WebApi.Model;
using MSS.Platform.Workflow.WebApi.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

// Coded By admin 2019/9/27 9:54:54
namespace MSS.Platform.Workflow.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _service;

        public MaintenanceController(IMaintenanceService service)
        {
            _service = service;
        }

        #region MaintenanceItem
        [HttpPost("SaveMItem")]
        public ActionResult SaveMItem(MaintenanceItem maintenanceItem)
        {
            var ret = _service.SaveMItem(maintenanceItem);
            return Ok(ret.Result);
        }
        #endregion

        #region MaintenanceModuleItem
        [HttpPost("SaveMMoudleItem")]
        public ActionResult SaveMMoudleItem(List<MaintenanceModuleItem> maintenanceModuleItem)
        {
            var ret = _service.SaveMMoudleItem(maintenanceModuleItem);
            return Ok(ret.Result);
        }
        #endregion

        #region MaintenanceModuleItemValue
        [HttpPost("SaveMMoudleItemValue")]
        public ActionResult SaveMMoudleItemValue(List<MaintenanceModuleItemValue> maintenanceModuleItemValue)
        {
            var ret = _service.SaveMMoudleItemValue(maintenanceModuleItemValue);
            return Ok(ret.Result);
        }
        #endregion

        #region MaintenanceModule
        [HttpPost("SaveMModule")]
        public ActionResult SaveMModule(MaintenanceModule maintenanceModule)
        {
            var ret = _service.SaveMModule(maintenanceModule);
            return Ok(ret.Result);
        }
        #endregion

    }
}



