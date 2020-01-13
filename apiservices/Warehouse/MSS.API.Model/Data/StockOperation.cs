using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;

/// <summary>
/// 库存总表 sum from 仓库明细 sum from 存货明细——库存操作明细 detail of 库存操作
/// </summary>
namespace MSS.API.Model.Data
{
    #region 库存操作：出入库、调整、移库
    public class StockOperation
    {
        public int ID { get; set; }
        public string OperationID { get; set; }
        public int Type { get; set; }
        public int Reason { get; set; }
        public string ReasonName { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public string Remark { get; set; }
        public int? ToWarehouse { get; set; }
        public string ToWarehouseName { get; set; }
        /// <summary>
        /// 采购退货时对应的采购接收id
        /// 借用/送修/送检归坏 对应的 借用/送修/送检id
        /// </summary>
        public int? FromStockOperation { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string FromStockOperationName { get; set; }
        public string PickerName { get; set; }
        public int? Picker { get; set; }
        public string PickerDeptName { get; set; }
        public string Agreement { get; set; }
        public string BudgetDeptName { get; set; }
        public string BudgetDeptPath { get; set; }
        public int? BudgetDept { get; set; }
        public string BudgetItems { get; set; }
        public string CreatedName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string DetailList { get; set; }
        public string SomeOrder { get; set; }
        public int? WorkingOrder { get; set; }
        public string WorkingOrderCode { get; set; }
    }

    public class StockOperationMap : EntityMap<StockOperation>
    {
        public StockOperationMap()
        {
            Map(o => o.OperationID).ToColumn("operation_id");
            Map(o => o.Type).ToColumn("type");

            Map(o => o.Reason).ToColumn("reason");
            Map(o => o.ReasonName).ToColumn("name");
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("wname");
            Map(o => o.ToWarehouse).ToColumn("to_warehouse");
            Map(o => o.ToWarehouseName).ToColumn("toWName");
            Map(o => o.FromStockOperation).ToColumn("from_stock_operation");
            Map(o => o.FromStockOperationName).ToColumn("fsoName");
            Map(o => o.Picker).ToColumn("picker");
            Map(o => o.PickerName).ToColumn("pname");
            Map(o => o.PickerDeptName).ToColumn("pdname");
            Map(o => o.BudgetDept).ToColumn("budget_dept");
            Map(o => o.BudgetDeptPath).ToColumn("budget_dept_path");
            Map(o => o.BudgetDeptName).ToColumn("bname");
            Map(o => o.WorkingOrderCode).ToColumn("code");
            Map(o => o.WorkingOrder).ToColumn("working_order");

            Map(o => o.Remark).ToColumn("remark");
            Map(o => o.Agreement).ToColumn("agreement");
            Map(o => o.BudgetItems).ToColumn("budget_items");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("user_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
        }
    }

    public class StockOperationQueryParm:BaseQueryParm
    {
        public int? SearchType { get; set; }
        public int? SearchReason { get; set; }
        public int? SearchToWarehouse { get; set; }
        public int? SearchWarehouse { get; set; }
        public string SearchAgreement { get; set; }
        public int? SearchPicker { get; set; }
        public DateTime? SearchStart { get; set; }
        public DateTime? SearchEnd { get; set; }
    }

    public class StockOperationSaveParm
    {
        public StockOperation stockOperation { get; set; }
        public List<StockOperationDetail> stockOperationDetails { get; set; }
        public List<StockOperationDetail> stockOperationDetailsUpdate { get; set; }
        public List<Stock> stocks { get; set; }
        public List<StockDetail> stockDetails { get; set; }
        public List<StockDetail> stockDetailsUpdate { get; set; }
        //public bool isAddStockDetails { get; set; }
        public List<StockSum> stockSums { get; set; }
        /// <summary>
        /// 移库专用
        /// </summary>
        //public List<StockDetail> stockDetailsAdd { get; set; }
        /// <summary>
        /// 仓库预警历史
        /// </summary>
        public List<WarehouseAlarmHistory> wAlarmHistory { get; set; }
    }
    #endregion

    #region 库存操作明细
    public class StockOperationDetail
    {
        public int ID { get; set; }
        public string Entity { get; set; }
        public int InStockNo { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public string SparePartsModel { get; set; }
        public string SparePartsUnit { get; set; }
        public int Warehouse { get; set; }
        public int StorageLocation { get; set; }
        public string StorageLocationName { get; set; }
        public int? FromStorageLocation { get; set; }
        public string FromStorageLocationName { get; set; }
        public int OrderNo { get; set; }
        public int CountNo { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public int Currency { get; set; }
        public string CurrencyName { get; set; }
        public string Invoice { get; set; }
        public DateTime? LifeDate { get; set; }
        public int? Supplier { get; set; }
        public string SupplierName { get; set; }
        public int? WorkingOrder { get; set; }
        public string Purchase { get; set; }
        public string Repair { get; set; }
        /// <summary>
        /// reason - order
        /// 采购接收 - 采购单
        /// 送修/送修归还 - 送修单
        /// 送检/送检归还 - 送检单
        /// 上述两个字段，Purchase和Repair弃用但预留
        /// </summary>
        public string SomeOrder { get; set; }

        public int Operation { get; set; }
        public double ExchangeRate { get; set; }
        public double TotalAmount { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 采购/其他接收时默认0，其余操作需要关联存货明细中的ID
        /// </summary>
        public int StockDetail { get; set; }
        /// <summary>
        /// 借用/送修/送检归还时对应的入库记录ID
        /// </summary>
        public int FromStockOperationDetail { get; set; }
        /// <summary>
        /// 借用/送修/送检归还数量
        /// </summary>
        public int ReturnNo { get; set; }
        /// <summary>
        /// 操作状态，目前主要描述借用/送修/送检未归还情况为1，其余0
        /// </summary>
        public int Status { get; set; }
        public int EditNo { get; set; }
        /// <summary>
        /// 移库专用，批次转移时需要新物资ID
        /// </summary>
        public string NewEntity { get; set; }
    }

    public class StockOperationDetailMap : EntityMap<StockOperationDetail>
    {
        public StockOperationDetailMap()
        {
            Map(o => o.Entity).ToColumn("entity");
            Map(o => o.InStockNo).ToColumn("in_stock_no");
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("name");
            Map(o => o.SparePartsModel).ToColumn("model");
            Map(o => o.SparePartsUnit).ToColumn("unit");
            Map(o => o.StorageLocation).ToColumn("storage_location");
            Map(o => o.StorageLocationName).ToColumn("slname");
            Map(o => o.FromStorageLocation).ToColumn("from_storage_location");
            Map(o => o.FromStorageLocationName).ToColumn("fslname");

            Map(o => o.OrderNo).ToColumn("order_no");
            Map(o => o.CountNo).ToColumn("count_no");
            Map(o => o.UnitPrice).ToColumn("unit_price");
            Map(o => o.Amount).ToColumn("amount");
            Map(o => o.Currency).ToColumn("currency");
            Map(o => o.CurrencyName).ToColumn("cname");
            Map(o => o.Invoice).ToColumn("invoice");
            Map(o => o.LifeDate).ToColumn("life_date");
            Map(o => o.Supplier).ToColumn("supplier");
            Map(o => o.SupplierName).ToColumn("sname");
            Map(o => o.WorkingOrder).ToColumn("working_order");
            Map(o => o.Purchase).ToColumn("purchase");
            Map(o => o.Repair).ToColumn("repair");
            Map(o => o.SomeOrder).ToColumn("some_order");
            Map(o => o.Operation).ToColumn("operation");
            Map(o => o.ExchangeRate).ToColumn("exchange_rate");
            Map(o => o.TotalAmount).ToColumn("total_amount");
            Map(o => o.Remark).ToColumn("remark");
            Map(o => o.StockDetail).ToColumn("stock_detail");
            Map(o => o.FromStockOperationDetail).ToColumn("stock_operation_detail");
            Map(o => o.ReturnNo).ToColumn("return_no");
            Map(o => o.Status).ToColumn("status");
        }
    }
    #endregion

    #region 仓库明细，仓库维度
    public class Stock
    {
        public int ID { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public int StockNo { get; set; }
        public int TroubleNo { get; set; }
        public int InStockNo { get; set; }
        public int InspectionNo { get; set; }
        public int RepairNo { get; set; }
        public int LentNo { get; set; }
        public int ScrapNo { get; set; }
        public double Amount { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public int StorageLocation { get; set; }
        public string StorageLocationName { get; set; }
        public bool IsAdd { get; set; }
        public int IsAlarm { get; set; }
        public int EditNo { get; set; }
    }

    public class StockMap : EntityMap<Stock>
    {
        public StockMap()
        {
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("spname");
            Map(o => o.StockNo).ToColumn("stock_no");
            Map(o => o.TroubleNo).ToColumn("trouble_no");
            Map(o => o.InStockNo).ToColumn("in_stock_no");
            Map(o => o.InspectionNo).ToColumn("inspection_no");
            Map(o => o.RepairNo).ToColumn("repair_no");
            Map(o => o.LentNo).ToColumn("lent_no");
            Map(o => o.ScrapNo).ToColumn("scrap_no");
            Map(o => o.Amount).ToColumn("amount");
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("name");
            Map(o => o.StorageLocation).ToColumn("storage_location");
            Map(o => o.StorageLocationName).ToColumn("slname");
            Map(o => o.EditNo).ToColumn("editNo");
            Map(o => o.IsAlarm).ToColumn("is_alarm");
        }
    }
    #endregion

    #region 存货明细，以采购接收/其他接收为原始记录
    public class StockDetail
    {
        public int ID { get; set; }
        public string Entity { get; set; }
        public int IsBatch { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public string Model { get; set; }
        public int StockOperationDetail { get; set; }
        public int FromStockOperationDetail { get; set; }
        public int StockNo { get; set; }
        public int TroubleNo { get; set; }
        public int InStockNo { get; set; }
        public int InspectionNo { get; set; }
        public int RepairNo { get; set; }
        public int LentNo { get; set; }
        public int ScrapNo { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public int StorageLocation { get; set; }
        public string StorageLocationName { get; set; }
        /// <summary>
        /// 接收数量
        /// </summary>
        public int AcceptNo { get; set; }
        /// <summary>
        /// 接收日期
        /// </summary>
        public DateTime AcceptDate { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }
        /// <summary>
        /// 保质期
        /// </summary>
        public DateTime? LifeDate { get; set; }
        /// <summary>
        /// 接收单价
        /// </summary>
        public double AcceptUnitPrice { get; set; }
        /// <summary>
        /// 接收金额
        /// </summary>
        public double AcceptAmount { get; set; }
        /// <summary>
        /// 接收币种
        /// </summary>
        public string CurrencyName { get; set; }
        /// <summary>
        /// 接收汇率
        /// </summary>
        public double ExchangeRate { get; set; }
        /// <summary>
        /// StockOperationDetail的备注
        /// </summary>
        public string Remark { get; set; }
        public string SomeOrder { get; set; }
        //public string WorkingOrderName { get; set; }
        public int WorkingOrder { get; set; }
        /// <summary>
        /// 使前端显示默认为0
        /// </summary>
        public int EditNo { get; set; }
        /// <summary>
        /// 移库专用，批次时比如录入新物资ID
        /// </summary>
        public string NewEntity { get; set; }
    }

    public class StockDetailMap : EntityMap<StockDetail>
    {
        public StockDetailMap()
        {
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("spname");
            Map(o => o.IsBatch).ToColumn("is_batch");
            Map(o => o.Model).ToColumn("model");
            Map(o => o.StockOperationDetail).ToColumn("stock_operation_detail"); 
            Map(o => o.FromStockOperationDetail).ToColumn("fsod"); 
            Map(o => o.StockNo).ToColumn("stock_no");
            Map(o => o.TroubleNo).ToColumn("trouble_no");
            Map(o => o.InStockNo).ToColumn("in_stock_no");
            Map(o => o.InspectionNo).ToColumn("inspection_no");
            Map(o => o.RepairNo).ToColumn("repair_no");
            Map(o => o.LentNo).ToColumn("lent_no");
            Map(o => o.ScrapNo).ToColumn("scrap_no");
            Map(o => o.Status).ToColumn("status");
            Map(o => o.StatusName).ToColumn("statusName");
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("name");
            Map(o => o.StorageLocation).ToColumn("storage_location");
            Map(o => o.StorageLocationName).ToColumn("slname");
            Map(o => o.SupplierName).ToColumn("sname");
            Map(o => o.AcceptDate).ToColumn("created_time");
            Map(o => o.AcceptNo).ToColumn("count_no");
            Map(o => o.LifeDate).ToColumn("life_date");
            Map(o => o.AcceptUnitPrice).ToColumn("unit_price");
            Map(o => o.CurrencyName).ToColumn("cname");
            Map(o => o.ExchangeRate).ToColumn("exchange_rate");
            Map(o => o.Remark).ToColumn("remark");
            Map(o => o.SomeOrder).ToColumn("some_order");
            Map(o => o.WorkingOrder).ToColumn("working_order");
        }
    }
    #endregion

    #region 库存总表，物资维度
    public class StockSum
    {
        public int ID { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public int StockNo { get; set; }
        public int TroubleNo { get; set; }
        public int InStockNo { get; set; }
        public int InspectionNo { get; set; }
        public int RepairNo { get; set; }
        public int LentNo { get; set; }
        public int ScrapNo { get; set; }
        public double Amount { get; set; }
        public bool IsAdd { get; set; }
        public List<Stock> stocks { get; set; }
        public int IsAlarm { get; set; }
    }

    public class StockSumMap : EntityMap<StockSum>
    {
        public StockSumMap()
        {
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("name");
            Map(o => o.StockNo).ToColumn("stock_no");
            Map(o => o.TroubleNo).ToColumn("trouble_no");
            Map(o => o.InStockNo).ToColumn("in_stock_no");
            Map(o => o.InspectionNo).ToColumn("inspection_no");
            Map(o => o.RepairNo).ToColumn("repair_no");
            Map(o => o.LentNo).ToColumn("lent_no");
            Map(o => o.ScrapNo).ToColumn("scrap_no");
            Map(o => o.Amount).ToColumn("amount");
            Map(o => o.IsAlarm).ToColumn("is_alarm");
        }
    }

    public class StockSumQueryParm : BaseQueryParm
    {
        public int? SearchSpareParts { get; set; }
        public int? SearchWarehouse { get; set; }
        public bool SearchIsAlarm { get; set; }
    }

    public class StockSumView
    {
        public List<StockSum> rows { get; set; }
        public int total { get; set; }
    }
    #endregion

}
