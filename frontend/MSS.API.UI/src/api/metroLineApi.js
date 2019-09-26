import axios from './interceptors'
// let api = 'http://127.0.0.1:3851/api/v1'
let api = 'http://10.89.36.154:5801/orgapi'
export default {
  getByPage: (param) => { return axios.get(`${api}/metroline`, { params: param }).then(res => res.data) },
  getByID: (id) => { return axios.get(`${api}/metroline/${id}`).then(res => res.data) },
  getAll: () => { return axios.get(`${api}/metroline/all`).then(res => res.data) },
  save: (data) => { return axios.post(`${api}/metroline`, data).then(res => res.data) },
  update: (id, data) => { return axios.put(`${api}/metroline/${id}`, data).then(res => res.data) },
  delete: ids => { return axios.delete(`${api}/metroline/${ids}`).then(res => res.data) }
}
