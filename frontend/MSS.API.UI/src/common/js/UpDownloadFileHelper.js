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
  if (arr[arr.length - 1] === 'pdf') {
    return true
  } else {
    downloadFile(id, name)
    return false
  }
}
