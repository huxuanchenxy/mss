import axios from './interceptors'
// let api = 'http://10.89.34.160:8400/api/v1'
let api = 'http://10.89.34.154:5801/consulapi160'
export default {
  getPage: parm => { return axios.get(`${api}/Consul`, {params: parm}).then(res => res.data) },
  startProcess: id => { return axios.get(`${api}/Consul/start/${id}`).then(res => res.data) },
  stopProcess: id => { return axios.get(`${api}/Consul/stop/${id}`).then(res => res.data) }
}
