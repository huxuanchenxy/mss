import axios from './interceptors'
 //let api = 'http://localhost:3851/api/v1'
let api = 'http://10.89.36.204:5801/operlogapi'
export default {
  getOperationLog: parm => { return axios.get(`${api}/UserOperationLog/QueryList`, {params: parm}).then(res => res.data) }
}
