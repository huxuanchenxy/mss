
import Echarts from 'echarts'
const optionPie = {
  tooltip: {
    trigger: 'item',
    formatter: '{a}<br/>{b}:{c} ({d}%)'
  },
  legend: {
    orient: 'vertical',
    x: 'left',
    data: [ '航头站', '周浦站', '御桥站', '江浦路站', '政立路站' ],
    textStyle: {
      fontSize: '13',
      color: 'white'
    }
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
        {value: 335, name: '航头站'},
        {value: 310, name: '周浦站'},
        {value: 234, name: '御桥站'},
        {value: 135, name: '江浦路站'},
        {value: 1548, name: '政立路站'}
      ]
    }
  ]
}

// 指定图表的配置项和数据
var optionRadar = {
  title: {
    text: '',
    subtext: ''
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
    text: '业务指标', // 标题文本内容
    textStyle: {
      fontSize: '13',
      color: 'white'
    }
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
  title: {
    text: '气温指数',
    subtext: '',
    // x: '45%',
    // y: '87%',
    // textAlign: 'center',
    textStyle: {
      color: '#fff',
      fontSize: 13
    },
    subtextStyle: {
      color: '#fff',
      fontSize: 11,
      x: 'right'
    }
  },
  xAxis: {
    type: 'category',
    data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日'],
    axisLine: {
      lineStyle: {
        color: '#fff'
      }
    }
  },
  yAxis: {
    type: 'value',
    axisLine: {
      lineStyle: {
        color: '#fff'
      }
    }
  },
  series: [{
    data: [120, 200, 150, 80, 70, 110, 130],
    type: 'bar',
    textStyle: {
      fontSize: '13',
      color: 'white'
    }
  }]
}

var optionLine = {
  title: {
    text: '',
    subtext: ''
  },
  tooltip: {
    trigger: 'axis'
  },
  legend: {
    data: ['最低气温'],
    textStyle: {
      fontSize: '13',
      color: '#fff'
    },
    x: 'left'
    // y: 'bottom',
  },
  // toolbox: {
  //   show: true,
  //   feature: {
  //     mark: {show: true},
  //     dataView: {show: true, readOnly: false},
  //     magicType: {show: true, type: ['line', 'bar']},
  //     restore: {show: true},
  //     saveAsImage: {show: true}
  //   }
  // },
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

var optionHBar = {
  grid: { // 直角坐标系内绘图网格
    // left: '80', // grid 组件离容器左侧的距离,
    // // left的值可以是80这样具体像素值，
    // // 也可以是'80%'这样相对于容器高度的百分比
    // top: '80',
    // right: '0',
    // bottom: '0',
    containLabel: true // gid区域是否包含坐标轴的刻度标签。为true的时候，
    // left/right/top/bottom/width/height决定的是包括了坐标轴标签在内的
    // 所有内容所形成的矩形的位置.常用于【防止标签溢出】的场景
  },
  xAxis: { // 直角坐标系grid中的x轴,
    // 一般情况下单个grid组件最多只能放上下两个x轴,
    // 多于两个x轴需要通过配置offset属性防止同个位置多个x轴的重叠。
    type: 'value', // 坐标轴类型,分别有：
    // 'value'-数值轴；'category'-类目轴;
    // 'time'-时间轴;'log'-对数轴
    splitLine: {show: false}, // 坐标轴在 grid 区域中的分隔线
    axisLabel: {show: false}, // 坐标轴刻度标签
    axisTick: {show: false}, // 坐标轴刻度
    axisLine: {show: false} // 坐标轴轴线
  },
  yAxis: {
    type: 'category',
    axisTick: {show: false},
    axisLine: {show: false},
    axisLabel: {
      color: 'white',
      fontSize: 13
    },
    data: ['本年时间进度', '年计划完成率', '月计划完成率']// 类目数据，在类目轴（type: 'category'）中有效。
    // 如果没有设置 type，但是设置了axis.data,则认为type 是 'category'。
  },
  series: [// 系列列表。每个系列通过 type 决定自己的图表类型
    {
      name: '%', // 系列名称
      type: 'bar', // 柱状、条形图
      barWidth: 19, // 柱条的宽度,默认自适应
      data: [20, 40, 60], // 系列中数据内容数组
      label: { // 图形上的文本标签
        show: true,
        position: 'right', // 标签的位置
        // offset: [0, -40], // 标签文字的偏移，此处表示向上偏移40
        formatter: '{c}{a}', // 标签内容格式器 {a}-系列名,{b}-数据名,{c}-数据值
        color: 'white', // 标签字体颜色
        fontSize: 13 // 标签字号
      },
      itemStyle: {// 图形样式
        normal: { // normal 图形在默认状态下的样式;
          // emphasis图形在高亮状态下的样式
          barBorderRadius: 10, // 柱条圆角半径,单位px.
          // 此处统一设置4个角的圆角大小;
          // 也可以分开设置[10,10,10,10]顺时针左上、右上、右下、左下
          color: new Echarts.graphic.LinearGradient(
            0, 0, 1, 0,
            [{
              offset: 0,
              color: '#22B6ED'// 柱图渐变色起点颜色
            },
            {
              offset: 1,
              color: '#3FE279'// 柱图渐变色终点颜色
            }
            ]
          )
        }
      },
      zlevel: 1// 柱状图所有图形的 zlevel 值,
      // zlevel 大的 Canvas 会放在 zlevel 小的 Canvas 的上面
    },
    {
      name: '进度条背景',
      type: 'bar',
      barGap: '-100%', // 不同系列的柱间距离，为百分比。
      // 在同一坐标系上，此属性会被多个 'bar' 系列共享。
      // 此属性应设置于此坐标系中最后一个 'bar' 系列上才会生效，
      // 并且是对此坐标系中所有 'bar' 系列生效。
      barWidth: 19,
      data: [100, 100, 100],
      color: '#63869e', // 柱条颜色
      itemStyle: {
        normal: {
          barBorderRadius: 10
        }
      }
    }
  ]
}

export default {
  optionPie: optionPie,
  optionRadar: optionRadar,
  optionGauge: optionGauge,
  optionBar: optionBar,
  optionLine: optionLine,
  optionHBar: optionHBar
}
