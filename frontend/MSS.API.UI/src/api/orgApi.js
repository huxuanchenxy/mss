import axios from './interceptors'
let api = 'http://127.0.0.1:3851/api/v1'
let serviceAuth = 'http://10.89.36.204:5800/authapi'
export default {
  getOrgAll: () => { return axios.get(`${api}/org/all`).then(res => res.data) },
  getOrgNode: id => { return axios.get(`${api}/org/node/${id}`).then(res => res.data) },
  addOrgNode: data => { return axios.post(`${api}/org`, data).then(res => res.data) },
  updateOrgNode: (id, data) => { return axios.put(`${api}/org/${id}`, data).then(res => res.data) },
  getAllUsers: () => { return axios.get(`${serviceAuth}/user/all`).then(res => res.data) },
  BindOrgUser: data => { return axios.post(`${api}/org/user`, data).then(res => res.data) },
  getOrgUser: id => { return axios.get(`${api}/org/user/${id}`).then(res => res.data) },
  getCanSelectedUser: id => { return axios.get(`${api}/org/user/all/${id}`).then(res => res.data) },
  delOrgNode: id => { return axios.delete(`${api}/org/${id}`).then(res => res.data) }
}
