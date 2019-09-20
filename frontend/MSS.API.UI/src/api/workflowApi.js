import axios from './interceptors'
let api = 'http://10.89.36.160:8443/api/v1'
// let api = 'http://10.89.36.154:5801/monitorapi'
export default {
  getPage: parm => { return axios.get(`${api}/Wf/QueryReadyTasks`, {params: parm}).then(res => res.data) },
  getMyApplyPage: parm => { return axios.get(`${api}/Wf/GetPageMyApply`, {params: parm}).then(res => res.data) }
}
