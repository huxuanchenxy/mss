import axios from './interceptors'
// let api = 'http://127.0.0.1:3851/api/v1'
let eqpApi = 'http://127.0.0.1:5801/eqpapi'
// let eqpApi = 'http://localhost:8084/api/v1'
let api = 'http://127.0.0.1:5801/orgapi'
// let api = 'http://localhost:8082/api/v1'
export default {
  getConfig: () => { return axios.get(`${api}/eventcenter/config`).then(res => res.data) },
  getAllWarning: () => { return axios.get(`${api}/eventcenter/warning`).then(res => res.data) },
  getAllNotification: () => { return axios.get(`${api}/eventcenter/notification`).then(res => res.data) },
  getAllNotificationPidcount: () => { return axios.get(`${eqpApi}/NotificationPidcount/GetPageList`).then(res => res.data) },
  getAllNotificationPidcountHis: (param) => { return axios.get(`${eqpApi}/NotificationPidcount/GetPageListHis`, { params: param }).then(res => res.data) },
  deleteNotificationPidcount: (id) => { return axios.delete(`${eqpApi}/NotificationPidcount/${id}`).then(res => res.data) },
  updateNotificationPidcount: parm => { return axios.put(`${eqpApi}/NotificationPidcount`, parm).then(res => res.data) },
  getAlarm: () => { return axios.get(`${api}/eventcenter/alarm/`).then(res => res.data) },
  // getAlarmEqp: (param) => { return axios.get(`${api}/eventcenter/alarm/eqps`, {params: param}).then(res => res.data) },
  getAlarmEqp: (param) => { return axios.post(`${eqpApi}/Equipment/ids`, param).then(res => res.data) },
  getWarningHistory: (param) => { return axios.get(`${api}/eventcenter/history/warning`, { params: param }).then(res => res.data) },
  getNotificationHistory: (param) => { return axios.get(`${api}/eventcenter/history/notification`, { params: param }).then(res => res.data) },
  getAlarmHistory: (param) => { return axios.get(`${api}/eventcenter/history/alarm`, { params: param }).then(res => res.data) },
  deleteNotification: (id) => { return axios.delete(`${api}/eventcenter/notification/${id}`).then(res => res.data) }
}
