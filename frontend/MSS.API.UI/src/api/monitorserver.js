import axios from './interceptors'
let api = 'http://10.89.34.187:8401/api/v1'
// let api = 'http://127.0.0.1:5801/monitorapi'
export default {
  getPage: parm => { return axios.get(`${api}/op/MonitorServer`, {params: parm}).then(res => res.data) },
  getPage2: parm => { return axios.get(`${api}/op/Dashboard2`).then(res => res.data) }
}
