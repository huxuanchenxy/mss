﻿using System;
using System.Collections.Generic;
using System.Text;

using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public abstract class BaseEntity
    {
        public int ID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public int Is_deleted { get; set;}
        public int IsDel { get; set; }
    }

    public class BaseEntityMap : EntityMap<BaseEntity>
    {
        public BaseEntityMap()
        {
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.Is_deleted).ToColumn("Is_deleted");
        }
    }

    public abstract class BaseQueryParm
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 每页显示行数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string sort { get; set; }
        /// <summary>
        /// asc/desc:顺序/降序
        /// </summary>
        public string order { get; set; }
    }

    public class QueryItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int LocationBy { get; set; }
    }
    public enum TroubleStatus
    {
        NewTrouble=48,
        Processing=49,
        Repaired=50,
        PendingApproval = 89,
        Delayed=133,
        Finished=134,
        Canceled=135
    }

    public enum TroubleOperation
    {
        NewTrouble = 141,
        CancelTrouble = 142,
        Assign = 143,
        Delayed = 144,
        Repost = 145,
        RepairReject = 146,
        Pass = 147,
        Unpass = 148,
        AssignReject = 149,
        UpdateTrouble=150,
        Dealed=173,
        PrePass=174,
        UnPrePass=175
    }

    /// <summary>
    /// 故障界面对应显示的内容，内部使用
    /// </summary>
    public enum TroubleView
    {
        MyRepair = 1,
        MyProcessing = 2,
        MyPreCheck = 3,
        MyCheck = 4
    }
    /// <summary>
    /// 员工考勤依据所需的状态，目前只做接口
    /// </summary>
    public enum AttandenceStatus
    {
        UnReported = 152,
        UnDeal = 153,
        UnRepaired = 154,
        UnFinished = 155,
        UnRepaired72 = 156
    }

    public static class Dictionary
    {
        public const int TROUBLELEVEL = 126;
    }
}
