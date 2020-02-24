import axios from './interceptors'
// let api = 'http://localhost:3851/api/v1'
let api = 'http://localhost:8084/api/v1'
// let api = 'http://10.89.36.154:5801/eqpapi'
export default {
  getEqpType: parm => { return axios.get(`${api}/EquipmentType`, {params: parm}).then(res => res.data) },
  getEqpTypeByID: id => { return axios.get(`${api}/EquipmentType/${id}`).then(res => res.data) },
  addEqpType: parm => { return axios.post(`${api}/EquipmentType`, parm).then(res => res.data) },
  updateEqpType: parm => { return axios.put(`${api}/EquipmentType`, parm).then(res => res.data) },
  delEqpType: ids => { return axios.delete(`${api}/EquipmentType/${ids}`).then(res => res.data) },
  getEqpTypeAll: () => { return axios.get(`${api}/EquipmentType/All`).then(res => res.data) },
  getListByPosition: (location, locationBy, eqpType) => { return axios.get(`${api}/Equipment/ListByPosition/${location}/${locationBy}/${eqpType}`).then(res => res.data) },

  getEqp: parm => { return axios.get(`${api}/Equipment`, {params: parm}).then(res => res.data) },
  getEqpByID: id => { return axios.get(`${api}/Equipment/${id}`).then(res => res.data) },
  getEqpDetailByID: id => { return axios.get(`${api}/Equipment/Detail/${id}`).then(res => res.data) },
  addEqp: parm => { return axios.post(`${api}/Equipment`, parm).then(res => res.data) },
  updateEqp: parm => { return axios.put(`${api}/Equipment`, parm).then(res => res.data) },
  delEqp: ids => { return axios.delete(`${api}/Equipment/${ids}`).then(res => res.data) },
  getEqpByTopOrg: (topOrg, line, location, locationBy) => { return axios.get(`${api}/Equipment/ListByTopOrg/${topOrg}/${line}/${location}/${locationBy}`).then(res => res.data) },
  getEqpByEqpType: ids => { return axios.get(`${api}/Equipment/GetByEqpType/${ids}`).then(res => res.data) },
  getEqpByTeam: id => { return axios.get(`${api}/Equipment/GetByTeam/${id}`).then(res => res.data) },
  getEqpAll: () => { return axios.get(`${api}/Equipment/All`).then(res => res.data) },

  getPidCount: parm => { return axios.get(`${api}/PidCount/GetPageList`, {params: parm}).then(res => res.data) },
  addPidCount: parm => { return axios.post(`${api}/PidCount`, parm).then(res => res.data) },
  getPidCountByID: id => { return axios.get(`${api}/PidCount/${id}`).then(res => res.data) },
  updatePidCount: parm => { return axios.put(`${api}/PidCount`, parm).then(res => res.data) },
  getFirm: parm => { return axios.get(`${api}/Firm`, {params: parm}).then(res => res.data) },
  getPidCountDetail: parm => { return axios.get(`${api}/PidCountDetail/GetPageList`, {params: parm}).then(res => res.data) },

  getFirmByID: id => { return axios.get(`${api}/Firm/${id}`).then(res => res.data) },
  addFirm: parm => { return axios.post(`${api}/Firm`, parm).then(res => res.data) },
  updateFirm: parm => { return axios.put(`${api}/Firm`, parm).then(res => res.data) },
  delFirm: ids => { return axios.delete(`${api}/Firm/${ids}`).then(res => res.data) },
  getFirmAll: () => { return axios.get(`${api}/Firm/All`).then(res => res.data) },
  getFirmByType: type => { return axios.get(`${api}/Firm/ListByType/${type}`).then(res => res.data) },

  // deleteUploadFile: id => { return axios.delete(`${api}/Upload/${id}`).then(res => res.data) },
  getUploadFileByIDs: ids => { return axios.get(`${api}/Upload/${ids}`).then(res => res.data) },
  getUploadCascaderByIDs: ids => { return axios.get(`${api}/Upload/Cascader/${ids}`).then(res => res.data) },
  getUploadFileByEqp: id => { return axios.get(`${api}/Upload/ListByEqp/${id}`).then(res => res.data) },
  addUploadFileRelation: list => { return axios.post(`${api}/Upload/SaveList`, list).then(res => res.data) },

  downloadFile: id => { return axios({method: 'post', url: `${api}/Upload/Download/${id}`, responseType: 'blob'}).then(res => res) },

  getAllEqpCount: () => { return axios.get(`${api}/Equipment/count`).then(res => res.data) },

  getEqpRepair: parm => { return axios.get(`${api}/EquipmentRepairHistory`, {params: parm}).then(res => res.data) },
  getEqpRepairByID: id => { return axios.get(`${api}/EquipmentRepairHistory/${id}`).then(res => res.data) },
  addEqpRepair: parm => { return axios.post(`${api}/EquipmentRepairHistory`, parm).then(res => res.data) },
  updateEqpRepair: parm => { return axios.put(`${api}/EquipmentRepairHistory`, parm).then(res => res.data) },
  delEqpRepair: ids => { return axios.delete(`${api}/EquipmentRepairHistory/${ids}`).then(res => res.data) }
}
