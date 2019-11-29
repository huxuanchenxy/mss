using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSS.API.Core.V1.Business;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using MSS.API.Common;

namespace MSS.API.Core.V1.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StorageLocationController : ControllerBase
    {
        private readonly IStorageLocationService _storageLocationService;
        public StorageLocationController(IStorageLocationService storageLocationService)
        {
            _storageLocationService = storageLocationService;

        }

        [HttpPost]
        public ActionResult Save(StorageLocation storageLocation)
        {
            var ret = _storageLocationService.Save(storageLocation);
            return Ok(ret.Result);
        }

        [HttpPut]
        public ActionResult Update(StorageLocation storageLocation)
        {
            var ret = _storageLocationService.Update(storageLocation);
            return Ok(ret.Result);
        }

        [HttpDelete("{ids}")]
        public ActionResult Delete(string ids)
        {
            var resp = _storageLocationService.Delete(ids);
            return Ok(resp.Result);
        }

        [HttpGet]
        public ActionResult GetPageByParm([FromQuery] StorageLocationQueryParm parm)
        {
            var resp = _storageLocationService.GetPageByParm(parm);
            return Ok(resp.Result);
        }

        [HttpGet("{id}")]
        public ActionResult GetByID(int id)
        {
            var resp = _storageLocationService.GetByID(id);
            return Ok(resp.Result);
        }

        [HttpGet("All")]
        public ActionResult GetAll()
        {
            var resp = _storageLocationService.GetAll();
            return Ok(resp.Result);
        }

        [HttpGet("ListByWarehouse/{warehouse}")]
        public ActionResult ListByWarehouse(int warehouse)
        {
            var resp = _storageLocationService.ListByWarehouse(warehouse);
            return Ok(resp.Result);
        }

        [HttpGet("ListWarehyouseLocation")]
        public ActionResult ListWarehyouseLocation()
        {
            var resp = _storageLocationService.ListWarehyouseLocation();
            return Ok(resp.Result);
        }
    }
}