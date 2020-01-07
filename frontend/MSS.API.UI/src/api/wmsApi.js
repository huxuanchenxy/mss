import axios from './interceptors'
let api = 'http://localhost:3851/api/v1'
// let api = 'http://10.89.36.154:5801/wmsapi'
export default {
  // 仓库
  getWarehouse: parm => { return axios.get(`${api}/Warehouse`, {params: parm}).then(res => res.data) },
  getWarehouseByID: id => { return axios.get(`${api}/Warehouse/${id}`).then(res => res.data) },
  addWarehouse: parm => { return axios.post(`${api}/Warehouse`, parm).then(res => res.data) },
  updateWarehouse: parm => { return axios.put(`${api}/Warehouse`, parm).then(res => res.data) },
  delWarehouse: ids => { return axios.delete(`${api}/Warehouse/${ids}`).then(res => res.data) },
  getWarehouseAll: () => { return axios.get(`${api}/Warehouse/All`).then(res => res.data) },
  // 库位
  getStorageLocation: parm => { return axios.get(`${api}/StorageLocation`, {params: parm}).then(res => res.data) },
  getStorageLocationByID: id => { return axios.get(`${api}/StorageLocation/${id}`).then(res => res.data) },
  addStorageLocation: parm => { return axios.post(`${api}/StorageLocation`, parm).then(res => res.data) },
  updateStorageLocation: parm => { return axios.put(`${api}/StorageLocation`, parm).then(res => res.data) },
  delStorageLocation: ids => { return axios.delete(`${api}/StorageLocation/${ids}`).then(res => res.data) },
  getStorageLocationAll: () => { return axios.get(`${api}/StorageLocation/All`).then(res => res.data) },
  getStorageLocationByWarehouse: warehouse => { return axios.get(`${api}/StorageLocation/ListByWarehouse/${warehouse}`).then(res => res.data) },
  // 仓库库位级联下拉，目前无用
  listWarehyouseLocation: () => { return axios.get(`${api}/StorageLocation/ListWarehyouseLocation`).then(res => res.data) },
  // 物资
  getSpareParts: parm => { return axios.get(`${api}/SpareParts`, {params: parm}).then(res => res.data) },
  getSparePartsByID: id => { return axios.get(`${api}/SpareParts/${id}`).then(res => res.data) },
  addSpareParts: parm => { return axios.post(`${api}/SpareParts`, parm).then(res => res.data) },
  updateSpareParts: parm => { return axios.put(`${api}/SpareParts`, parm).then(res => res.data) },
  delSpareParts: ids => { return axios.delete(`${api}/SpareParts/${ids}`).then(res => res.data) },
  getSparePartsAll: () => { return axios.get(`${api}/SpareParts/All`).then(res => res.data) },
  // 仓库预警
  getWarehouseAlarm: parm => { return axios.get(`${api}/WarehouseAlarm`, {params: parm}).then(res => res.data) },
  getWarehouseAlarmByID: id => { return axios.get(`${api}/WarehouseAlarm/${id}`).then(res => res.data) },
  addWarehouseAlarm: parm => { return axios.post(`${api}/WarehouseAlarm`, parm).then(res => res.data) },
  updateWarehouseAlarm: parm => { return axios.put(`${api}/WarehouseAlarm`, parm).then(res => res.data) },
  delWarehouseAlarm: ids => { return axios.delete(`${api}/WarehouseAlarm/${ids}`).then(res => res.data) },
  // 库存操作
  getStockOperation: parm => { return axios.get(`${api}/StockOperation`, {params: parm}).then(res => res.data) },
  getStockOperationByID: id => { return axios.get(`${api}/StockOperation/${id}`).then(res => res.data) },

  getStockOperationByReason: id => { return axios.get(`${api}/StockOperation/ListByReason/${id}`).then(res => res.data) },
  getStockOperationDetailByID: id => { return axios.get(`${api}/StockOperation/ListByOperation/${id}`).then(res => res.data) },
  getStockOperationDetailByIDForEdit: id => { return axios.get(`${api}/StockOperation/ListByOperationForEdit/${id}`).then(res => res.data) },
  getStockDetailByID: id => { return axios.get(`${api}/StockOperation/GetStockDetailByID/${id}`).then(res => res.data) },
  getSparePartsByWH: (warehouse) => { return axios.get(`${api}/StockOperation/GetSparePartsByWH/${warehouse}`).then(res => res.data) },
  addStockOperation: parm => { return axios.post(`${api}/StockOperation`, parm).then(res => res.data) },

  getStockSum: parm => { return axios.get(`${api}/StockOperation/ListStockSum`, {params: parm}).then(res => res.data) },
  getStockDetail: (id, warehouse, reason, storageLocation) => { return axios.get(`${api}/StockOperation/ListStockDetail/${id}/${warehouse}/${reason}/${storageLocation}`).then(res => res.data) },
  getStockDetailByReason: (reason) => { return axios.get(`${api}/StockOperation/GetStockDetailByReason/${reason}`).then(res => res.data) },
  // 库存预警历史
  getWarehouseAlarmHistory: parm => { return axios.get(`${api}/WarehouseAlarmHistory`, {params: parm}).then(res => res.data) }
}
