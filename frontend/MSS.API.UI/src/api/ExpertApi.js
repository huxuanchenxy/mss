import axios from './interceptors'
let api = 'http://10.89.36.204:5801/maintainapi'
// let api = 'http://localhost:3851/api/v1'
export default {
  getAllUsers: () => { return {} },
  Save: data => { return axios.post(`${api}/ExpertData/Save`, data).then(res => res.data) },
  Update: data => { return axios.put(`${api}/ExpertData/Update`, data).then(res => res.data) },
  GetListByPage: parm => { return axios.get(`${api}/ExpertData/GetListByPage`, {params: parm}).then(res => res.data) },
  GetExpertDataById: id => { return axios.get(`${api}/ExpertData/GetExpertDataById/${id}`).then(res => res.data) },
  GetdeptList: () => { return axios.get(`${api}/ExpertData/GetdeptList`).then(res => res.data) },
  GetDeviceTypeList: () => { return axios.get(`${api}/ExpertData/GetDeviceTypeList`).then(res => res.data) },
  Delete: id => { return axios.delete(`${api}/ExpertData/Delete/${id}`).then(res => res.data) },
  DeleteList: Ids => { return axios.delete(`${api}/ExpertData/DeleteList/${Ids}`, Ids).then(res => res.data) }
}
