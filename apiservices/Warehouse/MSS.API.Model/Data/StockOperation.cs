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
        public int? FromWarehouse { get; set; }
        public string FromWarehouseName { get; set; }
        /// <summary>
        /// 采购退货时对应的采购接收id
        /// </summary>
        public int? FromStockOperation { get; set; }
        public string PickerName { get; set; }
        public int? Picker { get; set; }
        public string PickerDeptName { get; set; }
        public string SupplierName { get; set; }
        public int? Supplier { get; set; }
        public string Agreement { get; set; }
        public string BudgetDeptName { get; set; }
        public string BudgetDeptPath { get; set; }
        public int? BudgetDept { get; set; }
        public string BudgetItems { get; set; }
        public string CreatedName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string DetailList { get; set; }
        public string DetailEditList { get; set; }
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
            Map(o => o.FromWarehouse).ToColumn("from_warehouse");
            Map(o => o.FromWarehouseName).ToColumn("fromWName");
            Map(o => o.FromStockOperation).ToColumn("from_stock_operation");
            Map(o => o.Picker).ToColumn("picker");
            Map(o => o.PickerName).ToColumn("pname");
            Map(o => o.PickerDeptName).ToColumn("pdname");
            Map(o => o.Supplier).ToColumn("supplier");
            Map(o => o.SupplierName).ToColumn("sname");
            Map(o => o.BudgetDept).ToColumn("budget_dept");
            Map(o => o.BudgetDeptPath).ToColumn("budget_dept_path");
            Map(o => o.BudgetDeptName).ToColumn("bname");

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
        public int? SearchFromWarehouse { get; set; }
        public int? SearchWarehouse { get; set; }
        public int? SearchSupplier { get; set; }
        public int? SearchPicker { get; set; }
        public DateTime? SearchStart { get; set; }
        public DateTime? SearchEnd { get; set; }
    }

    public class StockOperationSaveParm
    {
        public StockOperation stockOperation { get; set; }
        public List<StockOperationDetail> stockOperationDetails { get; set; }
        public List<Stock> stocks { get; set; }
        public List<StockDetail> stockDetails { get; set; }
        public bool isAddStockDetails { get; set; }
        public List<StockSum> stockSums { get; set; }
    }
    #endregion

    #region 库存操作明细
    public class StockOperationDetail
    {
        public int ID { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public string SparePartsModel { get; set; }
        public string SparePartsUnit { get; set; }
        public int OrderNo { get; set; }
        public int CountNo { get; set; }
        public double UnitPrice { get; set; }
        public double Amount { get; set; }
        public int Currency { get; set; }
        public string CurrencyName { get; set; }
        public string Invoice { get; set; }
        public DateTime? LifeDate { get; set; }
        public string SupplierName { get; set; }
        public string WorkingOrder { get; set; }
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
    }

    public class StockOperationDetailMap : EntityMap<StockOperationDetail>
    {
        public StockOperationDetailMap()
        {
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("name");
            Map(o => o.SparePartsModel).ToColumn("model");
            Map(o => o.SparePartsUnit).ToColumn("unit");

            Map(o => o.OrderNo).ToColumn("order_no");
            Map(o => o.CountNo).ToColumn("count_no");
            Map(o => o.UnitPrice).ToColumn("unit_price");
            Map(o => o.Amount).ToColumn("amount");
            Map(o => o.Currency).ToColumn("currency");
            Map(o => o.CurrencyName).ToColumn("cname");
            Map(o => o.Invoice).ToColumn("invoice");
            Map(o => o.LifeDate).ToColumn("life_date");
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
        public double Amount { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public bool isAdd { get; set; }
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
            Map(o => o.Amount).ToColumn("amount");
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("name");
        }
    }
    #endregion

    #region 存货明细，以采购接收/其他接收为原始记录
    public class StockDetail
    {
        public int ID { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public int StockOperationDetail { get; set; }
        public int StockNo { get; set; }
        public int TroubleNo { get; set; }
        public int InStockNo { get; set; }
        public int InspectionNo { get; set; }
        public int RepairNo { get; set; }
        public int Status { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
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
        public DateTime LifeDate { get; set; }
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
        /// <summary>
        /// 针对接收时修改的数量
        /// </summary>
        public int EditNo { get; set; }
    }

    public class StockDetailMap : EntityMap<StockDetail>
    {
        public StockDetailMap()
        {
            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("spname");
            Map(o => o.StockOperationDetail).ToColumn("stock_operation_detail");
            Map(o => o.StockNo).ToColumn("stock_no");
            Map(o => o.TroubleNo).ToColumn("trouble_no");
            Map(o => o.InStockNo).ToColumn("in_stock_no");
            Map(o => o.InspectionNo).ToColumn("inspection_no");
            Map(o => o.RepairNo).ToColumn("repair_no");
            Map(o => o.Status).ToColumn("status");
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("name");
            Map(o => o.SupplierName).ToColumn("sname");
            Map(o => o.AcceptDate).ToColumn("created_time");
            Map(o => o.AcceptNo).ToColumn("count_no");
            Map(o => o.LifeDate).ToColumn("life_date");
            Map(o => o.AcceptUnitPrice).ToColumn("unit_price");
            Map(o => o.CurrencyName).ToColumn("cname");
            Map(o => o.ExchangeRate).ToColumn("exchange_rate");
            Map(o => o.Remark).ToColumn("remark");
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
        public double Amount { get; set; }
        public bool isAdd { get; set; }
        public List<Stock> stocks { get; set; }
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
            Map(o => o.Amount).ToColumn("amount");
        }
    }

    public class StockSumQueryParm : BaseQueryParm
    {
        public int? SearchSpareParts { get; set; }
        public int? SearchWarehouse { get; set; }
    }

    public class StockSumView
    {
        public List<StockSum> rows { get; set; }
        public int total { get; set; }
    }
    #endregion

}
