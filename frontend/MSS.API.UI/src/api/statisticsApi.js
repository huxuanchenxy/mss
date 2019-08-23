import axios from './interceptors'
let api = 'http://127.0.0.1:8088/api/v1'
// let api = 'http://10.89.36.204:5801/idsapi'
export default {
  reportAlarm: (param) => { return axios.get(`${api}/statistics/alarm`, { params: param }).then(res => res.data) },
  reportSubChartEqpType: (param) => { return axios.get(`${api}/statistics/alarm/groupbyeqptype`, { params: param }).then(res => res.data) },
  reportSubChartSupplier: (param) => { return axios.get(`${api}/statistics/alarm/groupbysupplier`, { params: param }).then(res => res.data) },
  reportSubChartManufacturer: (param) => { return axios.get(`${api}/statistics/alarm/groupbymanufacturer`, { params: param }).then(res => res.data) },
  reportSubChartSubSystem: (param) => { return axios.get(`${api}/statistics/alarm/groupbysubsystem`, { params: param }).then(res => res.data) }
}
