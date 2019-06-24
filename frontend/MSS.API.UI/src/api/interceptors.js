import axios from 'axios'
import router from '../router'

// 添加请求拦截器
axios.interceptors.request.use(config => {
  let token = window.sessionStorage.getItem('token')
  if (token) { // 判断是否存在token，如果存在的话，则每个http header都加上token
    config.headers.Authorization = `Bearer ${token}`
  }
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
  if (error.response.status === 401) {
    router.push({ name: 'Login' })
  }
  return Promise.reject(error)
})

export default axios
