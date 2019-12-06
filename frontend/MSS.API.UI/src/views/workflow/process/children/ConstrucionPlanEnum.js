const importantLevel = {
  1: '普通计划',
  2: '核心计划',
  3: '临时计划',
  4: '抢修计划'
}

const conPlanType = {
  1: '抢修施工',
  2: '故障处理',
  3: '配合施工',
  4: '临时任务',
  5: '重大施工',
  6: '其他施工'
}

const conPlanTypeMenu = {
  115: 1,
  153: 2,
  154: 3,
  155: 4,
  156: 5,
  157: 6
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
  processState: processState,
  conPlanType: conPlanType,
  conPlanTypeMenu: conPlanTypeMenu
}
