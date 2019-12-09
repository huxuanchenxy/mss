import axios from './interceptors'
// let api = 'http://127.0.0.1:3851/api/v1'
let api = 'http://10.89.36.154:5801/orgapi'
let serviceAuth = 'http://10.89.36.154:5801/authapi'
export default {
  // getOrgAll: () => { return axios.get(`${api}/org/all`).then(res => res.data) },
  getOrgAll: () => { return axios.get(`${api}/org/curorg`).then(res => res.data) },
  getOrgUserAll: () => { return axios.get(`${api}/orguser`).then(res => res.data) },
  getOrgNode: id => { return axios.get(`${api}/org/node/${id}`).then(res => res.data) },
  addOrgNode: data => { return axios.post(`${api}/org`, data).then(res => res.data) },
  updateOrgNode: (id, data) => { return axios.put(`${api}/org/${id}`, data).then(res => res.data) },
  getAllUsers: () => { return axios.get(`${serviceAuth}/user/all`).then(res => res.data) },
  BindOrgUser: data => { return axios.post(`${api}/org/user`, data).then(res => res.data) },
  getOrgUser: id => { return axios.get(`${api}/org/user/${id}`).then(res => res.data) },
  getSelectedUsers: () => { return axios.get(`${api}/orguser/selected`).then(res => res.data) },
  getCanSelectedUser: id => { return axios.get(`${api}/org/user/all/${id}`).then(res => res.data) },
  delOrgNode: id => { return axios.delete(`${api}/org/${id}`).then(res => res.data) },
  getOrgUserByNodeID: (id) => { return axios.get(`${api}/orguser/${id}`).then(res => res.data) },
  delOrgUser: ids => { return axios.delete(`${api}/orguser/${ids}`).then(res => res.data) },
  listNodeByNodeType: (nodeType) => { return axios.get(`${api}/org/ListNodeByNodeType/${nodeType}`).then(res => res.data) },
  listUserByNode: (node) => { return axios.get(`${api}/orguser/ListUserByNode/${node}`).then(res => res.data) },

  getOrgTopNodeByUser: (user) => { return axios.get(`${api}/org/topnode/${user}`).then(res => res.data) },
  listUserAllByTopNode: () => { return axios.get(`${api}/orguser/usersintoporg`).then(res => res.data) }
}
