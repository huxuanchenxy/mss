import axios from './interceptors'
import {mock} from './baseUrl'

export default {
    getMenu: params => { return axios.get(`${mock}/UserInfo/GetMenu`, { params: params }).then(res => res.data) },
    Login: params => { return axios.get(`${mock}/Login`, { params: params }).then(res => res.data) }
}
