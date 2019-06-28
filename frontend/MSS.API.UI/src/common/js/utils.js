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

// 系统监控根据参数获取对应的名称
export const getSysControlName = val => {
  let res = ''
  switch (val) {
    case 'main':
      res = '管廊总貌图'
      break
    case 'environment':
      res = '环境监控'
      break
    case 'fas':
      res = '消防监控'
      break
    case 'pump':
      res = '排水监控'
      break
    case 'fan':
      res = '通风监控'
      break
    case 'cover':
      res = '井盖监控'
      break
    case 'telephone':
      res = '电话监控'
      break
    case 'power':
      res = '配电柜监控'
      break
    case 'lighting':
      res = '照明监控'
      break
    case 'UPS':
      res = 'UPS监控'
      break
    case 'EPS':
      res = 'EPS监控'
      break
    case 'emergencylighting':
      res = '应急照明监控'
      break
    case 'intrusion':
      res = '入侵报警监控'
      break
  }
  return res
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
  val.text = val.text.replace(/\s*/g, '')
  if (val.text === '' || val.text === null || vNumber(val.text)) {
    val.tips = ''
    return true
  } else {
    val.tips = '此项必填数字'
    return false
  }
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
// 验证非法字符
// var pattern = new RegExp("")
export const vInput = str => !/[@#$!%^&*()！￥……‘’“”：；:;""'']/.test(str)
// 不能输入中文
export const vcode = str => /[\u4E00-\u9FA5]/g.test(str)

// 任务中管廊、分区、设备类型、设备的常显示长度，超出此长度，则启用tip提示
export const TASK_TIP_LENGTH = 12

// 任务类型
export const TASKTYPE = 'TaskType'

// 巡检
export const TASK_TYPE_RI = 'RoutingInspect'
// 抢修
export const TASK_TYPE_UR = 'UrgentRepair'
// 维修
export const TASK_TYPE_RR = 'RegularRepair'
// 专项
export const TASK_TYPE_SI = 'SpecialInspect'

// 任务状态
export const TASKSTATUS = 'TaskStatus'

// 任务状态-未开始
export const TASKSTATUS_NOTSTART = 'NotStart'

// 任务状态-未结束
export const TASKTYPE_STARTING = 'Starting'

// 任务状态-待确认
export const TASKTYPE_REPAIRED = 'Repaired'

// 任务状态-已完成
export const TASKTYPE_FINISHED = 'Finished'

// 维修申请状态
export const MATYPE = 'MAType'

// 维修申请状态-未处理
export const MATYPE_UNDO = 'Undo'

// 维修申请状态-同意维修申请
export const MATYPE_AGREE = 'Agree'

// 维修申请状态-不同意维修申请
export const MATYPE_DISAGREE = 'Disagree'

// 全局url，api为虚拟目录，跨域使用
export const BASE_URL = process.env.NODE_ENV === 'production' ? '' : '/api'

// 上传文件总大小M
export const UPLOAD_SIZE = 20

// pdf 缩略图片
export const PDF_IMAGE = '/Main/static/pdf-icon.png'

export const RESULT = {
  Fail: 0,
  Success: 1,
  Reinsert: 2
}
export const ApiRESULT = {
  Success: 0,
  Failure: 1,
  DataIsExist: 2,
  DataIsnotExist: 3
}
