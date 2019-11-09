import axios from './interceptors'
let api = 'http://localhost:8443/api/v1'
// let api = 'http://10.89.36.154:5801/workflowapi'
export default {
  getPage: parm => { return axios.get(`${api}/Wf/QueryReadyTasks`, {params: parm}).then(res => res.data) },
  getMyApplyPage: parm => { return axios.get(`${api}/Wf/GetPageMyApply`, {params: parm}).then(res => res.data) },
  getProcessHisPage: parm => { return axios.get(`${api}/Wf/GetPageActivityInstance`, {params: parm}).then(res => res.data) },

  getImportPage: parm => { return axios.get(`${api}/ConstructionPlanImport`, {params: parm}).then(res => res.data) },
  getCommonPage: parm => { return axios.get(`${api}/ConstructionPlanImport/ListPageCommon`, {params: parm}).then(res => res.data) },

  createMonthPlan: query => { return axios.post(`${api}/ConstructionPlanMonthDetail/${query}`).then(res => res.data) },
  getMonthPlanPage: parm => { return axios.get(`${api}/ConstructionPlanMonthDetail/`, {params: parm}).then(res => res.data) },
  getMonthPlanDetailByID: id => { return axios.get(`${api}/ConstructionPlanMonthDetail/${id}`).then(res => res.data) },
  updateMonthPlan: parm => { return axios.put(`${api}/ConstructionPlanMonthDetail`, parm).then(res => res.data) },
  addConstructionPlan: parm => { return axios.post(`${api}/ConstructionPlan`, parm).then(res => res.data) },
  getConstructionPlanPage: parm => { return axios.get(`${api}/ConstructionPlan/GetPageList`, {params: parm}).then(res => res.data) },
  updateConstructionPlan: parm => { return axios.put(`${api}/ConstructionPlan`, parm).then(res => res.data) },
  delConstructionPlan: ids => { return axios.delete(`${api}/ConstructionPlan/${ids}`).then(res => res.data) },
  getConstructionPlanByID: id => { return axios.get(`${api}/ConstructionPlan/${id}`).then(res => res.data) },
  startprocess: parm => { return axios.post(`${api}/Wf/StartProcess`, parm).then(res => res.data) },
  nextprocess: parm => { return axios.post(`${api}/Wf/NextProcess`, parm).then(res => res.data) },
  getnextprocess: parm => { return axios.post(`${api}/Wf/GetNextStepRoleUserTree`, parm).then(res => res.data) }
}
