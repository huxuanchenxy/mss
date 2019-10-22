import axios from './interceptors'
// let api = 'http://10.89.36.160:8443/api/v1'
// let api = 'http://10.89.36.154:5801/workflowapi'
let api = 'http://localhost:3851/api/v1'
export default {
  getPage: parm => { return axios.get(`${api}/Wf/QueryReadyTasks`, {params: parm}).then(res => res.data) },
  getMyApplyPage: parm => { return axios.get(`${api}/Wf/GetPageMyApply`, {params: parm}).then(res => res.data) },
  getProcessHisPage: parm => { return axios.get(`${api}/Wf/GetPageActivityInstance`, {params: parm}).then(res => res.data) },

  getImportPage: parm => { return axios.get(`${api}/ConstructionPlanImport`, {params: parm}).then(res => res.data) },
  getCommonPage: parm => { return axios.get(`${api}/ConstructionPlanImport/ListPageCommon`, {params: parm}).then(res => res.data) },

  createMonthPlan: query => { return axios.post(`${api}/ConstructionPlanMonthDetail/${query}`).then(res => res.data) },
  getMonthPlanPage: parm => { return axios.get(`${api}/ConstructionPlanMonthDetail/`, {params: parm}).then(res => res.data) },
  getMonthPlanDetailByID: id => { return axios.get(`${api}/ConstructionPlanMonthDetail/${id}`).then(res => res.data) },
  updateMonthPlan: parm => { return axios.put(`${api}/ConstructionPlanMonthDetail`, parm).then(res => res.data) }
}
