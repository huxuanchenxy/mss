import axios from './interceptors'
// let api = 'http://localhost:3851/api/v1'
let api = 'http://10.89.36.204:5801/wmsapi'
export default {
  getWarehouse: parm => { return axios.get(`${api}/Warehouse`, {params: parm}).then(res => res.data) },
  getWarehouseByID: id => { return axios.get(`${api}/Warehouse/${id}`).then(res => res.data) },
  addWarehouse: parm => { return axios.post(`${api}/Warehouse`, parm).then(res => res.data) },
  updateWarehouse: parm => { return axios.put(`${api}/Warehouse`, parm).then(res => res.data) },
  delWarehouse: ids => { return axios.delete(`${api}/Warehouse/${ids}`).then(res => res.data) },
  getWarehouseAll: () => { return axios.get(`${api}/Warehouse/All`).then(res => res.data) }
}
