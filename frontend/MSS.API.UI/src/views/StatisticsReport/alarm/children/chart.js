function getTable (opt) {
  var axisData = opt.xAxis[0].data
  var series = opt.series
  var tdHeaders = '<td>时间</td>' // 表头
  series.forEach(function (item) {
    tdHeaders += '<td>' + item.name + '</td>' // 组装表头
  })
  var table = '<div class="table-responsive"><table class="tableechart table-bordered table-striped table-hover" style="text-align:center"><tbody><tr style="background-color:#0f69c5;">' + tdHeaders + '</tr>'
  var tdBodys = '' // 数据
  for (let i = 0, l = axisData.length; i < l; i++) {
    for (let j = 0; j < series.length; j++) {
      tdBodys += '<td>' + series[j].data[i] + '</td>' // 组装表数据
    }
    table += '<tr><td style="padding: 0 10px">' + axisData[i] + '</td>' + tdBodys + '</tr>'
    tdBodys = ''
  }

  table += '</tbody></table></div>'
  // console.info(table)
  return table
}
const backicon = require('../images/return.svg')
const optionCount = {
  title: {
    text: '报警次数',
    subtext: '',
    x: '45%',
    y: '87%',
    textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    },
    subtextStyle: {
      color: '#fff',
      fontSize: 11,
      x: 'right'
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
    }
  },
  legend: {
    data: [],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          // console.info(opt)
          var rettable = getTable(opt)
          return rettable
        }
      },
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionAvg = {
  title: {
    text: '平均恢复时间（小时）',
    subtext: '',
    x: '45%',
    y: '87%',
    textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    },
    subtextStyle: {
      color: '#fff',
      fontSize: 11,
      x: 'right'
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'shadow' // 默认为直线，可选为：'line' | 'shadow'
    }
  },
  legend: {
    data: [],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'horizontal',
    x: 'right',
    y: 'top',
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          // console.info(opt)
          var rettable = getTable(opt)
          return rettable
        }
      },
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionEqpTypeCount = {
  title: {
    text: '报警次数(以设备类型统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionEqpTypeAvg = {
  title: {
    text: '平均恢复时间(小时)(以设备类型统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionSupplierCount = {
  title: {
    text: '报警次数(以供应商统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionSupplierAvg = {
  title: {
    text: '平均恢复时间(小时)(以供应商统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionManufacturerCount = {
  title: {
    text: '报警次数(以制造商统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionManufacturerAvg = {
  title: {
    text: '平均恢复时间(小时)(以制造商统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionSubSystemCount = {
  title: {
    text: '报警次数(以子系统统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionSubSystemAvg = {
  title: {
    text: '平均恢复时间(小时)(以子系统统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff'
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionLocationCount = {
  title: {
    text: '报警次数(以位置信息统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  series: [
  ]
}

const optionLocationAvg = {
  title: {
    text: '平均恢复时间(小时)(以位置信息统计)',
    subtext: '',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
    data: ['groupby'],
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
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
      data: [],
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLine: {
        lineStyle: {
          color: '#fff'
        }
      }
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
        type: 'line',
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
    if (optionCount.xAxis[0].data.length === 0) {
      optionCount.xAxis[0].data.push('无数据')
      optionAvg.xAxis[0].data.push('无数据')
    }
    // 补数据
    for (let i = 0; i < groupModel.legend.length; ++i) {
      let needadd = true
      for (let j = 0; j < optionCount.legend.data.length; ++j) {
        if (optionCount.legend.data[j] === groupModel.legend[i]) {
          needadd = false
          break
        }
      }
      if (needadd) {
        optionCount.legend.data.push(groupModel.legend[i])
        optionAvg.legend.data.push(groupModel.legend[i])
        let dataAvg = {
          name: groupModel.legend[i],
          type: 'line',
          data: []
        }
        let dataCount = {
          name: groupModel.legend[i],
          type: 'bar',
          stack: 'test',
          barWidth: 20,
          data: []
        }
        for (let i = 0; i < optionCount.xAxis[0].data.length; ++i) {
          dataCount.data.push(0)
          dataAvg.data.push(0)
        }

        optionCount.series.push(dataCount)
        optionAvg.series.push(dataAvg)
      }
    }
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
      type: 'line',
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
