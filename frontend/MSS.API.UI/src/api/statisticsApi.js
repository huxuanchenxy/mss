import axios from './interceptors'
// let api = 'http://127.0.0.1:8088/api/v1'
let api = 'http://127.0.0.1:5801/statisticsserviceapi'
export default {
  reportAlarm: (param) => { return axios.get(`${api}/statistics/alarm`, { params: param }).then(res => res.data) },
  reportSubChartEqpType: (param) => { return axios.get(`${api}/statistics/alarm/groupbyeqptype`, { params: param }).then(res => res.data) },
  reportSubChartSupplier: (param) => { return axios.get(`${api}/statistics/alarm/groupbysupplier`, { params: param }).then(res => res.data) },
  reportSubChartManufacturer: (param) => { return axios.get(`${api}/statistics/alarm/groupbymanufacturer`, { params: param }).then(res => res.data) },
  reportSubChartSubSystem: (param) => { return axios.get(`${api}/statistics/alarm/groupbysubsystem`, { params: param }).then(res => res.data) },
  reportSubChartLocation: (param) => { return axios.get(`${api}/statistics/alarm/groupbylocation`, { params: param }).then(res => res.data) },
  reportSubChartOrg: (param) => { return axios.get(`${api}/statistics/alarm/groupbyorg`, { params: param }).then(res => res.data) },
  // 故障
  reportTroubleByDate: (param) => { return axios.get(`${api}/statistics/trouble/date`, { params: param }).then(res => res.data) },
  reportTroubleByOther: (param) => { return axios.get(`${api}/statistics/trouble/other`, { params: param }).then(res => res.data) },
  reportTroubleByLocation: (param) => { return axios.get(`${api}/statistics/trouble/groupbylocation`, { params: param }).then(res => res.data) },
  reportTroubleByOrg: (param) => { return axios.get(`${api}/statistics/trouble/groupbyorg`, { params: param }).then(res => res.data) },
  getStockOperChart: (param) => { return axios.get(`${api}/StockOperationDetail/GetStockOperationChart`, { params: param }).then(res => res.data) },
  reportTroubleRankByStation: (param) => { return axios.get(`${api}/statistics/trouble/rankbystation`, { params: param }).then(res => res.data) },
  reportRunningtime: (param) => { return axios.get(`${api}/statistics/trouble/runningtime`, { params: param }).then(res => res.data) },
  reportPlanChart: (param) => { return axios.get(`${api}/statistics/trouble/planchart`, { params: param }).then(res => res.data) },
  getNow: (param) => { return axios.get(`${api}/statistics/trouble/getnow`, { params: param }).then(res => res.data) },
  getRunningCost: (param) => { return axios.get(`${api}/statistics/trouble/runningcost`, { params: param }).then(res => res.data) },
  getPidChart: (param) => { return axios.get(`${api}/statistics/trouble/pidchart`, { params: param }).then(res => res.data) },
  getCostChart: (param) => { return axios.get(`${api}/statistics/trouble/costchart`, { params: param }).then(res => res.data) }
}
