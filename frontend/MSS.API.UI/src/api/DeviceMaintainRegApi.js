import axios from './interceptors'
let api = 'http://10.89.34.154:5801/maintainapi'
// let api = 'http://localhost:3851/api/v1'
export default {
  getAllUsers: () => { return {} },
  Save: data => { return axios.post(`${api}/DeviceMaintainReg/Save`, data).then(res => res.data) },
  Update: data => { return axios.put(`${api}/DeviceMaintainReg/Update`, data).then(res => res.data) },
  GetListByPage: parm => { return axios.get(`${api}/DeviceMaintainReg/GetListByPage`, {params: parm}).then(res => res.data) },
  GetDeviceMaintainRegById: id => { return axios.get(`${api}/DeviceMaintainReg/GetDeviceMaintainRegById/${id}`).then(res => res.data) },
  GetTeamGroupList: () => { return axios.get(`${api}/DeviceMaintainReg/GetTeamGroupList`).then(res => res.data) },
  GetDirectorList: () => { return axios.get(`${api}/DeviceMaintainReg/GetDirectorList`).then(res => res.data) },
  GetEquipmentTypeList: () => { return axios.get(`${api}/DeviceMaintainReg/GetEquipmentTypeList`).then(res => res.data) },
  GetDeviceListByTypeId: id => { return axios.get(`${api}/LifeTimeKeyMaintain/GetDeviceListByTypeId/${id}`).then(res => res.data) },
  GetEqpByTypeAndLine: type => { return axios.get(`${api}/LifeTimeKeyMaintain/GetEqpByTypeAndLine/${type}`).then(res => res.data) },
  Delete: id => { return axios.delete(`${api}/DeviceMaintainReg/Delete/${id}`).then(res => res.data) },
  GetLifeTimeKeyListByPage: parm => { return axios.get(`${api}/LifeTimeKeyMaintain/GetLifeTimeKeyListByPage`, {params: parm}).then(res => res.data) },
  DeleteList: Ids => { return axios.delete(`${api}/DeviceMaintainReg/DeleteList/${Ids}`, Ids).then(res => res.data) },

  ListByEqp: (id, isHide) => { return axios.get(`${api}/EqpHistory/ListByEqp/${id}/${isHide}`).then(res => res.data) },
  getWorkingApplicationByID: id => { return axios.get(`${api}/WorkingApplication/${id}`).then(res => res.data) },

  getTroubleReportByID: id => { return axios.get(`${api}/TroubleReport/${id}`).then(res => res.data) },
  getTroubleReportPage: parm => { return axios.get(`${api}/TroubleReport`, {params: parm}).then(res => res.data) },
  Operation: (ids, operation, content) => { return axios.put(`${api}/TroubleReport/operation/${ids}/${operation}/${content}`).then(res => res.data) },
  SaveTroubleReport: parm => { return axios.post(`${api}/TroubleReport`, parm).then(res => res.data) },
  UpdateTroubleReport: parm => { return axios.put(`${api}/TroubleReport`, parm).then(res => res.data) },
  getTroubleHistoryByID: id => { return axios.get(`${api}/TroubleReport/ListHistoryByTrouble/${id}`).then(res => res.data) },
  getTroubleEqpByID: (id, topOrg, troubleView) => { return axios.get(`${api}/TroubleReport/ListEqpByTrouble/${id}/${topOrg}/${troubleView}`).then(res => res.data) },
  assignEqp: (eqps) => { return axios.put(`${api}/TroubleReport/AssignEqp/${eqps}`).then(res => res.data) },
  getAllTroubleReport: () => { return axios.get(`${api}/TroubleReport/ListAll`).then(res => res.data) },
  getTroubleReportByStatus: status => { return axios.get(`${api}/TroubleReport/ListByStatus/${status}`).then(res => res.data) },

  saveTroubleDeal: parm => { return axios.post(`${api}/TroubleReport/SaveDeal`, parm).then(res => res.data) },
  getDealByID: (id, orgTop) => { return axios.get(`${api}/TroubleReport/GetDealByID/${id}/${orgTop}`).then(res => res.data) },

  getEqpRepair: parm => { return axios.get(`${api}/EquipmentRepairHistory`, {params: parm}).then(res => res.data) },
  getEqpRepairByID: id => { return axios.get(`${api}/EquipmentRepairHistory/${id}`).then(res => res.data) },
  addEqpRepair: parm => { return axios.post(`${api}/EquipmentRepairHistory`, parm).then(res => res.data) },
  updateEqpRepair: parm => { return axios.put(`${api}/EquipmentRepairHistory`, parm).then(res => res.data) },
  delEqpRepair: ids => { return axios.delete(`${api}/EquipmentRepairHistory/${ids}`).then(res => res.data) },

  getHealthConfig: parm => { return axios.get(`${api}/HealthConfig`, {params: parm}).then(res => res.data) },
  getHealthConfigByTroubleLevel: troubleLevel => { return axios.get(`${api}/HealthConfig/${troubleLevel}`).then(res => res.data) },
  getHealthConfigByType: type => { return axios.get(`${api}/HealthConfig/GetByType/${type}`).then(res => res.data) },
  addHealthConfig: parm => { return axios.post(`${api}/HealthConfig`, parm).then(res => res.data) },
  updateHealthConfig: parm => { return axios.put(`${api}/HealthConfig`, parm).then(res => res.data) },
  delHealthConfig: ids => { return axios.delete(`${api}/HealthConfig/${ids}`).then(res => res.data) },

  getHealth: parm => { return axios.get(`${api}/Health`, {params: parm}).then(res => res.data) }
}
