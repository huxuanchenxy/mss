import axios from './interceptors'
// let api = 'http://localhost:3851/api/v1'
// let api = 'http://localhost:8084/api/v1'
let api = 'http://localhost:8081/api/v1'
export default {
  // deleteUploadFile: id => { return axios.delete(`${api}/Upload/${id}`).then(res => res.data) },
  getUploadFileByIDs: ids => { return axios.get(`${api}/Upload/${ids}`).then(res => res.data) },
  getUploadCascaderByIDs: ids => { return axios.get(`${api}/Upload/Cascader/${ids}`).then(res => res.data) },
  getUploadFileByEqp: id => { return axios.get(`${api}/Upload/ListByEqp/${id}`).then(res => res.data) },
  addUploadFileRelation: list => { return axios.post(`${api}/Upload/SaveList`, list).then(res => res.data) },
  fileIsExist: id => { return axios.get(`${api}/Upload/FileIsExist/${id}`).then(res => res.data) },

  downloadFile: id => { return axios({method: 'post', url: `${api}/Upload/Download/${id}`, responseType: 'blob'}).then(res => res) }
}
