using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using MSS.API.Model.DTO;

namespace MSS.API.Dao.Implement
{
    public class OrgRepo : BaseRepo, IOrgRepo<OrgTree>
    {
        public OrgRepo(DapperOptions options) : base(options) { }

        public async Task<List<OrgTree>> ListAllOrgNode()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM org_tree";

                var list = await c.QueryAsync<OrgTree>(sql);
                return list.ToList();
            });
        }

        public async Task<List<OrgUser>> ListAllOrgUser()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT a.*, b.user_name FROM org_user AS a"
                    + " INNER JOIN user AS b on a.user_id=b.id WHERE a.is_del != 1";
                List<OrgUser> orguser = (await c.QueryAsync<OrgUser>(sql)).ToList();
                return orguser;
            });
        }

        public async Task<List<OrgUser>> ListOrgNodeUsers(int id)
        {
            return await WithConnection(async c =>
            {
                // string sql = "SELECT * FROM org_user WHERE org_node_id = @ID and is_del != 1";
                string sql = "SELECT a.*, b.user_name, b.email, b.mobile FROM org_user a"
                    + " INNER JOIN user AS b on a.user_id=b.id "
                    + " WHERE a.org_node_id = @ID and a.is_del != 1";
                var prop = await c.QueryAsync<OrgUser>(sql,
                new
                {
                    ID = id
                });
                return prop.ToList();
            });
        }
    }
}
