import axios from './interceptors'
import {api, mock} from './baseUrl'

export default {
    getOrgAll: () => { return axios.get(`${api}/org/all`).then(res => res.data) },
    getOrgNode: id => { return axios.get(`${api}/org/node/${id}`).then(res => res.data) },
    addOrgNode: data => { return axios.post(`${api}/org`, data).then(res => res.data) },
    updateOrgNode: (id, data) => { return axios.put(`${api}/org/${id}`, data).then(res => res.data) },
    getAllUsers: () => { return axios.get(`${mock}/users/all`).then(res => res.data) },
    BindOrgUser: data => { return axios.post(`${api}/org/user`, data).then(res => res.data) },
    getOrgUser: id => { return axios.get(`${api}/org/user/${id}`).then(res => res.data) }
}
