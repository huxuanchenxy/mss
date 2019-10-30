<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <el-tabs class="tab-height" v-model="activeName">
        <el-tab-pane class="pane-height pane-notification" label="设备类型" name="eqpType">
          <div class="box">
            <!-- 搜索框 -->
            <div class="con-padding-horizontal search-wrap">
              <div class="wrap">
                <div class="input-group">
                  <label for="name">设备类型</label>
                  <div class="inp">
                    <el-select v-model="eqpTypeOnly" filterable placeholder="请选择" @change="eqpTypeOnlyChange">
                      <el-option
                        v-for="item in eqpTypeList"
                        :key="item.key"
                        :label="item.tName"
                        :value="item.id">
                      </el-option>
                    </el-select>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="scroll">
            <el-scrollbar>
              <upload-pdf class="upload-list"
              :readOnly="readOnly"
              :systemResource="systemResource"
              :fileIDs="eqpTypeFileIDs"
              :unSelectedEntity="unSelectedEqpType"
              @getFileIDs="getFileIDs">
              </upload-pdf>
              <!-- 按钮 -->
              <div class="btn-group">
                <el-button type="primary" class="btn" v-show="btnSave" @click.native="saveEqpType" :loading="saveEqpTypeLoading">保存</el-button>
              </div>
            </el-scrollbar>
          </div>
        </el-tab-pane>
        <el-tab-pane class="pane-height pane-notification" label="设备" name="eqp">
          <div class="box">
            <!-- 搜索框 -->
            <div class="con-padding-horizontal search-wrap">
              <div class="wrap">
                <div class="input-group">
                  <label for="name">设备类型</label>
                  <div class="inp">
                    <el-select v-model="eqpType" filterable placeholder="请选择" @change="eqpTypeChange">
                      <el-option
                        v-for="item in eqpTypeList"
                        :key="item.key"
                        :label="item.tName"
                        :value="item.id">
                      </el-option>
                    </el-select>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">设备</label>
                  <div class="inp">
                    <el-cascader class="cascader_width"
                      placeholder="请先选择设备类型"
                      expand-trigger="hover"
                      :props="defaultParams"
                      :show-all-levels="false"
                      :options="eqpList"
                      @change="eqpChange"
                      v-model="eqpSelected">
                    </el-cascader>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="scroll">
            <el-scrollbar>
              <upload-pdf class="upload-list" :readOnly="readOnly" :systemResource="systemResource" :fileIDs="fileIDs" :unSelectedEntity="unSelectedEqp" @getFileIDs="getFileIDs"></upload-pdf>
              <!-- 按钮 -->
              <div class="btn-group">
                <el-button type="primary" class="btn" v-show="btnSave" @click.native="saveEqp" :loading="saveEqpLoading">保存</el-button>
              </div>
            </el-scrollbar>
          </div>
        </el-tab-pane>
      </el-tabs>
    </div>
  </div>
</template>
<script>
import { ApiRESULT } from '@/common/js/utils.js'
import { systemResource } from '@/common/js/dictionary.js'
import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import apiMainTain from '@/api/DeviceMaintainRegApi.js'
import api from '@/api/eqpApi'
import MyUploadPDF from '@/components/UploadPDF'
export default {
  name: 'AddTechnicalData',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      title: ' | 技术资料维护',
      defaultParams: {
        label: 'name',
        value: 'id',
        children: 'children'
      },
      loading: false,
      unSelectedEqp: systemResource.eqp,
      saveEqpLoading: false,
      systemResource: systemResource.eqpType,
      fileIDs: '',
      fileIDsEdit: [],
      eqpType: '',
      eqpTypeList: [],
      eqp: '',
      eqpSelected: [],
      eqpList: [],
      activeName: 'eqpType',

      unSelectedEqpType: systemResource.eqpType,
      saveEqpTypeLoading: false,
      eqpTypeFileIDs: '',
      eqpTypeFileIDsEdit: [],
      eqpTypeOnly: '',

      btnSave: true,
      readOnly: false
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btnSave = actions.some((item, index) => {
        return item.actionID === btn.emergencyPlan.save
      })
      if (!this.btnSave) this.readOnly = true
    }
    // 设备类型加载
    api.getEqpTypeAll().then(res => {
      this.eqpTypeList = res.data
      this.eqpType = this.eqpTypeList[0].id
      // 设备加载
      apiMainTain.GetEqpByTypeAndLine(this.eqpType).then(res => {
        this.eqpList = res.data
      }).catch(err => console.log(err))
    }).catch(err => console.log(err))
  },
  watch: {
    $route () {
      // this.activeName = this.$route.params.type
    },
    activeName () {
      if (this.activeName === 'eqp') {
        this.systemResource = systemResource.eqp
      } else {
        this.systemResource = systemResource.eqpType
      }
    }
  },
  methods: {
    getFileIDs (ids) {
      if (this.activeName === 'eqp') {
        this.fileIDsEdit = ids
      } else {
        this.eqpTypeFileIDsEdit = ids
      }
    },
    eqpTypeChange () {
      this.eqpSelected = []
      apiMainTain.GetEqpByTypeAndLine(this.eqpType).then(res => {
        this.eqpList = res.data
        this.unSelectedEqp = systemResource.eqp
      }).catch(err => console.log(err))
    },
    eqpChange () {
      this.eqp = this.eqpSelected[this.eqpSelected.length - 1]
      this.loading = true
      api.getEqpByID(this.eqp).then(res => {
        this.loading = false
        let _res = res.data
        if (_res !== null) {
          this.fileIDs = _res.fileIDs
        } else {
          this.fileIDs = ''
        }
        this.unSelectedEqp = 0
      }).catch(err => console.log(err))
    },
    saveEqp () {
      if (this.eqpSelected.length === 0) {
        this.$message({
          message: '请选择设备',
          type: 'warning'
        })
        return
      }
      if (!this.validateAll()) return
      this.saveEqpLoading = true
      let obj = {
        entity: this.eqp,
        systemResource: this.systemResource,
        files: JSON.stringify(this.fileIDsEdit)
      }
      api.addUploadFileRelation(obj).then(res => {
        this.saveEqpLoading = false
        if (res.code === ApiRESULT.Success) {
          this.$message({
            message: '保存成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.msg === '' ? '保存失败' : res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },

    eqpTypeOnlyChange () {
      this.loading = true
      api.getEqpTypeByID(this.eqpTypeOnly).then(res => {
        this.loading = false
        let _res = res.data
        if (_res !== null) {
          this.eqpTypeFileIDs = _res.uploadFiles
        } else {
          this.eqpTypeFileIDs = ''
        }
        this.unSelectedEqpType = 0
      }).catch(err => console.log(err))
    },
    saveEqpType () {
      if (this.eqpTypeOnly === '') {
        this.$message({
          message: '请选择设备类型',
          type: 'warning'
        })
        return
      }
      if (!this.validateAll()) return
      this.saveEqpTypeLoading = true
      let obj = {
        entity: this.eqpTypeOnly,
        systemResource: systemResource.eqpType,
        files: JSON.stringify(this.eqpTypeFileIDsEdit)
      }
      api.addUploadFileRelation(obj).then(res => {
        this.saveEqpTypeLoading = false
        if (res.code === ApiRESULT.Success) {
          this.$message({
            message: '保存成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.msg === '' ? '保存失败' : res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    validateAll () {
      if (this.fileIDsEdit.length === 0) {
        this.$message({
          message: '请修改后进行保存操作',
          type: 'warning'
        })
        return false
      } else if (this.fileIDsEdit.length !== 0 && !isUploadFinished(this.fileIDsEdit)) {
        this.$message({
          message: '文件正在上传中，请耐心等待',
          type: 'warning'
        })
        return false
      }
      return true
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 56;
// 内容区
.content-wrap{
  // overflow: hidden;
  height: percent($con-height, $content-height);
  // text-align: center;
  .content-header{
    flex-wrap: wrap;
    display: flex;
    justify-content: space-between;
    // align-items: center;
    height: 30px;
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }
  .tab-height{
    height: percent($con-height, $con-height);
  }
  /deep/ .el-tabs__header{
    margin-left: 10px!important;
    height: percent(50, $con-height)
  }
  /deep/ .el-tabs__content{
    overflow: hidden;
    height: percent($con-height - 50, $con-height)
  }
  .pane-height{
    height: 100%
  }
  .pane-notification{
    .content-header{
      height: percent(50, $con-height)
    }
  }
  .scroll{
    height: percent($con-height - 50, $con-height)
  }
  .el-scrollbar__thumb{
    z-index: 99!important;
    background: #1b7ec9!important;
  }
  .list-wrap{
    .list{
      &:nth-of-type(even){
        .list-content{
          background: rgba(186,186,186,.5);
        }
      }
    }

    .list-content{
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: PXtoEm(15) PXtoEm(24);

      div{
        word-break: break-all;
      }
    }

    .left-title{
      margin-right: 10px;
      font-weight: bold;
    }

    // 隐藏内容
    .sub-content{
      overflow: hidden;
      height: 0;
      font-size: $font-size-small;
      text-align: left;
      color: $color-content-text;

      &.active{
        overflow: inherit;
        height: auto;
        transition: .7s .2s;
      }
    }

    .sub-con-list{
      display: flex;
      padding: PXtoEm(15) PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);

      .right-wrap{
        display: flex;
        flex-wrap: wrap;
      }

      .list{
        margin-right: 10px;
      }
    }
  }

  .number,
  .name,
  .btn-wrap{
    width: 10%;
  }
  .content{
    width: 15%;
  }
  /deep/ .el-checkbox__label{
    display: none;
  }

  .name{
    a{
      color: #42abfd;
    }
  }

  .last-update-time{
    width: 18%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .state{
    width: 5%;
  }

}

.upload-list{
  float: left;
  margin-left: PXtoEm(25);
  margin-top: PXtoEm(25);
  margin-bottom: PXtoEm(25);
  width: 430px;
}

.btn-group{
  margin-top: PXtoEm(25);
  float: left;
  display: flex;
  align-items: center;
  height: percent(65, 145);
  /deep/
  .btn{
    &:hover{
      border-color: $color-main-btn!important;
      background: $color-main-btn!important;
    }
  }
}
</style>
