import axios from './interceptors'
// let api = 'http://localhost:3851/api/v1'
let api = 'http://10.89.34.154:5801/authapi'
export default {
  login: (acc, pwd) => { return axios.get(`${api}/User/Login/${acc}/${pwd}`).then(res => res.data) },
  getMenu: () => { return axios.get(`${api}/User/GetMenu`).then(res => res.data) },
  getUser: parm => { return axios.get(`${api}/User/QueryList`, {params: parm}).then(res => res.data) },
  getUserByID: id => { return axios.get(`${api}/User/${id}`).then(res => res.data) },
  addUser: parm => { return axios.post(`${api}/User/Add`, parm).then(res => res.data) },
  updateUser: parm => { return axios.put(`${api}/User/Update`, parm).then(res => res.data) },
  delUser: ids => { return axios.delete(`${api}/User/${ids}`).then(res => res.data) },
  getUserAll: () => { return axios.get(`${api}/User/All`).then(res => res.data) },
  changePwd: (oldPwd, newPwd) => { return axios.put(`${api}/User/changePwd/${oldPwd}/${newPwd}`).then(res => res.data) },
  ResetPwd: ids => { return axios.put(`${api}/User/ResetPwd/${ids}`).then(res => res.data) },
  getActionByUser: () => { return axios.get(`${api}/User/GetAction`).then(res => res.data) },

  getSubCode: code => { return axios.get(`${api}/Dictionary/SubCode/${code}`).then(res => res.data) },
  getSubCodeOrder: code => { return axios.get(`${api}/Dictionary/SubCodeByOrder/${code}`).then(res => res.data) },
  getTwoCascader: code => { return axios.get(`${api}/Dictionary/GetTwoCascader/${code}`).then(res => res.data) },
  getBusinessType: code => { return axios.get(`${api}/Dictionary/BusinessType/${code}`).then(res => res.data) },
  // getDictionary: parm => { return axios.get(`${api}/Dictionary/QueryList`, {params: parm}).then(res => res.data) },
  // getDictionaryByID: id => { return axios.get(`${api}/Dictionary/${id}`).then(res => res.data) },
  // addDictionary: parm => { return axios.post(`${api}/Dictionary/Add`, parm).then(res => res.data) },
  // updateDictionary: parm => { return axios.put(`${api}/Dictionary/Update`, parm).then(res => res.data) },
  // delDictionary: ids => { return axios.delete(`${api}/Dictionary/${ids}`).then(res => res.data) },

  getActionGroup: parm => { return axios.get(`${api}/ActionGroup/QueryList`, {params: parm}).then(res => res.data) },
  getActionGroupByID: id => { return axios.get(`${api}/ActionGroup/${id}`).then(res => res.data) },
  addActionGroup: parm => { return axios.post(`${api}/ActionGroup/Add`, parm).then(res => res.data) },
  updateActionGroup: parm => { return axios.put(`${api}/ActionGroup/Update`, parm).then(res => res.data) },
  delActionGroup: ids => { return axios.delete(`${api}/ActionGroup/${ids}`).then(res => res.data) },
  getActionGroupAll: () => { return axios.get(`${api}/ActionGroup/All`).then(res => res.data) },

  getAction: parm => { return axios.get(`${api}/Action/QueryList`, {params: parm}).then(res => res.data) },
  getActionByID: id => { return axios.get(`${api}/Action/${id}`).then(res => res.data) },
  addAction: parm => { return axios.post(`${api}/Action/Add`, parm).then(res => res.data) },
  updateAction: parm => { return axios.put(`${api}/Action/Update`, parm).then(res => res.data) },
  delAction: ids => { return axios.delete(`${api}/Action/${ids}`).then(res => res.data) },
  getActionAll: () => { return axios.get(`${api}/Action/All`).then(res => res.data) },
  getActionMenu: parm => { return axios.get(`${api}/Action/Menu`, {params: parm}).then(res => res.data) },
  getActionTree: id => { return axios.get(`${api}/Action/ActionTree`).then(res => res.data) },

  getRole: parm => { return axios.get(`${api}/Role/QueryList`, {params: parm}).then(res => res.data) },
  getRoleByID: id => { return axios.get(`${api}/Role/${id}`).then(res => res.data) },
  addRole: parm => { return axios.post(`${api}/Role/Add`, parm).then(res => res.data) },
  updateRole: parm => { return axios.put(`${api}/Role/Update`, parm).then(res => res.data) },
  delRole: ids => { return axios.delete(`${api}/Role/${ids}`).then(res => res.data) },
  getRoleAll: () => { return axios.get(`${api}/Role/All`).then(res => res.data) },
  getMenuMock: () => {
    return axios.get(`/static/mock/menu.json`).then(res => {
      return res.data
    })
  }
}
