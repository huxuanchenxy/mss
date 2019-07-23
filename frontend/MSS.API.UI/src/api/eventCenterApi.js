import axios from './interceptors'
// let api = 'http://127.0.0.1:3851/api/v1'
let api = 'http://10.89.36.204:5801/orgapi'
export default {
  getConfig: () => { return axios.get(`${api}/eventcenter/config`).then(res => res.data) },
  getAllWarning: () => { return axios.get(`${api}/eventcenter/warning`).then(res => res.data) },
  getAllNotification: () => { return axios.get(`${api}/eventcenter/notification`).then(res => res.data) },
  getAlarm: () => {
    return axios.get(`/static/mock/alarm.json`).then(res => {
      return res.data
    })
  },
  getWarningHistory: (param) => { return axios.get(`${api}/eventcenter/history/warning`, { params: param }).then(res => res.data) },
  getNotificationHistory: (param) => { return axios.get(`${api}/eventcenter/history/notification`, { params: param }).then(res => res.data) },
  getAlarmHistory: () => {
    return axios.get(`/static/mock/alarmHistory.json`).then(res => {
      return res.data
    })
  }
}
