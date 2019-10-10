
const optionPie = {
  tooltip: {
    trigger: 'item',
    formatter: '{a}<br/>{b}:{c} ({d}%)'
  },
  legend: {
    orient: 'vertical',
    x: 'left',
    data: [ '马当路', '新天地', '人民广场', '江浦路', '陆家浜路' ]
  },
  series: [
    {
      name: '设备报警数量',
      type: 'pie',
      radius: ['50%', '70%'],
      avoidLabelOverlap: false,
      label: {
        normal: {
          show: false,
          position: 'center'
        },
        emphasis: {
          show: true,
          textStyle: {
            fontSize: '30',
            fontWeight: 'blod'
          }
        }
      },
      labelLine: {
        normal: {
          show: false
        }
      },
      data: [
        {value: 335, name: '马当路'},
        {value: 310, name: '新天地'},
        {value: 234, name: '人民广场'},
        {value: 135, name: '江浦路'},
        {value: 1548, name: '陆家浜路'}
      ]
    }
  ]
}

// 指定图表的配置项和数据
var optionRadar = {
  title: {
    text: '',
    subtext: '纯属虚构'
  },
  tooltip: {
    trigger: 'axis'
  },
  legend: {
    orient: 'vertical',
    x: 'right',
    y: 'bottom',
    data: ['报警数', '故障数']
  },
  toolbox: {
    show: true,
    feature: {
      mark: {show: true},
      dataView: {show: true, readOnly: false},
      restore: {show: true},
      saveAsImage: {show: true}
    }
  },
  polar: [
    {
      indicator: [
        // eslint-disable-next-line standard/object-curly-even-spacing
        { text: '风机', max: 6000},
        // eslint-disable-next-line standard/object-curly-even-spacing
        { text: '水泵', max: 16000},
        // eslint-disable-next-line standard/object-curly-even-spacing
        { text: '烟感', max: 30000},
        // eslint-disable-next-line standard/object-curly-even-spacing
        { text: '空调', max: 38000},
        // eslint-disable-next-line standard/object-curly-even-spacing
        { text: '空气质量', max: 52000},
        // eslint-disable-next-line standard/object-curly-even-spacing
        { text: '信号', max: 25000}
      ],
      splitArea: {
        show: true,
        areaStyle: {
          // eslint-disable-next-line standard/array-bracket-even-spacing
          // color: ['#d72941', '#f1c35b', '#99c736', '#75813b', '#4e3829']
          // 图表背景网格的颜色
        }
      },
      splitLine: {
        show: true,
        lineStyle: {
          width: 1,
          color: ['#7954a1', '#ef91ab']
          // 图表背景网格线的颜色
        }
      }
    }
  ],
  calculable: true,
  series: [
    {
      name: '预算 vs 开销（Budget vs spending）',
      type: 'radar',
      data: [
        {
          value: [4300, 10000, 28000, 35000, 50000, 19000],
          name: '报警数'
        },
        {
          value: [5000, 14000, 28000, 31000, 42000, 21000],
          name: '故障数'
        }
      ]
    }
  ]
}

var optionGauge = {
  title: {
    text: '业务指标' // 标题文本内容
  },
  toolbox: { // 可视化的工具箱
    show: true,
    feature: {
      restore: { // 重置
        show: true
      },
      saveAsImage: {// 保存图片
        show: true
      }
    }
  },
  tooltip: { // 弹窗组件
    formatter: '{a} <br/>{b} : {c}%'
  },
  series: [{
    name: '业务指标',
    type: 'gauge',
    detail: {formatter: '{value}%'},
    data: [{value: 45, name: '完成率'}]
  }]

}

var optionBar = {
  xAxis: {
    type: 'category',
    data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
  },
  yAxis: {
    type: 'value'
  },
  series: [{
    data: [120, 200, 150, 80, 70, 110, 130],
    type: 'bar'
  }]
}

var optionLine = {
  title: {
    text: '未来一周气温变化',
    subtext: '纯属虚构'
  },
  tooltip: {
    trigger: 'axis'
  },
  legend: {
    data: ['最低气温']
  },
  toolbox: {
    show: true,
    feature: {
      mark: {show: true},
      dataView: {show: true, readOnly: false},
      magicType: {show: true, type: ['line', 'bar']},
      restore: {show: true},
      saveAsImage: {show: true}
    }
  },
  calculable: true,
  xAxis: [
    {
      type: 'category',
      boundaryGap: false,
      data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
    }
  ],
  yAxis: [
    {
      type: 'value',
      axisLabel: {
        formatter: '{value} °C'
      }
    }
  ],
  series: [

    {
      name: '标准气温',
      type: 'line',
      data: [6, 6, 6, 6, 6, 6, 6], // 标准线效果
      markLine: {
        data: [
          {type: 'average', name: '平均值'}
        ]
      }

    },
    {
      name: '最低气温',
      type: 'line',
      data: [1, -2, 2, 5, 3, 2, 0],
      markPoint: {
        data: [
          {name: '周最低', value: -2, xAxis: 1, yAxis: -1.5}
        ]
      },
      markLine: {
        data: [
          {type: 'average', name: '平均值'}
        ]
      }
    }
  ]
}

export default {
  optionPie: optionPie,
  optionRadar: optionRadar,
  optionGauge: optionGauge,
  optionBar: optionBar,
  optionLine: optionLine
}
