import axios from './interceptors'
let api = 'http://localhost:8443/api/v1'
// let api = 'http://10.89.36.154:5801/workflowapi'
export default {
  getPage: parm => { return axios.get(`${api}/Wf/QueryReadyTasks`, {params: parm}).then(res => res.data) },
  getMyApplyPage: parm => { return axios.get(`${api}/Wf/GetPageMyApply`, {params: parm}).then(res => res.data) },
  getProcessHisPage: parm => { return axios.get(`${api}/Wf/GetPageActivityInstance`, {params: parm}).then(res => res.data) },
  addConstructionPlan: parm => { return axios.post(`${api}/ConstructionPlan`, parm).then(res => res.data) },
  getConstructionPlanPage: parm => { return axios.get(`${api}/ConstructionPlan/GetPageList`, {params: parm}).then(res => res.data) },
  getConstructionPlanByID: (id) => { return axios.get(`${api}/ConstructionPlan/${id}`).then(res => res.data) }
}
