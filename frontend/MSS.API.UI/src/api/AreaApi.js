import axios from './interceptors'
let api = 'http://localhost:52227/api/System'
export default { 
    getAllUsers: () => { return  {} }, 
     SaveConfigBigArea: data=>{ return axios.post(`${api}/SaveConfigBigArea`, data).then(res => res.data)},
     UpdateConfigBigArea: data=>{ return axios.post(`${api}/UpdateConfigBigArea`, data).then(res => res.data)},
     GetBigAreaQueryPageByParm: parm => { return axios.get(`${api}/GetBigAreaQueryPageByParm`, {params: parm}).then(res => res.data) },
     GetConfigBigAreaId: id => { return axios.get(`${api}/GetConfigBigAreaId/${id}`).then(res => res.data) },
     DelConfigBigAreaId: id => { return axios.get(`${api}/DelConfigBigAreaId/${id}`).then(res => res.data) },
     MutilDelConfigBigAreaId: Ids=> { return axios.get(`${api}/MutilDelConfigBigAreaId/${Ids}`).then(res => res.data) },
    
    
     GetChezhanData: () => { return axios.get(`${api}/GetChezhanData`).then(res => res.data) },
    
    
     SaveConfigMidArea: data=>{ return axios.post(`${api}/SaveConfigMidArea`, data).then(res => res.data)},
     UpdateConfigMidArea: data=>{ return axios.post(`${api}/UpdateConfigMidArea`, data).then(res => res.data)},
     GetMidAreaQueryPageByParm: parm => { return axios.get(`${api}/GetMidAreaQueryPageByParm`, {params: parm}).then(res => res.data) },
     GetConfigMidAreaId: id => { return axios.get(`${api}/GetConfigMidAreaId/${id}`).then(res => res.data) },
     DelConfigMidAreaId: id => { return axios.get(`${api}/DelConfigMidAreaId/${id}`).then(res => res.data) },
     MutilDelConfigMidAreaId: Ids=> { return axios.get(`${api}/MutilDelConfigMidAreaId/${Ids}`).then(res => res.data) },        
     
    SelectDicAreaData: AreaCode => { return axios.get(`${api}/SelectDicAreaData/${AreaCode}`).then(res => res.data) }
     
}