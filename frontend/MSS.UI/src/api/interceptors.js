import axios from 'axios'
// import router from '../router'

// // 添加请求拦截器
// axios.interceptors.request.use(config => {
//     return config
// }, error => {
//     return Promise.reject(error)
// })

// // 添加响应拦截器
// axios.interceptors.response.use(response => {
//     return {
//         data: response.data,
//         status: response.status,
//         statusText: response.statusText
//     }
// }, error => {
//     if (error.response.status === 502) {
//         router.push({ name: 'login' })
//     }
//     return Promise.reject(error)
// })

export default axios
