const backicon = require('../images/icon-user.svg')
const optionCount = {
  title: {
    text: '报警次数',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis',
    axisPointer: { // 坐标轴指示器，坐标轴触发有效
      type: 'line' // 默认为直线，可选为：'line' | 'shadow'
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

export default {
  optionCount: optionCount,
  optionAvg: optionAvg,
  prepareChartData: prepareChartData
}
