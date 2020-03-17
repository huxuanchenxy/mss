<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeEqpRepairList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">维修设备<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-cascader class="cascader_width"
                  filterable
                  :props="defaultParams"
                  :show-all-levels="false"
                  :options="eqpList"
                  v-model="eqpSelected.text">
                </el-cascader>
              </div>
            </div>
            <p class="validate-tips">{{ eqpSelected.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">维修类型<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="pmType.text" filterable placeholder="请选择维修类型" @change="validateSelect(pmType)">
                  <el-option label="中修" value="40"/>
                  <el-option label="大修" value="41"/>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ pmType.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">更换类型<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="replaceType.text" filterable placeholder="请选择更换类型" @change="validateSelect(replaceType)">
                  <el-option label="无更换" value="0"/>
                  <el-option label="部分更换" value="157"/>
                  <el-option label="整件更换" value="205"/>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ replaceType.tips }}</p>
          </li>
          <li class="list"/>
          <!--<li class="list" v-show="false">
            <div class="inp-wrap">
              <span class="text">故障编号<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="trouble.text" filterable placeholder="请选择故障编号" @change="validateSelect(trouble)">
                  <el-option
                    v-for="item in troubleList"
                    :key="item.key"
                    :label="item.code"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ trouble.tips }}</p>
          </li>-->
        </ul>
        <div class="con-padding-horizontal cause">
          <span class="lable">过程描述<em class="validate-mark">*(500字以内)</em></span>
          <el-input type="textarea" :rows="4" v-model="desc.text" placeholder="请输入过程描述" @keyup.native="validateInputRange(desc)"></el-input>
        </div>
        <div class="upload-list con-padding-horizontal">
          <upload-pdf :systemResource="systemResource" :fileIDs="fileIDs" @getFileIDs="getFileIDs"></upload-pdf>
        </div>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeEqpRepairList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, nullToEmpty, strToIntArr } from '@/common/js/utils.js'
import { systemResource } from '@/common/js/dictionary.js'
import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
import MyUploadPDF from '@/components/UploadPDF'
import XButton from '@/components/button'
import apiMain from '@/api/DeviceMaintainRegApi.js'
import api from '@/api/eqpApi'
export default {
  name: 'AddEqpRepair',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      loading: false,
      title: '| 添加维修过程记录',
      defaultParams: {
        label: 'name',
        value: 'id',
        children: 'children'
      },
      isAllUpdate: '0',
      eqpRepairID: '',
      // 履历类型中提取这四种类型，由于部分更换比较重要，但健康度实在无法计算，所以只能分为维修类型和更换类型
      pmType: {text: '', tips: ''},
      replaceType: {text: '', tips: ''},
      // trouble: {text: '', tips: ''},
      // troubleList: [],
      desc: {text: '', tips: ''},
      eqpSelected: {text: [], tips: ''},
      eqpList: [],
      systemResource: systemResource.eqpRepair,
      fileIDs: '',
      fileIDsEdit: []
    }
  },
  created () {
    this.init()
  },
  methods: {
    getFileIDs (ids) {
      this.fileIDsEdit = ids
    },
    init () {
      this.eqpRepairID = this.$route.query.id
      // apiMain.getAllTroubleReport().then(res => {
      //   this.troubleList = res.data
      // }).catch(err => console.log(err))
      // 设备加载
      apiMain.GetEqpByTypeAndLine(0).then(res => {
        this.eqpList = res.data
      }).catch(err => console.log(err))
      if (this.$route.query.type !== 'Add') {
        this.title = '| 修改维修过程记录'
        this.loading = true
        this.getEqpRepair()
      }
    },
    getEqpRepair () {
      api.getEqpRepairByID(this.eqpRepairID).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.eqpSelected.text = strToIntArr(data.eqpPath)
          // this.trouble.text = data.trouble
          this.pmType.text = data.pmType + ''
          this.replaceType.text = data.replaceType + ''
          this.desc.text = nullToEmpty(data.desc)
          this.fileIDs = data.uploadFiles
        }
        this.loading = false
      }).catch(err => console.log(err))
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
    validateInputRange () {
      if (this.desc.text.length > 500) {
        this.desc.tips = '过程描述必须在500字以内'
        return false
      }
      return validateInputCommon(this.desc)
    },
    validateInputAll () {
      if (!this.validateInputRange() || !this.validateSelect(this.pmType) || !this.validateSelect(this.replaceType)) {
        return false
      }
      return true
    },
    save () {
      if (!this.validateInputAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      if (this.fileIDsEdit.length !== 0 && !isUploadFinished(this.fileIDsEdit)) {
        this.$message({
          message: '文件正在上传中，请耐心等待',
          type: 'warning'
        })
        return
      }
      let eqp
      let len = this.eqpSelected.text.length
      if (len < 3) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        this.eqpSelected.tips = '此项必选设备'
        return
      } else {
        eqp = this.eqpSelected.text[len - 1]
      }
      let eqpRepair = {
        eqp: eqp,
        eqpPath: this.eqpSelected.text.join(','),
        // trouble: this.trouble.text,
        desc: this.desc.text,
        pmType: this.pmType.text,
        replaceType: this.replaceType.text,
        Type: this.systemResource,
        UploadFiles: this.fileIDsEdit.length === 0 ? '' : JSON.stringify(this.fileIDsEdit)
      }
      if (this.$route.query.type === 'Add') {
        api.addEqpRepair(eqpRepair).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeEqpRepairList'})
            this.$message({
              message: '添加成功',
              type: 'success'
            })
          } else {
            this.$message({
              message: res.msg === '' ? '添加失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else {
        eqpRepair.ID = this.eqpRepairID
        api.updateEqpRepair(eqpRepair).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeEqpRepairList'})
            this.$message({
              message: '修改成功',
              type: 'success'
            })
          } else {
            this.$message({
              message: res.msg === '' ? '修改失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    }
  }
}
</script>
<style lang="scss" scoped>
// 显示大图容器
.el-dialog__wrapper{
  width: 100%;
  height: 100%;

  .el-carousel{
    height: 100%;
  }
}

.header{
  display: flex;
  justify-content: space-between;
}

// 顶部信息
.middle{
  position: relative;
  margin-bottom: 10px;
  padding-bottom: 20px;

  .text-right{
    text-align: right !important;
  }

  [class*="list-wrap"]{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      margin-top: 20px;
      padding: 0 2%;
      border-right: 1px solid #C9CACD;
      text-align: center;

      &:first-of-type{
        padding-left: 0;
        text-align: left;
      }

      &:last-of-type{
        padding-right: 0;
        border: 0;
        text-align: right;
      }
    }
  }

  .list-wrap{
    .list{
      width: 16%;

      span{
        display: inline-block;
        width: 100%;
        @extend %ellipsis;
      }
    }
  }

  .sub-list-wrap{
    .list{
      margin-right: 40px;
      padding: 0;
      border: 0;
      text-align: left;
    }

    .text{
      color: $color-content-text;
    }

    &:after{
      content: "";
      flex: auto;
    }
  }
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

.input-group{
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;

  .list{
    width: 30%;
    margin-top: PXtoEm(20);

    .inp-wrap{
      display: flex;
      align-items: center;
    }

    .text{
      width: 28%;
    }

    &:nth-of-type(3n+1){
      justify-content: flex-start;
    }

    &:nth-of-type(3n){
      justify-content: flex-end;
    }
  }
}
.cause{
  display: flex;
  margin-top: 20px;
  align-items: center;

  .el-textarea{
    flex: 1;
    width: auto;
  }
}

// 列表
.list-bottom-wrap{
  margin-top: 10px;

  .list{
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    // @extend .con-padding-horizontal;
    background: rgba(186,186,186,.1);
    text-align: center;

    &:nth-of-type(odd){
      background: rgba(128,128,128,.1);
    }

    &.list-header{
      height: 50px;
      background: $color-content-table-header;
    }

    &:not(.list-header){
      padding: {
        top: 20px;
        bottom: 20px;
      }
    }
  }

  .list-con{
    display: flex;
    align-items: center;
    justify-content: center;
    width: 20%;

    &:first-of-type{
      justify-content: flex-start;
    }
  }

  .textarea-wrap{
    width: 100%;
    margin-top: 10px;
  }
}

// 提交底部按钮
.btn-group{
  padding: 20px 0;
  text-align: center;
}

.lable{
  width: 100px;
}

.disabled{
  background: #8c939d;
}

.upload-list{
  margin-top: PXtoEm(25);
  margin-bottom: PXtoEm(25);
  width: 50%;
}
.left{
  text-indent: 9.5%
}
.cause{
  display: flex;
  margin-top: 20px;
  align-items: center;

  .el-textarea{
    flex: 1;
    width: auto;
  }
}
</style>
