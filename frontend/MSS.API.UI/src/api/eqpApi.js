import axios from './interceptors'
// 'http://10.89.36.204:5801/eqpapi'
// 'http://localhost:3851/api/v1'
let api = 'http://10.89.36.204:5801/eqpapi'
export default {
  getEqpType: parm => { return axios.get(`${api}/EquipmentType`, {params: parm}).then(res => res.data) },
  getEqpTypeByID: id => { return axios.get(`${api}/EquipmentType/${id}`).then(res => res.data) },
  addEqpType: (fd, config) => { return axios.post(`${api}/EquipmentType`, fd, config).then(res => res.data) },
  updateEqpType: (fd, config) => { return axios.put(`${api}/EquipmentType`, fd, config).then(res => res.data) },
  delEqpType: ids => { return axios.delete(`${api}/EquipmentType/${ids}`).then(res => res.data) },
  getEqpTypeAll: () => { return axios.get(`${api}/EquipmentType/All`).then(res => res.data) },

  getEqp: parm => { return axios.get(`${api}/Equipment`, {params: parm}).then(res => res.data) },
  getEqpByID: id => { return axios.get(`${api}/Equipment/${id}`).then(res => res.data) },
  addEqp: parm => { return axios.post(`${api}/Equipment`, parm).then(res => res.data) },
  updateEqp: parm => { return axios.put(`${api}/Equipment`, parm).then(res => res.data) },
  delEqp: ids => { return axios.delete(`${api}/Equipment/${ids}`).then(res => res.data) },
  getEqpAll: () => { return axios.get(`${api}/Equipment/All`).then(res => res.data) }
}