using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.API.Core.V1.Business
{
    static class UploadFileHelper
    {
        /// <summary>
        /// 根据数据库查询结果获取对应的菜单树结构
        /// </summary>
        /// <param name="laa">数据库查询权限结果</param>
        /// <returns>前端所需要的菜单数据结构列表</returns>
        public static List<object> MyList(List<UploadFile> ufs)
        {
            List<object> objs = new List<object>();
            IEnumerable<IGrouping<int, UploadFile>> groupAction = ufs.GroupBy(a => a.Type);
            foreach (IGrouping<int, UploadFile> group in groupAction)
            {
                List<object> tmp = new List<object>();
                int type=0;
                string typeName="";
                foreach (UploadFile item in group)
                {
                    type = item.Type;
                    typeName = item.TypeName;
                    tmp.Add(new {
                        type = item.Type,
                        typeName = item.TypeName,
                        id = item.ID,
                        name = item.FileName,
                        url = item.FilePath,
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
    }
}
