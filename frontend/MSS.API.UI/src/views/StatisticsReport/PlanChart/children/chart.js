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
    text: '计划统计',
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

  // calculable: true,
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
      name: '完成率',
      type: 'value',
      axisLabel: {
        formatter: function (value) {
          return value + '%'
        }
      },
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
  console.log('pre:' + JSON.stringify(data))
  // let legendData = {}
  let xAxisData = {}
  if (data) {
    // for (let i = 0; i < data.length; ++i) {
    //   let obj = data[i]
    //   let groupID = obj.dimension[groupModel.modelID]
    //   if (!legendData[groupID]) {
    //     let groupName = obj.dimension[groupModel.modelName]
    //     legendData[groupID] = groupName || '其他'
    //   }
    //   if (!xAxisData[obj.date]) {
    //     xAxisData[obj.date] = []
    //   }
    //   xAxisData[obj.date].push(obj)
    // }
    let seariescount = []
    // var seriesobj = data.legend
    // legendData = ['计划', '实际完成']
    // optionCount.legend.data = Object.values(legendData)
    optionCount.legend.data = data.legend
    // xAxisData = ['2019-11-01', '2019-11-02', '2019-11-03']
    xAxisData = data.dimension
    // optionCount.xAxis[0].data = Object.keys(xAxisData)
    optionCount.xAxis[0].data = xAxisData
    // console.log('xAxisData:' + JSON.stringify(xAxisData))
    // console.log('legendData:' + JSON.stringify(legendData))
    // optionCount.series = seariescount
    // seariescount = [{'name': '计划', 'data': [50, 60, 70], 'type': 'bar'}, {'name': '实际完成', 'data': [45, 48, 65], 'type': 'bar'}]
    seariescount = data.series
    // console.log('seariescount:' + JSON.stringify(seariescount))
    seariescount.push({'name': '基准线', markLine: {'lineStyle': {'normal': {'color': '#fff'}}, 'label': {'normal': {formatter: '100%'}}, 'data': [{yAxis: '100'}]}, 'type': 'line', 'data': []})
    optionCount.series = seariescount
    if (optionCount.xAxis[0].data.length === 0) {
      optionCount.xAxis[0].data.push('无数据')
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
    for (let i = 0; i < data.length; ++i) {
      let obj = data[i]
      xAxisData.push(obj.dimension[groupby])
      objcount.data.push(obj.num)
    }
    seariescount.push(objcount)
  }
  result['xAxisData'] = xAxisData
  result['seariescount'] = seariescount
  result['seariesavg'] = seariesavg
  return result
}

export default {
  optionCount: optionCount,
  prepareChartData: prepareChartData,
  prepareSubChartData: prepareSubChartData
}
