const importantLevel = {
  1: '普通计划',
  2: '核心计划',
  3: '临时计划',
  4: '抢修计划'
}

const processState = {
  0: '未启动',
  1: '准备状态',
  2: '运行中',
  4: '已完成',
  5: '挂起',
  6: '取消',
  7: '废弃',
  8: '自然终止'
}

export default {
  importantLevel: importantLevel,
  processState: processState
}
