import axios from './interceptors'
// let api = 'http://localhost:3851/api/v1'
let api = 'http://10.89.36.204:5801/wmsapi'
export default {
  getWarehouse: parm => { return axios.get(`${api}/Warehouse`, {params: parm}).then(res => res.data) },
  getWarehouseByID: id => { return axios.get(`${api}/Warehouse/${id}`).then(res => res.data) },
  addWarehouse: parm => { return axios.post(`${api}/Warehouse`, parm).then(res => res.data) },
  updateWarehouse: parm => { return axios.put(`${api}/Warehouse`, parm).then(res => res.data) },
  delWarehouse: ids => { return axios.delete(`${api}/Warehouse/${ids}`).then(res => res.data) },
  getWarehouseAll: () => { return axios.get(`${api}/Warehouse/All`).then(res => res.data) },

  getSpareParts: parm => { return axios.get(`${api}/SpareParts`, {params: parm}).then(res => res.data) },
  getSparePartsByID: id => { return axios.get(`${api}/SpareParts/${id}`).then(res => res.data) },
  addSpareParts: parm => { return axios.post(`${api}/SpareParts`, parm).then(res => res.data) },
  updateSpareParts: parm => { return axios.put(`${api}/SpareParts`, parm).then(res => res.data) },
  delSpareParts: ids => { return axios.delete(`${api}/SpareParts/${ids}`).then(res => res.data) },
  getSparePartsAll: () => { return axios.get(`${api}/SpareParts/All`).then(res => res.data) },

  getWarehouseAlarm: parm => { return axios.get(`${api}/WarehouseAlarm`, {params: parm}).then(res => res.data) },
  getWarehouseAlarmByID: id => { return axios.get(`${api}/WarehouseAlarm/${id}`).then(res => res.data) },
  addWarehouseAlarm: parm => { return axios.post(`${api}/WarehouseAlarm`, parm).then(res => res.data) },
  updateWarehouseAlarm: parm => { return axios.put(`${api}/WarehouseAlarm`, parm).then(res => res.data) },
  delWarehouseAlarm: ids => { return axios.delete(`${api}/WarehouseAlarm/${ids}`).then(res => res.data) },

  getStockOperation: parm => { return axios.get(`${api}/StockOperation`, {params: parm}).then(res => res.data) },
  getStockOperationByID: id => { return axios.get(`${api}/StockOperation/${id}`).then(res => res.data) },
  addStockOperation: parm => { return axios.post(`${api}/StockOperation`, parm).then(res => res.data) }
}
