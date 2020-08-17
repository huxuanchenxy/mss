import axios from './interceptors'
// let api = 'http://127.0.0.1:3851/api/v1'
let gatewayAuth = 'http://10.89.34.154:5801/idsapi'
let serviceAuth = 'http://10.89.34.154:5801/authapi'
export default {
  Login: data => { return axios.post(`${gatewayAuth}/Login/GetToken`, data).then(res => res.data) },
  getUserInfo: () => { return axios.get(`${serviceAuth}/User`).then(res => res.data) },
  getUserAction: () => { return axios.get(`${serviceAuth}/User/GetAction`).then(res => res.data) }
}
