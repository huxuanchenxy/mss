import Axios from 'axios'
import qs from 'qs'
import router from '../router'

let option = {
  timeout: 20000,
  baseURL: process.env.NODE_ENV === 'production' ? '' : '/api',
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded',
    'X-Requested-With': 'XMLHttpRequest'
  },
  transformRequest: data => qs.stringify(data)
}

let axios = Axios.create(option)

// 添加请求拦截器
axios.interceptors.request.use(config => {
  return config
}, error => {
  return Promise.reject(error)
})

// 添加响应拦截器
axios.interceptors.response.use(response => {
  return {
    data: response.data,
    status: response.status,
    statusText: response.statusText
  }
}, error => {
  if (error.response.status === 502) {
    router.push({ name: 'Login' })
  }
  return Promise.reject(error)
})

export default axios
