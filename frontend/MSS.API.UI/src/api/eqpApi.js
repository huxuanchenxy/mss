import axios from './interceptors'
// 'http://10.89.36.204:5800/authapi'
// 'http://127.0.0.1:3851/api/v1'
let api = 'http://10.89.36.204:5801/eqpapi'
export default {
  getEqpType: parm => { return axios.get(`${api}/EquipmentType`, {params: parm}).then(res => res.data) },
  getEqpTypeByID: id => { return axios.get(`${api}/EquipmentType/${id}`).then(res => res.data) },
  addEqpType: (fd, config) => { return axios.post(`${api}/EquipmentType`, fd, config).then(res => res.data) },
  updateEqpType: parm => { return axios.put(`${api}/EquipmentType`, parm).then(res => res.data) },
  delEqpType: ids => { return axios.delete(`${api}/EquipmentType/${ids}`).then(res => res.data) },
  getEqpTypeAll: id => { return axios.get(`${api}/EquipmentType/All`).then(res => res.data) }
}
