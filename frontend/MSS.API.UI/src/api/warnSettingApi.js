import axios from './interceptors'
let api = 'http://127.0.0.1:3851/api/v1'
// let api = 'http://10.89.36.204:5801/orgapi'
let serviceeqp = 'http://10.89.36.204:5801/eqpapi'
export default {
  getWarnSetting: (param) => { return axios.get(`${api}/warnsetting`, { params: param }).then(res => res.data) },
  getWarnSettingByID: (id) => { return axios.get(`${api}/warnsetting/${id}`).then(res => res.data) },
  getSettingExType: () => { return axios.get(`${api}/warnsetting/extype`).then(res => res.data) },
  saveWarnningSetting: (data) => { return axios.post(`${api}/warnsetting`, data).then(res => res.data) },
  updateWarnningSetting: (id, data) => { return axios.put(`${api}/warnsetting/${id}`, data).then(res => res.data) },
  deleteWarnningSetting: ids => { return axios.delete(`${api}/warnsetting/${ids}`).then(res => res.data) },
  getAllEqpType: () => { return axios.get(`${serviceeqp}/EquipmentType/All`).then(res => res.data) },
  // getAllEqpType: () => {
  //   return {
  //     then: (call) => {
  //       call({
  //         data: [
  //           {
  //             id: 1,
  //             tName: 'test'
  //           },
  //           {
  //             id: 2,
  //             tName: 'test2'
  //           }
  //         ]})
  //     }
  //   }
  // },
  getParamByEqpType: (id) => {
    return axios.get(`/static/mock/param.json`).then(res => {
      res.data = res.data.filter(item => item.type === id)
      return res
    })
  }
  // getParamByEqpType: (id) => {
  //   return {
  //     then: (call) => {
  //       var data
  //       if (id === 1) {
  //         data = [{
  //           _paramID: 'aa',
  //           _paramName: '温度',
  //           _paramUnit: '℃'
  //         }, {
  //           _paramID: 'aa2',
  //           _paramName: '温度2',
  //           _paramUnit: '℃'
  //         }]
  //       } else {
  //         data = [{
  //           _paramID: 'bb',
  //           _paramName: '湿度',
  //           _paramUnit: '度'
  //         }]
  //       }
  //       call({data: data})
  //     }
  //   }
  // }
}
