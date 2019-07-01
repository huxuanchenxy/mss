import axios from './interceptors'
let api = 'http://localhost:52227/api' // 'http://10.89.36.204:5801/areaapi'
export default {
  getAllUsers: () => { return {} },
  SaveConfigBigArea: data => { return axios.post(`${api}/System/SaveConfigBigArea`, data).then(res => res.data) },
  UpdateConfigBigArea: data => { return axios.post(`${api}/System/UpdateConfigBigArea`, data).then(res => res.data) },
  GetBigAreaQueryPageByParm: parm => { return axios.get(`${api}/System/GetBigAreaQueryPageByParm`, {params: parm}).then(res => res.data) },
  GetConfigBigAreaId: id => { return axios.get(`${api}/System/GetConfigBigAreaId/${id}`).then(res => res.data) },
  DelConfigBigAreaId: id => { return axios.get(`${api}/System/DelConfigBigAreaId/${id}`).then(res => res.data) },
  MutilDelConfigBigAreaId: Ids => { return axios.get(`${api}/System/MutilDelConfigBigAreaId/${Ids}`).then(res => res.data) },
  GetSubWayStation: () => { return axios.get(`${api}/System/GetSubWayStation`).then(res => res.data) },
  SaveConfigMidArea: data => { return axios.post(`${api}/System/SaveConfigMidArea`, data).then(res => res.data) },
  UpdateConfigMidArea: data => { return axios.post(`${api}/System/UpdateConfigMidArea`, data).then(res => res.data) },
  GetMidAreaQueryPageByParm: parm => { return axios.get(`${api}/System/GetMidAreaQueryPageByParm`, {params: parm}).then(res => res.data) },
  GetConfigMidAreaId: id => { return axios.get(`${api}/System/GetConfigMidAreaId/${id}`).then(res => res.data) },
  DelConfigMidAreaId: id => { return axios.get(`${api}/System/DelConfigMidAreaId/${id}`).then(res => res.data) },
  MutilDelConfigMidAreaId: Ids => { return axios.get(`${api}/System/MutilDelConfigMidAreaId/${Ids}`).then(res => res.data) },
  SelectDicAreaData: AreaCode => { return axios.get(`${api}/System/SelectDicAreaData/${AreaCode}`).then(res => res.data) }
}
