function getTable (opt) {
  var axisData = opt.xAxis[0].data
  var series = opt.series
  var tdHeaders = '<td>' + opt.xAxisTypeName + '</td>' // 表头
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
function mutiColTooltip (params) {
  var res = params[0].name
  var rows = []
  var size = 5
  for (var i = 0, l = params.length; i < l; i++) {
    rows[i % size] = rows[i % size] ? rows[i % size] + ', ' : '<br/>'
    rows[i % size] += params[i].marker + ' ' + params[i].seriesName + ' : ' + params[i].value
    // res += '<br/>' + params[i].marker + ' ' + params[i].seriesName + ' : ' + params[i].value
  }
  rows.forEach(function (item) {
    res += item
  })
  return res
}
const optionCount = {
  xAxisTypeName: '时间',
  title: {
    text: '故障次数',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
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
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: mutiColTooltip
  },
  legend: {
    data: [],
    itemWidth: 10,
    itemHeight: 7,
    itemGap: 5,
    padding: [5, 5, 0, 110],
    textStyle: {
      color: '#fff',
      fontSize: 11
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
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
      magicType: { show: true, type: ['line', 'bar', 'stack', 'tiled'] },
      myTool: {
        show: true,
        title: '返回',
        icon: `image://${backicon}`,
        onclick: this.back
      }
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
  xAxisTypeName: '时间',
  title: {
    text: '平均恢复时间（秒）',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
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
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: mutiColTooltip
  },
  legend: {
    data: [],
    itemWidth: 10,
    itemHeight: 7,
    itemGap: 5,
    padding: [5, 5, 0, 110],
    textStyle: {
      color: '#fff',
      fontSize: 11
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
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
      magicType: { show: true, type: ['line', 'bar', 'stack', 'tiled'] },
      myTool: {
        show: true,
        title: '返回',
        icon: `image://${backicon}`,
        onclick: this.back
      }
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
  xAxisTypeName: '设备类型',
  title: {
    text: '故障次数(以设备类型统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['报警次数'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          // console.info(opt)
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '设备类型',
  title: {
    text: '平均恢复时间(秒)(以设备类型统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['平均恢复时间(秒)'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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

const optionTroubleTypeCount = {
  xAxisTypeName: '故障类型',
  title: {
    text: '故障次数(以故障类型统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['报警次数'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          // console.info(opt)
          var rettable = getTable(opt)
          return rettable
        }
      }
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

const optionTroubleTypeAvg = {
  xAxisTypeName: '故障类型',
  title: {
    text: '平均恢复时间(秒)(以故障类型统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['平均恢复时间(秒)'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '供应商',
  title: {
    text: '故障次数(以供应商统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['报警次数'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '供应商',
  title: {
    text: '平均恢复时间(秒)(以供应商统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['平均恢复时间(秒)'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '制造商',
  title: {
    text: '故障次数(以制造商统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['报警次数'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '制造商',
  title: {
    text: '平均恢复时间(秒)(以制造商统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['平均恢复时间(秒)'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '子系统',
  title: {
    text: '故障次数(以子系统统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['报警次数'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '子系统',
  title: {
    text: '平均恢复时间(秒)(以子系统统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['平均恢复时间(秒)'],
    textStyle: {
      color: '#fff'
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '位置',
  title: {
    text: '故障次数(以位置信息统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['报警次数'],
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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
  xAxisTypeName: '位置',
  title: {
    text: '平均恢复时间(秒)(以位置信息统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['平均恢复时间(秒)'],
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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

const optionOrgCount = {
  xAxisTypeName: '部门',
  title: {
    text: '故障次数(以部门信息统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['报警次数'],
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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

const optionOrgAvg = {
  xAxisTypeName: '部门',
  title: {
    text: '平均恢复时间(秒)(以部门信息统计)',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'none' // 默认为直线，可选为：'line' | 'shadow'
    },
    formatter: function (params) {
      var res = params[0].name
      res += ':' + params[0].value

      return res
    }
  },
  legend: {
    show: false,
    data: ['平均恢复时间(秒)'],
    textStyle: {
      color: '#fff',
      fontSize: 12
    }
  },
  toolbox: {
    show: true,
    orient: 'vertical',
    x: 'right',
    y: '50',
    padding: [0, 30, 0, 0],
    feature: {
      dataView: {
        show: true,
        readOnly: false,
        optionToContent: function (opt) {
          var rettable = getTable(opt)
          return rettable
        }
      }
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

function getDataZoom (size) {
  var datazoom = {
    show: true,
    realtime: true,
    // type: 'inside',
    // orient: 'vertical',   // 'horizontal'
    // x: 0,
    // y: 36,
    // width: 400,
    height: 15,
    // backgroundColor: 'rgba(221,160,221,0.5)',
    // dataBackgroundColor: 'rgba(138,43,226,0.5)',
    // fillerColor: 'rgba(38,143,26,0.6)',
    // handleColor: 'rgba(128,43,16,0.8)',
    handleSize: 18,
    cursor: 'move',
    // xAxisIndex:[],
    // yAxisIndex:[],
    xAxisIndex: [0],
    start: 0,
    end: 60,
    textStyle: {
      color: '#fff',
      fontSize: 11
    }
  }
  datazoom.end = (40 / size) * 100
  return datazoom
}

// groupModel 为alarmData数据以什么字段聚合，modelName和modelID为数据中属性名（modelID为值，modelName为显示名）
// 以设备类型为例，modelID=eqpTypeID modelName=eqpTypeName。
function prepareChartData (data, groupModel, cursor) {
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
        barMaxWidth: 20,
        cursor: cursor,
        data: []
      }
      let objavg = {
        id: key,
        name: legendData[key],
        type: 'line',
        cursor: cursor,
        data: []
      }
      for (let x in xAxisData) {
        let count = 0
        let avg = 0
        for (let i = 0; i < xAxisData[x].length; ++i) {
          if (xAxisData[x][i].dimension[groupModel.modelID] === +key) {
            count = xAxisData[x][i].num
            avg = (xAxisData[x][i].avgtime / (1000)).toFixed(1)
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
          cursor: cursor,
          data: []
        }
        let dataCount = {
          name: groupModel.legend[i],
          type: 'bar',
          stack: 'test',
          barWidth: 20,
          cursor: cursor,
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
    // datazoom
    var size = optionCount.legend.data.length * optionCount.xAxis[0].data.length
    if (size > 40) {
      optionCount.dataZoom = getDataZoom(size)
    } else {
      optionCount.dataZoom = null
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
      name: '报警次数',
      type: 'bar',
      stack: 'test',
      barWidth: 20,
      cursor: 'default',
      data: []
    }
    let objavg = {
      name: '平均恢复时间(秒)',
      type: 'line',
      cursor: 'default',
      data: []
    }
    for (let i = 0; i < data.length; ++i) {
      let obj = data[i]
      if (groupby === 'troubleName') {
        xAxisData.push(obj[groupby])
      } else {
        xAxisData.push(obj.dimension[groupby])
      }

      objcount.data.push(obj.num)
      objavg.data.push((obj.avgtime / (1000)).toFixed(1))
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
  optionTroubleTypeAvg: optionTroubleTypeAvg,
  optionTroubleTypeCount: optionTroubleTypeCount,
  optionSupplierAvg: optionSupplierAvg,
  optionSupplierCount: optionSupplierCount,
  optionManufacturerAvg: optionManufacturerAvg,
  optionManufacturerCount: optionManufacturerCount,
  optionSubSystemAvg: optionSubSystemAvg,
  optionSubSystemCount: optionSubSystemCount,
  optionLocationAvg: optionLocationAvg,
  optionLocationCount: optionLocationCount,
  optionOrgAvg: optionOrgAvg,
  optionOrgCount: optionOrgCount,
  prepareChartData: prepareChartData,
  prepareSubChartData: prepareSubChartData
}
