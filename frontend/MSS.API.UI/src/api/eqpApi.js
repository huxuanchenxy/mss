import axios from './interceptors'
// let api = 'http://localhost:3851/api/v1'
let api = 'http://localhost:8081/api/v1'
// let api = 'http://127.0.0.1:5801/eqpapi'
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
  addPidCountDetail: parm => { return axios.post(`${api}/PidCountDetail`, parm).then(res => res.data) },

  getFirmByID: id => { return axios.get(`${api}/Firm/${id}`).then(res => res.data) },
  addFirm: parm => { return axios.post(`${api}/Firm`, parm).then(res => res.data) },
  updateFirm: parm => { return axios.put(`${api}/Firm`, parm).then(res => res.data) },
  delFirm: ids => { return axios.delete(`${api}/Firm/${ids}`).then(res => res.data) },
  getFirmAll: () => { return axios.get(`${api}/Firm/All`).then(res => res.data) },
  getFirmByType: type => { return axios.get(`${api}/Firm/ListByType/${type}`).then(res => res.data) },

  getAllEqpCount: () => { return axios.get(`${api}/Equipment/count`).then(res => res.data) },

  getImportExcelConfig: parm => { return axios.get(`${api}/ImportExcelConfig`, {params: parm}).then(res => res.data) },
  getImportExcelConfigByID: id => { return axios.get(`${api}/ImportExcelConfig/${id}`).then(res => res.data) },
  addImportExcelConfig: parm => { return axios.post(`${api}/ImportExcelConfig`, parm).then(res => res.data) },
  updateImportExcelConfig: parm => { return axios.put(`${api}/ImportExcelConfig`, parm).then(res => res.data) },
  delImportExcelConfig: ids => { return axios.delete(`${api}/ImportExcelConfig/${ids}`).then(res => res.data) },

  getImportExcelClass: () => { return axios.get(`${api}/ImportExcelConfig/ListClass`).then(res => res.data) },
  getPropertyByClass: id => { return axios.get(`${api}/ImportExcelConfig/ListPropertyByClass/${id}`).then(res => res.data) },

  getImportExcelLog: parm => { return axios.get(`${api}/ImportExcelConfig/ListLogByParm`, {params: parm}).then(res => res.data) }
}
