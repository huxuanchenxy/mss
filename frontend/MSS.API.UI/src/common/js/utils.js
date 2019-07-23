// 获取时间
export const getDate = (type = 'week') => {
  let date = new Date()
  let result = ''
  switch (type) {
    case 'time':
      result = {
        hours: String(date.getHours()).padStart(2, '0'),
        minutes: String(date.getMinutes()).padStart(2, '0'),
        seconds: String(date.getSeconds()).padStart(2, '0')
      }
      break
    case 'date':
      result = {
        year: date.getFullYear(),
        month: String(date.getMonth() + 1).padStart(2, '0'),
        date: String(date.getDate()).padStart(2, '0')
      }
      break
    default:
      switch (date.getDay()) {
        case 0:
          result = '星期日'
          break
        case 1:
          result = '星期一'
          break
        case 2:
          result = '星期二'
          break
        case 3:
          result = '星期三'
          break
        case 4:
          result = '星期四'
          break
        case 5:
          result = '星期五'
          break
        case 6:
          result = '星期六'
          break
      }
  }
  return result
}

// 转换内容区域中间的时间 "/Date(1523845477000)/" 无时分秒
export const transformDate = str => {
  // let result = ''
  // str.replace(/\/\w+\((\d+)\)\//, ($1, $2) => {
  //   let date = new Date(+$2)
  //   // result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getDate()} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')}`
  //   result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
  // })
  return str.replace('T', ' ')
}

// 没有时分秒
export const transformDateNoTime = str => {
  // let result = ''
  // str.replace(/\/\w+\((\d+)\)\//, ($1, $2) => {
  //   let date = new Date(+$2)
  //   // result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getDate()} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')}`
  //   result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
  // })
  if (str !== null && str !== '') {
    return str.slice(0, 10)
  }
  return ''
}
// 转换内容区域中间的时间 "/Date(1523845477000)/" 有时分秒
// export const transformTime = str => {
//   let result = ''
//   str.replace(/\/\w+\((\d+)\)\//, ($1, $2) => {
//     let date = new Date(+$2)
//     result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getDate()} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')}`
//   })
//   return result
// }

export const addDate = (date, days) => {
  var d = new Date(date)
  d.setDate(d.getDate() + days)
  var m = d.getMonth() + 1
  return d.getFullYear() + '-' + m + '-' + d.getDate()
}

// 非法字符验证
export const validateInputCommon = val => {
  val.text = val.text.replace(/\s*/g, '')
  if (val.text === '') {
    val.tips = '此项必填'
    return false
  } else if (!vInput(val.text)) {
    val.tips = '此项含有非法字符'
    return false
  } else {
    val.tips = ''
    return true
  }
}

// 数字验证
export const validateNumberCommon = val => {
  val.text = val.text.toString()
  val.text = val.text.replace(/\s*/g, '')
  if (val.text === '' || val.text === null || vNumber(val.text)) {
    val.tips = ''
    return true
  } else {
    val.tips = '此项必填数字'
    return false
  }
}

export const nullToEmpty = val => {
  return val === null ? '' : val
}

// 手机号验证
export const vPhone = str => /^1[345789]\d{9}$/.test(str)
export const vTelephone = str => /^([0-9]|[-])+$/g.test(str)
// 邮箱验证
export const vEmail = str => /^(\w+|\w+(\.\w+))+@(\w+\.)+\w+$/.test(str)

// 验证纯数字
export const vNumber = str => /^\d+$/.test(str)
// 浮点型验证
export const vdouble = str => /^(:?(:?\d+.\d+)|(:?\d+))$/.test(str)
export const vdouble3 = str => /^(:?(:?\d+.\d{1,3})|(:?\d+))$/.test(str)
// 验证非法字符
// var pattern = new RegExp("")
export const vInput = str => !/[@#$!%^&*()！￥……‘’“”：；:;""'']/.test(str)
// 不能输入中文
export const vcode = str => /[\u4E00-\u9FA5]/g.test(str)

// 全局pdf的url，api为虚拟目录，跨域使用
export const PDF_URL = process.env.NODE_ENV === 'production' ? '' : 'E:/bin/eqp/'
// 全局已上传的pdf的查看控件路径
export const PDF_UPLOADED_VIEW_URL = 'http://10.89.36.103:8090' + '/Compoment/pdfViewer/web/viewer.html?file=/'
// 全局本地准备上传还未上传或第一次完成上传的pdf的查看控件路径
// export const PDF_BLOB_VIEW_URL = process.env.NODE_ENV === 'production' ? '' : '/api' + '/Compoment/pdfViewer/web/viewer.html?file='
export const PDF_BLOB_VIEW_URL = process.env.NODE_ENV === 'production' ? '' : 'http://10.89.36.103:8090' + '/Compoment/pdfViewer/web/viewer.html?file='

// 全局url，api为虚拟目录，跨域使用
export const BASE_URL = process.env.NODE_ENV === 'production' ? '' : '/api'

// 上传文件总大小M
export const UPLOAD_SIZE = 20

// pdf 缩略图片
export const PDF_IMAGE = '/static/pdf-icon.png'
// 服务器路径
export const FILE_SERVER_PATH = 'http://10.89.36.103:8090/'

export const RESULT = {
  Fail: 0,
  Success: 1,
  Reinsert: 2
}
export const ApiRESULT = {
  Success: 0,
  Failure: 1,
  DataIsExist: 2,
  DataIsnotExist: 3,
  CheckDataRulesFail: 4,
  BindUserConflict: 5
}
export const FileType = {
  EqpType_Working_Instruction: 0,
  EqpType_Technical_Drawings: 1,
  EqpType_Installation_Manual: 2,
  EqpType_User_Guide: 3,
  EqpType_Regulations: 4,
  Eqp_Drawings: 5,
  DeviceMaintain_attach: 6,
  ExpertData_vedio: 7,
  ExpertData_attach: 8
}
