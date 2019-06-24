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
    let result = ''
    str.replace(/\/\w+\((\d+)\)\//, ($1, $2) => {
        let date = new Date(+$2)
        // result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getDate()} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')}`
        result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
    })
    return result
}

// 转换内容区域中间的时间 "/Date(1523845477000)/" 有时分秒
export const transformTime = str => {
    let result = ''
    str.replace(/\/\w+\((\d+)\)\//, ($1, $2) => {
        let date = new Date(+$2)
        result = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${date.getDate()} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}:${String(date.getSeconds()).padStart(2, '0')}`
    })
    return result
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
export default{
    getUrlKey: function (name) {
        return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.href)) || null)
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

// Page
export const PageSize = 10
export const PagerCount = 5

export const ERR_SHOW = [
    '正常',
    '逻辑错误',
    '主键或名称重复',
    '数据库操作系统错误',
    '关联错误',
    '该记录不存在',
    '数量溢出',
    '密码错误',
    '没有产线',
    'lot创建错误',
    '没有创建lot或已经被操作',
    '路径编号没有对应工位',
    '工位没有匹配的设备',
    '组合进站时工位不一致',
    '不允许进站',
    '不允许出站',
    '不允许合批',
    '不允许分批',
    '没有找到对应的类',
    'Json字符串数据为空',
    '批量导入订单记录有值为空',
    '批量导入部分数据'
]
export const SUCCESS_SHOW = {
    Add: '新增成功',
    Update: '修改成功',
    Delete: '删除成功'
}
export const FORM_TITLE = {
    Add: '新增',
    Update: '修改'
}

export const RESULT = {
    Fail: 0,
    Success: 1,
    Reinsert: 2
}
