
using Dapper.FluentMap.Mapping;
using MSS.Platform.Workflow.WebApi.Data;
using System;
using System.Collections.Generic;
using System.Data;

// Coded by admin 2019/9/26 16:46:46
namespace MSS.Platform.Workflow.WebApi.Model
{
    #region MaintenanceItem
    public class MaintenanceItem
    {
        public int ID { get; set; }
        public string ItemName { get; set; }
        public int ItemType { get; set; }
    }
    public class MaintenanceItemMap : EntityMap<MaintenanceItem>
    {
        public MaintenanceItemMap()
        {
            Map(o => o.ItemName).ToColumn("item_name");
            Map(o => o.ItemType).ToColumn("item_type");
        }

    }
    #endregion

    #region MaintenanceModuleItem
    public class MaintenanceModuleItem
    {
        public int ID { get; set; }
        public int Module { get; set; }
        public int Item { get; set; }
    }
    public class MaintenanceModuleItemMap : EntityMap<MaintenanceModuleItem>
    {
        public MaintenanceModuleItemMap()
        {
            Map(o => o.Module).ToColumn("module");
        }
    }
    #endregion

    #region MaintenanceModuleItemValue
    public class MaintenanceModuleItemValueParm
    {
        public List<MaintenanceModuleItemValue> MaintenanceModuleItemValues { get; set; }
        public bool IsFinished { get; set; }

    }
    public class MaintenanceModuleItemValue
    {
        public int ID { get; set; }
        public int List { get; set; }
        public int Module { get; set; }
        public int Item { get; set; }
        public string Eqp { get; set; }
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
        public int ItemType { get; set; }
    }
    public class MaintenanceModuleItemValueMap : EntityMap<MaintenanceModuleItemValue>
    {
        public MaintenanceModuleItemValueMap()
        {
            Map(o => o.ItemValue).ToColumn("item_value");
            Map(o => o.ItemType).ToColumn("item_type");
            Map(o => o.ItemName).ToColumn("item_name");
        }
    }
    #endregion

    #region MaintenanceModule
    public class MaintenanceModule
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShowName { get; set; }
        public int Type { get; set; }
        public string PlanCode { get; set; }
        public bool IsShowEqp { get; set; }
        /// <summary>
        /// 当检查设施数量大于1的时候，ModuleItem和Eqp才能确定唯一子项的值
        /// </summary>
        public string Eqp { get; set; }

        public List<MaintenanceModuleItemValue> Items { get; set; }
    }
    public class MaintenanceModuleMap : EntityMap<MaintenanceModule>
    {
        public MaintenanceModuleMap()
        {
            Map(o => o.PlanCode).ToColumn("plan_code");
        }
    }
    #endregion

    #region MaintenanceList
    public class MaintenanceListParm: BaseQueryParm
    {
        public int? Status { get; set; }
        public string Title { get; set; }
    }
    public class MaintenanceListView
    {
        public List<MaintenanceList> rows{ get; set; }
        public int total { get; set; }
    }
    public class MaintenanceList
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Team { get; set; }
        public string TeamName { get; set; }
        public string PlanDate { get; set; }
        public int Location { get; set; }
        public string LocationName { get; set; }
        public int Locationby { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Remark { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime UpdatedTime { get; set; }
        public List<MaintenancePlanDetail> Details { get; set; }
    }
    public class MaintenanceListMap : EntityMap<MaintenanceList>
    {
        public MaintenanceListMap()
        {
            Map(o => o.TeamName).ToColumn("team_name");
            Map(o => o.StatusName).ToColumn("name");
            Map(o => o.PlanDate).ToColumn("plan_date");
            Map(o => o.Locationby).ToColumn("location_by");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedByName).ToColumn("cname");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedByName).ToColumn("user_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
        }
    }
    #endregion

    #region MaintenancePlanDetail
    public class MaintenancePlanDetail
    {
        public int ID { get; set; }
        public int List { get; set; }
        public int Detail { get; set; }
        public string PlanCode { get; set; }
        public int Count { get; set; }
        public int PMType { get; set; }
    }
    public class MaintenancePlanDetailMap : EntityMap<MaintenancePlanDetail>
    {
        public MaintenancePlanDetailMap()
        {
            Map(o => o.PMType).ToColumn("pm_type");
            Map(o => o.PlanCode).ToColumn("plan_code");
        }
    }
    #endregion

    #region MaintenanceModuleItemAll,根据检修单获取所有父项及其子项
    public class MaintenanceModuleItemAll
    {
        public int ID { get; set; }
        public string ModuleName { get; set; }
        public int Item { get; set; }
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
        public string Eqp { get; set; }
        public int ItemType { get; set; }
        public int Count { get; set; }
    }
    public class MaintenanceModuleItemAllMap : EntityMap<MaintenanceModuleItemAll>
    {
        public MaintenanceModuleItemAllMap()
        {
            Map(o => o.ItemName).ToColumn("item_name");
            Map(o => o.ItemType).ToColumn("item_type");
            Map(o => o.ItemValue).ToColumn("item_value");
            Map(o => o.ModuleName).ToColumn("name");
        }
    }
    #endregion


}