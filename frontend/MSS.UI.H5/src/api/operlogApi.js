import axios from './interceptors'
// let api = 'http://10.89.36.187:3861/api/v1'
let api = 'http://10.89.36.154:5801/operlogapi'
export default {
  getOperationLogByID: id => { return axios.get(`${api}/UserOperationLog/${id}`).then(res => res.data) },
  getOperationLog: parm => { return axios.get(`${api}/UserOperationLog/QueryList`, {params: parm}).then(res => res.data) }
}
