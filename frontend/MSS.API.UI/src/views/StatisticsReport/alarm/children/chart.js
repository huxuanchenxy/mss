const backicon = require('../images/icon-user.svg')
const optionCount = {
  title: {
    text: '报警次数',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
    }
  },
  legend: {
    data: []
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      myTool: {
        show: true,
        title: '返回',
        icon: `image://${backicon}`,
        onclick: this.back
      },
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionAvg = {
  title: {
    text: '平均恢复时间（小时）',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
    }
  },
  legend: {
    data: []
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      myTool: {
        show: true,
        title: '返回',
        icon: `image://${backicon}`,
        onclick: this.back
      },
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionEqpTypeCount = {
  title: {
    text: '报警次数(以设备类型统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionEqpTypeAvg = {
  title: {
    text: '平均恢复时间(小时)(以设备类型统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionSupplierCount = {
  title: {
    text: '报警次数(以供应商统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionSupplierAvg = {
  title: {
    text: '平均恢复时间(小时)(以供应商统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionManufacturerCount = {
  title: {
    text: '报警次数(以制造商统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionManufacturerAvg = {
  title: {
    text: '平均恢复时间(小时)(以制造商统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionSubSystemCount = {
  title: {
    text: '报警次数(以子系统统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionSubSystemAvg = {
  title: {
    text: '平均恢复时间(小时)(以子系统统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionLocationCount = {
  title: {
    text: '报警次数(以位置信息统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

const optionLocationAvg = {
  title: {
    text: '平均恢复时间(小时)(以位置信息统计)',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['groupby']
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      saveAsImage: { show: true }
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
  ]
}

// groupModel 为alarmData数据以什么字段聚合，modelName和modelID为数据中属性名（modelID为值，modelName为显示名）
// 以设备类型为例，modelID=eqpTypeID modelName=eqpTypeName。
function prepareChartData (data, groupModel) {
  let legendData = {}
  let xAxisData = {}
  if (data) {
    for (let i = 0; i < data.length; ++i) {
      let obj = data[i]
      let groupID = obj.dimension[groupModel.modelID]
      if (!legendData[groupID]) {
        let groupName = obj.dimension[groupModel.modelName]
        legendData[groupID] = groupName || '其他'
      }
      if (!xAxisData[obj.date]) {
        xAxisData[obj.date] = []
      }
      xAxisData[obj.date].push(obj)
    }
    let seariescount = []
    let seariesavg = []
    for (let key in legendData) {
      let objcount = {
        id: key,
        name: legendData[key],
        type: 'bar',
        stack: 'test',
        barWidth: 20,
        data: []
      }
      let objavg = {
        id: key,
        name: legendData[key],
        type: 'bar',
        stack: 'test',
        barWidth: 20,
        data: []
      }
      for (let x in xAxisData) {
        let count = 0
        let avg = 0
        for (let i = 0; i < xAxisData[x].length; ++i) {
          if (xAxisData[x][i].dimension[groupModel.modelID] === +key) {
            count = xAxisData[x][i].num
            avg = (xAxisData[x][i].avgtime / (1000 * 60 * 60)).toFixed(1)
            break
          }
        }
        objcount.data.push(count)
        objavg.data.push(avg)
      }
      seariescount.push(objcount)
      seariesavg.push(objavg)
    }
    optionCount.legend.data = Object.values(legendData)
    optionCount.xAxis[0].data = Object.keys(xAxisData)
    optionCount.series = seariescount
    optionAvg.legend.data = Object.values(legendData)
    optionAvg.xAxis[0].data = Object.keys(xAxisData)
    optionAvg.series = seariesavg
  }
}

function prepareSubChartData (data, groupby) {
  let xAxisData = []
  let seariescount = []
  let seariesavg = []
  let result = {}
  if (data) {
    let objcount = {
      name: 'groupby',
      type: 'bar',
      stack: 'test',
      barWidth: 20,
      data: []
    }
    let objavg = {
      name: 'groupby',
      type: 'bar',
      stack: 'test',
      barWidth: 20,
      data: []
    }
    for (let i = 0; i < data.length; ++i) {
      let obj = data[i]
      xAxisData.push(obj.dimension[groupby])
      objcount.data.push(obj.num)
      objavg.data.push((obj.avgtime / (1000 * 60 * 60)).toFixed(1))
    }
    seariescount.push(objcount)
    seariesavg.push(objavg)
  }
  result['xAxisData'] = xAxisData
  result['seariescount'] = seariescount
  result['seariesavg'] = seariesavg
  return result
}

export default {
  optionCount: optionCount,
  optionAvg: optionAvg,
  optionEqpTypeAvg: optionEqpTypeAvg,
  optionEqpTypeCount: optionEqpTypeCount,
  optionSupplierAvg: optionSupplierAvg,
  optionSupplierCount: optionSupplierCount,
  optionManufacturerAvg: optionManufacturerAvg,
  optionManufacturerCount: optionManufacturerCount,
  optionSubSystemAvg: optionSubSystemAvg,
  optionSubSystemCount: optionSubSystemCount,
  optionLocationAvg: optionLocationAvg,
  optionLocationCount: optionLocationCount,
  prepareChartData: prepareChartData,
  prepareSubChartData: prepareSubChartData
}
