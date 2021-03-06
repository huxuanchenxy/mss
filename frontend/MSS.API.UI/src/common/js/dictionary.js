export const dictionary = {
  actionGroupType: 5,
  subSystem: 17,
  firmType: 22,
  sparePartsType: 59,
  currency: 94,
  troubleType: 51,
  workType: 111,
  pmType: 119,
  troubleStatus: 47,
  troubleLevel: 126,
  pmStatus: 176,
  pmMajor: 180,
  pmLevel: 185,
  troubleChartType: 191,
  healthType: 198
}

export const troubleOperation = {
  newTrouble: 141,
  canceleTrouble: 142,
  assign: 143,
  delayed: 144,
  repost: 145,
  repairReject: 146,
  pass: 147,
  unpass: 148,
  assignReject: 149,
  updateTrouble: 150,
  prepass: 174,
  unprepass: 175
}

export const troubleStatus = {
  newTrouble: 48,
  processing: 49,
  repaired: 50,
  delayed: 133,
  finished: 134,
  canceled: 135,
  pendingApproval: 89
}

export const firmType = {
  supplier: 23,
  manufacturer: 24
}

export const systemResource = {
  eqpType: 25,
  eqp: 26,
  expert: 27,
  maintainReg: 28,
  emergencyPlan: 57,
  regulation: 58,
  construction: 109,
  troubleReport: 136,
  eqpRepair: 190
}

export const sparePartsOperationType = {
  receive: 68,
  delivery: 69,
  adjust: 70,
  move: 71
}

export const sparePartsOperationDetailType = {
  purchaseReturn: 72,
  purchaseReceive: 73,
  otherReceive: 84,
  distribution: 74,
  materialReturn: 75,
  moveTo: 76,
  inventoryProfit: 77,
  inventoryLoss: 78,
  troubleReturn: 79,
  troubleRepair: 80,
  materialLend: 81,
  troubleMoveTo: 82,
  inspection: 85,
  inspectionReturn: 86,
  troubleScrap: 87,
  inStockScrap: 88,
  lendReturn: 92,
  repairReceive: 93,
  moveLocation: 171,
  troubleMoveLocation: 172
}

export const orgType = {
  company: 1,
  department: 2,
  team: 3
}

export const pmStatus = {
  init: 177,
  editing: 178,
  finished: 179
}

export const eqpHistoryType = {
  initall: 39,
  mediumPM: 40,
  majorPM: 41,
  troublePM: 46,
  firstWork: 43,
  secondWork: 44,
  change: 157,
  maintenance: 189
}

export const healthType = {
  trouble: 199,
  pm: 200,
  time: 201,
  mediumPM: 202,
  majorPM: 203,
  eqpReplace: 204
}
