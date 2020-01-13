import api from '@/api/eqpApi'
import { Message } from 'element-ui'
export const downloadFile = (id, fileName) => {
  api.downloadFile(id).then(res => {
    if (res.data.size !== 0) {
      let aTag = document.createElement('a')
      let blob = new Blob([res.data])
      aTag.download = fileName
      aTag.href = URL.createObjectURL(blob)
      aTag.click()
      URL.revokeObjectURL(blob)
    } else {
      Message({
        message: '未找到所要下载的文件',
        type: 'error'
      })
    }
  })
}

export const isUploadFinished = fileIDsEdit => {
  return fileIDsEdit.some(item => {
    let arr = item.ids.split(',')
    return arr.some(me => {
      if (me === '') {
        Message({
          message: '文件还未上传完成，请耐心等待',
          type: 'warning'
        })
        return false
      }
      return true
    })
  })
}

export const isPreview = (id, name) => {
  let arr = name.split('.')
  let ext = arr[arr.length - 1].toLowerCase()
  if (ext === 'pdf' || ext === 'mp4' || ext === 'avi' || ext === 'flv' || ext === 'rmvb' || ext === 'ogg') {
    return true
  } else {
    downloadFile(id, name)
    return false
  }
}

/**
 * 判断服务的文件是否存在
 * @param filepath 文件地址  即目录名
 * @returns {Boolean}  文件名
 */
export const isExistFile = (filepath) => {
  let xmlhttp
  if (filepath === null || filepath === undefined || filepath === '') {
    return false
  }
  if (window.XMLHttpRequest) {
    xmlhttp = new XMLHttpRequest()
  }
  debugger
  xmlhttp.open('GET', filepath, false)
  xmlhttp.send()
  if (xmlhttp.readyState === 4) {
    if (xmlhttp.status === 200) return true
    else if (xmlhttp.status === 404) return false
    else return false
  }
}
