using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using Dapper;
using MSS.API.Model.DTO;
using System.Linq;
using static MSS.API.Utility.Const;
using MSS.API.Utility;

namespace MSS.API.Dao.Implement
{
    public class UserRepo : BaseRepo, IUserRepo<User>
    {
        public UserRepo(DapperOptions options) : base(options) { }

        public async Task<MSSResult<UserView>> GetPageByParm(UserQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                MSSResult<UserView> mRet = new MSSResult<UserView>();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as created_name,u2.user_name as updated_name,r.role_name ")
                .Append(" FROM User a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ")
                .Append(" left join role r on a.role_id=r.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no + " and a.is_super=" + (int)IsSuper.no);
                if (!string.IsNullOrWhiteSpace(parm.searchName))
                {
                    whereSql.Append(" and a.user_name like '%" + parm.searchName + "%' ");
                }
                if (parm.searchRole != null)
                {
                    whereSql.Append(" and a.role_id=" + parm.searchRole);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var tmp = await c.QueryAsync<UserView>(sql.ToString());
                if (tmp!=null)
                {
                    mRet.data = tmp.ToList();
                }
                mRet.relatedData = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from user a " + whereSql.ToString());
                return mRet;
            });
        }
        public async Task<User> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<User>(
                    " SELECT * FROM User WHERE id = @id and is_del=" + (int)IsDeleted.no, new { id = id });
                return result;
            });
        }

        public async Task<User> GetByAcc(string acc)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<UserView>(
                    " SELECT * FROM User WHERE acc_name = @acc and is_del="+IsDeleted.no, new { acc = acc });
                return result;
            });
        }

        public async Task<int> GetUserCountByRole(string[] ids)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<int>(
                    "SELECT count(*) FROM User WHERE role_id in @ids", new { ids = ids });
                return result;
            });
        }

        public async Task<int> Add(User user)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" insert into User " +
                    " values(0,@acc_name,@user_name,@password,@random_num,@job_number,@position, " +
                    " @id_card,@birth,@sex,@mobile,@email,@id_photo,@role_id, " +
                    " @created_time,@created_by,@updated_time,@updated_by,@is_del,@is_super) ", user);
                return result;
            });
        }

        public async Task<int> Update(User user)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update User " +
                    " set acc_name=@acc_name,user_name=@user_name,job_number=@job_number,position=@position,id_card=@id_card, " +
                    " birth=@birth,sex=@sex,mobile=@mobile,email=@email,id_photo=@id_photo,role_id=@role_id, " +
                    " updated_time=@updated_time,updated_by=@updated_by where id=@id", user);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update User set is_del="+(int)IsDeleted.yes+
                    ",updated_time=@updated_time,updated_by=@updated_by" +
                    " WHERE id in @ids ", new { ids = ids, updated_time=DateTime.Now, updated_by=userID });
                return result;
            });
        }

        public async Task<List<User>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<User>(
                    "select * from user where is_del="+(int)IsDeleted.no+" and is_super="+(int)IsSuper.no)).ToList();
                return result;
            });
        }

        public async Task<List<User>> GetAllContainSuper()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<User>(
                    "select * from user where is_del=" + (int)IsDeleted.no)).ToList();
                return result;
            });
        }


        public async Task<int> ChangePwd(User user)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update User set password=@password," +
                    " random_num=@random_num,updated_time=@updated_time,updated_by=@updated_by where id=@id", user);
                return result;
            });
        }

        public async Task<int> ResetPwd(string[] ids,int userID)
        {
            return await WithConnection(async c =>
            {
                Encrypt en = new Encrypt();
                int r = new Random().Next(1, PWD_RANDOM_MAX);
                string pwd = en.DoEncrypt(INIT_PASSWORD,r);
                var result = await c.ExecuteAsync(" update User " +
                    " set password=@password,updated_time=@updated_time,updated_by=@updated_by where id in @ids", 
                    new { password=pwd, updated_time=DateTime.Now, updated_by =userID,ids=ids});
                return result;
            });
        }
    }
}
