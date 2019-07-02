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
  public  class ConfigBigAreaRepo : BaseRepo, IConfigBigAreaRepo<TB_Config_BigArea>
    {
        public ConfigBigAreaRepo(DapperOptions options) : base(options) { }

        #region  成员方法

        #endregion  成员方法
        #region  MethodEx

        #endregion  MethodEx
        public int Add(TB_Config_BigArea model)
        {
            var ret = DbContext.Execute("insert into TB_Config_BigArea (AreaName,ConfigType,sort,Is_Used,Is_Deleted,Created_Time,Created_By,Updated_Time,Updated_By,Remark)" +
                 " values(@AreaName,@ConfigType,@sort,@Is_Used,@Is_Deleted,@Created_Time,@Created_By,@Updated_Time,@Updated_By,@Remark)", model);
            return ret;
        }

        public bool Delete(int id)
        {
            var ret = DbContext.Execute(" delete from TB_Config_BigArea where id="+id);
            if (ret > 0)
            {
                return true;
            }
            return false;
        }

        public bool DeleteList(string IDlist)
        {
            var ret = DbContext.Execute(" delete from TB_Config_BigArea where id in (@IDlist)", IDlist);
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

        public List<TB_Config_BigArea> GetList(string strWhere)
        {
            var WF_TemplateList = DbContext.Query<TB_Config_BigArea>(" select * from TB_Config_BigArea where " + strWhere);
            List<TB_Config_BigArea> list = new List<TB_Config_BigArea>();
            list.AddRange(WF_TemplateList);
            return list;
        }

        public List<TB_Config_BigArea> GetList(int Top, string strWhere, string filedOrder)
        {
            var WF_TemplateList = DbContext.Query<TB_Config_BigArea>(" select * from TB_Config_BigArea where " + strWhere);
            List<TB_Config_BigArea> list = new List<TB_Config_BigArea>();
            list.AddRange(WF_TemplateList);
            return list;
        }

        /// <summary>
        /// 获取场区类型获取子类:比如:车站：马当路、新天地、龙阳路
        /// </summary>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public List<TB_Config_BigArea> GetListByConfigType(string filedOrder)
        {
            var WF_TemplateList = DbContext.Query<TB_Config_BigArea>(" select * from TB_Config_BigArea where ConfigType='"+filedOrder+"'");
            List <TB_Config_BigArea> list = new List<TB_Config_BigArea>();
            list.AddRange(WF_TemplateList);
            return list;
        }

      

        public List<TB_Config_BigArea> GetListByPage(string strWhere, string sort, string orderby, int page, int size)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM TB_Config_BigArea where " + strWhere);
            if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(orderby))
            {
                sql.Append(" order by " + sort + " " + orderby);
            }
            sql.Append(" limit " + (page - 1) * size + "," + size);
            List<TB_Config_BigArea> list = new List<TB_Config_BigArea>();
             var BigArealist = DbContext.Query<TB_Config_BigArea>(sql.ToString());
            list.AddRange(BigArealist);
            return list;
        }

        public int GetMaxId()
        {
            throw new NotImplementedException();
        }

        public TB_Config_BigArea GetModel(int id)
        {
            var ret = DbContext.QueryFirstOrDefault<TB_Config_BigArea>(" select * from TB_Config_BigArea where id="+id);
            return ret;
        }



        public int GetRecordCount(string strWhere)
        {
            throw new NotImplementedException();
        }

        public bool Update(TB_Config_BigArea model)
        {
            var ret = DbContext.Execute("update TB_Config_BigArea set  AreaName=@AreaName,ConfigType=@ConfigType,Updated_Time=@Updated_Time,Updated_By=@Updated_By,Remark=@Remark where id=@id", model);
            if (ret > 0)
            {
                return true;
            }
            return false;
        }
    }
}
