import axios from './interceptors'
import {api} from './baseUrl'

export default {
    getActionGroupList: params => { return axios.get(`${api}/ActionGroup/GetActionGroup`, { params: params }).then(res => res.data) }
}
