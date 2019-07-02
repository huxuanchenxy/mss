using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.API.DAO.Dapper;
using System.API.DAO.Interface;
using System.API.Model;
using System.Text;

namespace System.API.DAO.Implement
{
   public class ConfigMidAreaRepo : BaseRepo, IConfigMidAreaRepo<TB_Config_MidArea>
    {
        public ConfigMidAreaRepo(DapperOptions options) : base(options) { }

        #region  成员方法

        #endregion  成员方法
        #region  MethodEx

        #endregion  MethodEx
        public int Add(TB_Config_MidArea model)
        {
            var ret = DbContext.Execute("insert into TB_Config_MidArea (AreaName,PID,sort,Is_Used,Is_Deleted,Created_Time,Created_By,Updated_Time,Updated_By,Remark)" +
                 " values(@AreaName,@PID,@sort,@Is_Used,@Is_Deleted,@Created_Time,@Created_By,@Updated_Time,@Updated_By,@Remark)", model);
            return ret;
        }

        public bool Delete(int id)
        {
            var ret = DbContext.Execute(" delete from TB_Config_MidArea where id="+id);
            if (ret > 0)
            {
                return true;
            }
            return false;
        }

        public bool DeleteList(string idlist)
        {
            var ret = DbContext.Execute(" delete from TB_Config_MidArea where id in (@WF_TemIDlist)", idlist);
            if (ret > 0)
            {
                return true;
            }
            return false;
        }

        public bool Exists(int id)
        {
            throw new NotImplementedException();
        }

        public List<TB_Config_MidArea> GetList(string strWhere)
        {
            var WF_TemplateList = DbContext.Query<TB_Config_MidArea>(" select * from TB_Config_MidArea where " + strWhere);
            List<TB_Config_MidArea> list = new List<TB_Config_MidArea>();
            list.AddRange(WF_TemplateList);
            return list;
        }

        public List<TB_Config_MidArea> GetList(int Top, string strWhere, string filedOrder)
        {
            var WF_TemplateList = DbContext.Query<TB_Config_MidArea>(" select * from TB_Config_MidArea where " + strWhere);
            List<TB_Config_MidArea> list = new List<TB_Config_MidArea>();
            list.AddRange(WF_TemplateList);
            return list;
        }

        /// <summary>
        /// 获取最下层 :比如:马当路：机房1、机房2
        /// </summary>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public List<TB_Config_MidArea> GetListByPid(int Pid)
        {
            var WF_TemplateList = DbContext.Query<TB_Config_MidArea>(" select * from TB_Config_MidArea where PID='" + Pid + "'");
            List<TB_Config_MidArea> list = new List<TB_Config_MidArea>();
            list.AddRange(WF_TemplateList);
            return list;
        }

        public List<TB_Config_MidArea> GetListByPage(string strWhere, string sort, string orderby, int page, int size)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM TB_Config_MidArea where " + strWhere);
            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(orderby))
            {
                sql.Append(" order by " + sort + " " + orderby);
            }
            sql.Append(" limit " + (page - 1) * size + "," + size);
            List<TB_Config_MidArea> list = new List<TB_Config_MidArea>();
            var MidArealist = DbContext.Query<TB_Config_MidArea>(sql.ToString()); 
            list.AddRange(MidArealist);
            return list; 
        }

        public int GetMaxId()
        {
            throw new NotImplementedException();
        }

        public TB_Config_MidArea GetModel(int id)
        {
            var ret = DbContext.QueryFirstOrDefault<TB_Config_MidArea>(" select * from TB_Config_MidArea where id=@id)", id);
            return ret;
        }

        public int GetRecordCount(string strWhere)
        {
            throw new NotImplementedException();
        }

        public bool Update(TB_Config_MidArea model)
        {
            var ret = DbContext.Execute("update TB_Config_MidArea set  AreaName=@AreaName,PID=@PID,Updated_Time=@Updated_Time,Updated_By=@Updated_By,Remark=@Remark where id=@id", model);
            if (ret > 0)
            {
                return true;
            }
            return false;
        }
    }
}

