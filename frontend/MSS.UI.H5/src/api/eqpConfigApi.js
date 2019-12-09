import axios from './interceptors'
// let api = 'http://10.89.36.187:3861/api/v1'
let api = 'http://10.89.36.154:5801/eqpapi'
export default {
  getEquipmentConfig: parm => { return axios.get(`${api}/EquipmentConfig`, {params: parm}).then(res => res.data) },
  addEquipmentConfig: data => { return axios.post(`${api}/EquipmentConfig`, data).then(res => res.data) },
  getEquipmentConfigByID: id => { return axios.get(`${api}/EquipmentConfig/${id}`).then(res => res.data) },
  updateEquipmentConfig: parm => { return axios.put(`${api}/EquipmentConfig`, parm).then(res => res.data) },
  delEquipmentConfig: ids => { return axios.delete(`${api}/EquipmentConfig/${ids}`).then(res => res.data) }
}
