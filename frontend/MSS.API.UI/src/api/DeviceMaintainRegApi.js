import axios from './interceptors'
let api = 'http://10.89.36.204:5801/maintainapi'
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
  Delete: id => { return axios.delete(`${api}/DeviceMaintainReg/Delete/${id}`).then(res => res.data) },
  GetLifeTimeKeyListByPage: parm => { return axios.get(`${api}/LifeTimeKeyMaintain/GetLifeTimeKeyListByPage`, {params: parm}).then(res => res.data) },
  DeleteList: Ids => { return axios.delete(`${api}/DeviceMaintainReg/DeleteList/${Ids}`, Ids).then(res => res.data) },
  ListByEqp: id => { return axios.get(`${api}/EqpHistory/ListByEqp/${id}`).then(res => res.data) }
}
