<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active">
        <router-link :to="{name:'SeePidCountList'}">返回</router-link>
      </x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div class="inp-wrap">
                <span class="text">车站编号<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入车站编号" v-model="nodeId.text" @keyup.native="validateInput(nodeId)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ nodeId.tips }}</p>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">车站名称<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入车站名称" v-model="nodeName.text" @keyup.native="validateInput(nodeName)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ nodeName.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">车站缩写<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入车站缩写" v-model="nodeTip.text" @keyup.native="validateInput(nodeTip)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ nodeTip.tips }}</p>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">点位容量<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入点位容量" v-model="capacityCount.text" @keyup.native="validateInput(capacityCount)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ capacityCount.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">已使用数量</span>
                <div class="inp">
                  <el-input placeholder="请输入已使用数量" v-model="usedCount.text" @keyup.native="validateInputNull(usedCount)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ usedCount.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">剩余数量</span>
                <div class="inp">
                  <el-input placeholder="请输入剩余数量" v-model="remainCount.text" @keyup.native="validateInputNull(remainCount)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ remainCount.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">预警数量<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入预警数量" v-model="remindCount.text" @keyup.native="validateInputNull(remindCount)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ remindCount.tips }}</p>
            </li>
          </ul>
        </div>
        <div class="btn-enter">
          <x-button class="close">
            <router-link :to="{ name: 'SeePidCountList' }">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="enter">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
// import { validateInputCommon, validateNumberCommon, vInput, vdouble3, PDF_BLOB_VIEW_URL, PDF_UPLOADED_VIEW_URL, nullToEmpty, FileType } from '@/common/js/utils.js'
import { validateInputCommon, validateNumberCommon, vInput, vdouble3, nullToEmpty } from '@/common/js/utils.js'
// import { dictionary, systemResource } from '@/common/js/dictionary.js'
// import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
import XButton from '@/components/button'
import api from '@/api/eqpApi'
export default {
  name: 'AddPidCount',
  components: {
    XButton
    // 'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      fileIDs: '',
      fileIDsEdit: [],
      title: '| 添加点位数量',
      loading: false,
      isShow: this.$route.params.mark,
      editObjID: this.$route.params.id,
      nodeId: {
        text: '',
        tips: ''
      },
      nodeName: {
        text: '',
        tips: ''
      },
      nodeTip: {
        text: '',
        tips: ''
      },
      capacityCount: {
        text: '',
        tips: ''
      },
      usedCount: {
        text: '',
        tips: ''
      },
      remainCount: {
        text: '',
        tips: ''
      },
      remindCount: {
        text: '',
        tips: ''
      },
      desc: {
        text: '',
        tips: ''
      }
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加记录'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改记录'
      this.getObj()
    }
  },
  methods: {
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let obj = {
        nodeId: this.nodeId.text,
        nodeName: this.nodeName.text,
        nodeTip: this.nodeTip.text,
        capacityCount: this.capacityCount.text,
        usedCount: this.usedCount.text,
        remainCount: this.remainCount.text,
        remindCount: this.remindCount.text
      }
      if (this.isShow === 'add') {
        // 添加
        api.addPidCount(obj).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeePidCountList'
                })
              }
            })
          } else {
            this.$message({
              message: res.msg === '' ? '添加失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else if (this.isShow === 'edit') {
        obj.ID = this.editObjID
        // 修改
        api.updatePidCount(obj).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeePidCountList'
                })
              }
            })
          } else {
            this.$message({
              message: res.msg === '' ? '修改失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    },
    // 修改设备时获取设备资料
    getObj () {
      api.getPidCountByID(this.editObjID).then(res => {
        this.loading = false
        let _res = res.data
        this.nodeId.text = _res.nodeId
        this.nodeName.text = _res.nodeName
        this.nodeTip.text = _res.nodeTip
        // this.teamPath.text = strToIntArr(_res.teamPath)
        this.capacityCount.text = nullToEmpty(_res.capacityCount)
        this.usedCount.text = nullToEmpty(_res.usedCount)
        this.remainCount.text = nullToEmpty(_res.remainCount)
        this.remindCount.text = nullToEmpty(_res.remindCount)
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      return validateInputCommon(val)
    },

    // 验证3位小数
    validateDouble (val) {
      val.tips = ''
      if (val.text !== '') {
        if (!vdouble3(val.text)) {
          val.tips = '此项必须为最多三位小数的浮点数'
          return false
        } else {
          return true
        }
      }
      return true
    },

    // 验证非法字符串，但允许为空
    validateInputNull (val) {
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateSelect (val) {
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        val.tips = ''
        return true
      }
    },

    validateNumberNull (val) {
      return validateNumberCommon(val)
    },
    validateNumber (val) {
      val.tips = ''
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        return validateNumberCommon(val)
      }
    },

    validateAll () {
      if (!this.validateInput(this.nodeId)) return false
      if (!this.validateInput(this.nodeName)) return false
      if (!this.validateInput(this.nodeTip)) return false
      if (!this.validateNumber(this.capacityCount)) return false
      if (!this.validateNumber(this.remainCount)) return false
      if (!this.validateNumber(this.usedCount)) return false
      if (!this.validateNumber(this.remainCount)) return false
      return true
    }
  },
  mounted () {
    // console.log(this.$refs.team.id)
    // let el = document.querySelector('.pop-display')
    // console.log(el)
    // el.style.display = 'block'
  }

}
</script>
<style lang="scss" scoped>
.pop-display{
  display: block!important;
}
// 功能区
.operation{
  .input-group{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      width: 30%;
      margin-top: PXtoEm(25);

      span{
        width: 28%;
      }

      .inp-wrap{
        display: flex;
        align-items: center;
      }

      .inp-wrap-upload{
        // display: flex;
        align-items: center;
      }

      &:nth-of-type(3n+1){
        // justify-content: flex-start;
      }

      &:nth-of-type(3n){
        // justify-content: flex-end;
      }
    }
    .upload-list{
      margin-top: PXtoEm(25);
      margin-bottom: PXtoEm(25);
      width: 50%;
    }
  }
}
.btn-enter{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
.cascader_width{
  width: 100%!important;
}
// .el-date-editor.el-input, .el-date-editor.el-input__inner{
//   width: 80%!important;
// }
.el-date-width{
  width: 93%!important;
}
.scroll{
  /**
   * percent函数转换百分比
   * $content-height内容区域总高度
   * 页面标题栏高度：56
   */
  height: percent($content-height - 56, $content-height);
  .upload-wrap{
    display: flex;
    align-items: center;
  }
  /deep/ .el-collapse-item{
    .img-list{
      margin: 20px 10px 0 0;
      cursor: pointer;
    }
  }
}
</style>
