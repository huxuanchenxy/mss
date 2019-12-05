
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
    public class MaintenanceModuleItemValue
    {
        public int ID { get; set; }
        public int ModuleItem { get; set; }
        public int Item { get; set; }
        public string ItemValue { get; set; }
    }
    public class MaintenanceModuleItemValueMap : EntityMap<MaintenanceModuleItemValue>
    {
        public MaintenanceModuleItemValueMap()
        {
            Map(o => o.ItemValue).ToColumn("item_value");
            Map(o => o.ModuleItem).ToColumn("module_item");
        }
    }
    #endregion

    #region MaintenanceModule
    public class MaintenanceModule
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string PlanCode { get; set; }
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
    public class MaintenanceList
    {
        public int Team { get; set; }
        public int PMType { get; set; }
        public DateTime PlanDate { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
    public class MaintenanceListMap : EntityMap<MaintenanceList>
    {
        public MaintenanceListMap()
        {
            Map(o => o.PMType).ToColumn("pm_type");
            Map(o => o.PlanDate).ToColumn("plan_date");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedByName).ToColumn("cname");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedByName).ToColumn("user_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
        }
    }
    #endregion


}