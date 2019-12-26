import axios from './interceptors'
let api = 'http://10.89.36.154:5801/areaapi' // 'http://localhost:52227/api'
// let api = 'http://localhost:8083/api'
// let api = 'http://localhost:3851/api'
export default {
  getAllUsers: () => { return {} },
  SaveConfigBigArea: data => { return axios.post(`${api}/System/SaveConfigBigArea`, data).then(res => res.data) },
  UpdateConfigBigArea: data => { return axios.put(`${api}/System/UpdateConfigBigArea`, data).then(res => res.data) },
  GetBigAreaQueryPageByParm: parm => { return axios.get(`${api}/System/GetBigAreaQueryPageByParm`, {params: parm}).then(res => res.data) },
  GetConfigBigAreaId: id => { return axios.get(`${api}/System/GetConfigBigAreaId/${id}`).then(res => res.data) },
  DelConfigBigAreaId: id => { return axios.get(`${api}/System/DelConfigBigAreaId/${id}`).then(res => res.data) },
  MutilDelConfigBigAreaId: Ids => { return axios.get(`${api}/System/MutilDelConfigBigAreaId/${Ids}`).then(res => res.data) },
  GetSubWayStation: id => { return axios.get(`${api}/System/GetSubWayStation/${id}`).then(res => res.data) },
  SaveConfigMidArea: data => { return axios.post(`${api}/System/SaveConfigMidArea`, data).then(res => res.data) },
  UpdateConfigMidArea: data => { return axios.put(`${api}/System/UpdateConfigMidArea`, data).then(res => res.data) },
  GetMidAreaQueryPageByParm: parm => { return axios.get(`${api}/System/GetMidAreaQueryPageByParm`, {params: parm}).then(res => res.data) },
  GetConfigMidAreaId: id => { return axios.get(`${api}/System/GetConfigMidAreaId/${id}`).then(res => res.data) },
  DelConfigMidAreaId: id => { return axios.get(`${api}/System/DelConfigMidAreaId/${id}`).then(res => res.data) },
  MutilDelConfigMidAreaId: Ids => { return axios.get(`${api}/System/MutilDelConfigMidAreaId/${Ids}`).then(res => res.data) },
  SelectDicAreaData: AreaCode => { return axios.get(`${api}/System/SelectDicAreaData/${AreaCode}`).then(res => res.data) },
  SelectConfigAreaData: () => { return axios.get(`${api}/System/SelectConfigAreaData`).then(res => res.data) },
  GetNameByUid: id => { return axios.get(`${api}/System/GetNameByUid/${id}`).then(res => res.data) },
  ListBigAreaByLine: id => { return axios.get(`${api}/System/ListBigAreaByLine/${id}`).then(res => res.data) }
}
