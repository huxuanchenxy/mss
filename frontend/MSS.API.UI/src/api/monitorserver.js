import axios from './interceptors'
// let api = 'http://10.89.36.160:8401/api/v1'
let api = 'http://10.89.36.154:5801/monitorapi'
export default {
  getPage: parm => { return axios.get(`${api}/op/MonitorServer`, {params: parm}).then(res => res.data) }
}