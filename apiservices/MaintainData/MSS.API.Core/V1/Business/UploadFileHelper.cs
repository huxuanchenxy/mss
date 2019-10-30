using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using MSS.Common.Consul;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MSS.API.Common.MyDictionary;

namespace MSS.API.Core.V1.Business
{
    static class UploadFileHelper
    {
        /// <summary>
        /// 根据数据库查询结果获取对应的上传显示结构
        /// </summary>
        /// <param name="laa">数据库查询权限结果</param>
        /// <returns>前端所需要的上传显示结构</returns>
        public static List<object> ListShow(List<UploadFile> ufs)
        {
            List<object> objs = new List<object>();
            IEnumerable<IGrouping<int, UploadFile>> groupAction = ufs.GroupBy(a => a.type);
            foreach (IGrouping<int, UploadFile> group in groupAction)
            {
                List<object> tmp = new List<object>();
                int type=0;
                string typeName="";
                foreach (UploadFile item in group)
                {
                    type = item.type;
                    typeName = item.TypeName;
                    tmp.Add(new {
                        type = item.type,
                        typeName = item.TypeName,
                        id = item.ID,
                        name = item.file_name,
                        url = item.file_path,
                        status = "success"
                    });
                }
                if (type!=0)
                {
                    objs.Add(new
                    {
                        type = type,
                        typeName = typeName,
                        list = tmp
                    });
                }
            }
            return objs;
        }

        /// <summary>
        /// 根据数据库查询结果获取对应的上传级联显示结构
        /// </summary>
        /// <param name="laa">数据库查询权限结果</param>
        /// <returns>前端所需要的上传级联显示结构</returns>
        public static List<object> CascaderShow(List<UploadFile> ufs)
        {
            List<object> objs = new List<object>();
            IEnumerable<IGrouping<int, UploadFile>> groupAction = ufs.GroupBy(a => a.type);
            foreach (IGrouping<int, UploadFile> group in groupAction)
            {
                List<object> tmp = new List<object>();
                int type = 0;
                string typeName = "";
                foreach (UploadFile item in group)
                {
                    type = item.type;
                    typeName = item.TypeName;
                    tmp.Add(new
                    {
                        type = item.type,
                        typeName = item.TypeName,
                        value = item.ID,
                        label = item.file_name,
                        url = item.file_path
                    });
                }
                if (type != 0)
                {
                    objs.Add(new
                    {
                        value = type,
                        label = typeName,
                        children = tmp
                    });
                }
            }
            return objs;
        }

        public static async Task<List<UploadFilesEntity>> GetUploadFile(string ids, UploadShowType ust,
            IServiceDiscoveryProvider _consulServiceProvider, SystemResource systemResource)
        {
            var _services = await _consulServiceProvider.GetServiceAsync("EqpService");
            IHttpClientHelper<ApiResult> h = new HttpClientHelper<ApiResult>();
            //string ids = String.Join(",", etv.rows.Select(a => a.ID));
            string url = "/api/v1/Upload/ListByEntity/" + ids + "/" + (int)systemResource + "/" + (int)ust;
            ApiResult r = await h.GetSingleItemRequest(_services + url);
            return JsonConvert.DeserializeObject<List<UploadFilesEntity>>(r.data.ToString());
        }
    }

    public class UploadFilesEntity
    {
        public int Entity { get; set; }
        public string UploadFiles { get; set; }
    }

}
