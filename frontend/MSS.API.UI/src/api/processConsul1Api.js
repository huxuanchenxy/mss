import axios from './interceptors'
// let api = 'http://localhost:8400/api/v1'
let api = 'http://10.89.36.154:5801/consulapi'
export default {
  getPage: parm => { return axios.get(`${api}/Consul`, {params: parm}).then(res => res.data) },
  startProcess: id => { return axios.get(`${api}/Consul/remotestart/${id}`).then(res => res.data) },
  stopProcess: id => { return axios.get(`${api}/Consul/remotestop/${id}`).then(res => res.data) }
}
