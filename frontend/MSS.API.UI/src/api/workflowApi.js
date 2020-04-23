import axios from './interceptors'
let api = 'http://localhost:8443/api/v1'
// let api = 'http://10.89.36.154:5801/workflowapi'
export default {
  getPage: parm => { return axios.get(`${api}/Wf/QueryReadyTasks`, {params: parm}).then(res => res.data) },
  getMyApplyPage: parm => { return axios.get(`${api}/Wf/GetPageMyApply`, {params: parm}).then(res => res.data) },
  getProcessHisPage: parm => { return axios.get(`${api}/Wf/GetPageActivityInstance`, {params: parm}).then(res => res.data) },
  getMyFinprocess: parm => { return axios.get(`${api}/Wf/QueryFinishTasks`, {params: parm}).then(res => res.data) },
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
  withdrawprocess: parm => { return axios.post(`${api}/Wf/WithdrawProcess`, parm).then(res => res.data) },
  sendbackprocess: parm => { return axios.post(`${api}/Wf/SendBackProcess`, parm).then(res => res.data) },
  getprocesslist: id => { return axios.get(`${api}/Wf/GetProcessListSimple`).then(res => res.data) },
  getnextprocess: parm => { return axios.post(`${api}/Wf/GetNextStepRoleUserTree`, parm).then(res => res.data) },

  getcurrentprocess: parm => { return axios.post(`${api}/Wf/QueryReadyActivityInstance`, parm).then(res => res.data) },
  getMonthChart: parm => { return axios.get(`${api}/ConstructionPlanMonthChart/GetMonthChart`, {params: parm}).then(res => res.data) },

  saveMList: parm => { return axios.post(`${api}/Maintenance/SaveMList`, parm).then(res => res.data) },
  saveMMoudleItemValue: parm => { return axios.post(`${api}/Maintenance/SaveMMoudleItemValue`, parm).then(res => res.data) },
  getMList: parm => { return axios.get(`${api}/Maintenance`, {params: parm}).then(res => res.data) },
  listMModuleByID: (id, isInit) => { return axios.get(`${api}/Maintenance/ListMModule/${id}/${isInit}`).then(res => res.data) },
  getModuleList: parm => { return axios.get(`${api}/Maintenance/ListModulePage`, {params: parm}).then(res => res.data) },
  getModuleByID: id => { return axios.get(`${api}/Maintenance/GetModuleByID/${id}`).then(res => res.data) },

  savePMEntity: parm => { return axios.post(`${api}/Maintenance/SavePMEntity`, parm).then(res => res.data) },
  getEntityList: parm => { return axios.get(`${api}/Maintenance/ListEntityPage`, {params: parm}).then(res => res.data) },
  delEntity: ids => { return axios.delete(`${api}/Maintenance/${ids}`).then(res => res.data) },
  getEntityByID: (id, isUpdate) => { return axios.get(`${api}/Maintenance/GetEntityByID/${id}/${isUpdate}`).then(res => res.data) },
  updatePMEntity: parm => { return axios.put(`${api}/Maintenance/UpdatePMEntity`, parm).then(res => res.data) },
  updatePMEntityStatus: (id, status) => { return axios.put(`${api}/Maintenance/UpdatePMEntityStatus/${id}/${status}`).then(res => res.data) }
}
