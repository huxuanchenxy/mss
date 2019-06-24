import axios from './interceptors'
// let api = 'http://127.0.0.1:3851/api/v1'
let serviceAuth = 'http://10.89.36.204:5801/idsapi'
export default {
  Login: data => { return axios.post(`${serviceAuth}/Login/GetToken`, data).then(res => res.data) }
}
