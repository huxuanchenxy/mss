<template>
  <div class="wrap height-full"
       v-loading="loading"
       element-loading-text="加载中"
       element-loading-spinner="el-icon-loading">
    <div ref="header"
         class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive"
             alt=""
             class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active">
        <router-link :to="{ name: 'DeviceMaintainList' }">返回</router-link>
      </x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">设备类别<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-cascader clearable
                             change-on-select
                             :show-all-levels="true"
                             :options="deviceTypeList"
                             v-model="deviceType.text">
                </el-cascader>
              </div>
            </div>
            <p class="validate-tips">{{ deviceType.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">负责班组<em class="validate-mark">*</em></span>
              <div class="inp">
                <!-- <el-select v-model="team_group.text"
                           clearable
                           placeholder="请选择">
                  <option disabled
                          value=""
                          selected>请选择</option>
                  <el-option v-for="item in TeamGroupList"
                             :key="item.key"
                             :value="item.id"
                             :label="item.teamGroupName">
                  </el-option>
                </el-select> -->
                 <el-cascader class="cascader_width" clearable
                    :props="defaultParams"
                    change-on-select
                    @change="cascader_change_copy"
                    :show-all-levels="true"
                    :options="teamList"
                    v-model="teamPath.text">
                  </el-cascader>
              </div>
            </div>
            <p class="validate-tips">{{ teamPath.tips }}</p>
          </li>
        </ul>
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">维护负责人<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="director.text"
                           clearable
                           placeholder="请选择">
                  <option disabled
                          value=""
                          selected>请选择</option>
                  <el-option v-for="item in directorList"
                             :key="item.key"
                             :value="item.id"
                             :label="item.directorName">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ director.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">维护日期<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-date-picker v-model="maintain_date.text"
                                type="date"
                                @keyup.native="validateInput(maintain_date.text)"
                                prefix-icon="el-icon-date"
                                :unlink-panels="true"
                                placeholder="选择日期"
                                value-format="yyyy-MM-dd">
                </el-date-picker>
              </div>
            </div>
          </li>
        </ul>
        <div class="con-padding-horizontal cause">
          <span class="text">备件更换过程记录<em class="validate-mark">*</em></span>
          <el-input type="textarea"
                    v-model="detail_desc.text"
                    placeholder="请输入内容"></el-input>
          <p class="validate-tips">{{ detail_desc.tips }}</p>
        </div>
        <!-- 上传图片列表 -->
        <br />
        <div class="upload-wrap con-padding-horizontal">
          <!-- <span class="lable">附件上传</span>
           <el-upload
            ref="uploadattaching"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.attaching"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadattaching"
            :on-change="onChangeattaching"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload> -->
          <upload-pdf :fileType="fileType"
                      label="附件上传"
                      :fileIDs="fileIDs"
                      @getFileIDs="getFileID"></upload-pdf>
        </div>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'DeviceMaintainList'}">取消</router-link>
          </x-button>
          <x-button class="active"
                    @click.native="enter">保存</x-button>
        </div>
      </el-scrollbar>
    </div>

    <!-- <el-dialog
      :visible.sync="centerDialogVisible"
      :modal-append-to-body="false"
      custom-class="show-list-wrap"
      center>
      <iframe :src="previewUrl" width="100%" height="100%" frameborder="0"></iframe>
    </el-dialog> -->
  </div>
</template>
<script>
import { validateInputCommon, vInput, FileType } from '@/common/js/utils.js'
import api from '@/api/DeviceMaintainRegApi.js'
import XButton from '@/components/button'
import UploadPDF from '@/components/UploadPDF'
import axios from '@/api/interceptors'
import apiOrg from '@/api/orgApi'
export default {
  name: 'addDeviceMaintain',
  components: {
    XButton,
    'upload-pdf': UploadPDF
  },
  data () {
    return {
      loading: false,
      isShow: this.$route.params.mark,
      editID: this.$route.params.id,
      fileIDs: '',
      fileIDsEdit: '',
      fileType: FileType.DeviceMaintain_attach,
      maintain_date: {
        text: '',
        tips: ''
      },
      device: {
        text: '',
        tips: ''
      },
      devicelist: [],
      detail_desc: {
        text: '',
        tips: ''
      },
      team_group: {
        text: '',
        tips: ''
      },
      TeamGroupList: [],
      director: {
        text: '',
        tips: ''
      },
      team: '',
      teamPath: {
        text: [],
        tips: ''
      },
      teamList: [],
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      directorList: [],
      deviceType: {
        text: '',
        tips: ''
      },
      deviceTypeList: [],
      centerDialogVisible: false,
      pageType: ''
    }
  },
  props: {
    random: Number
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加设备维修记录'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改设备维修记录'
      this.getEditData()
    }
    // 设备配置类型列表
    api.GetEquipmentTypeList().then(res => {
      res.data.map((e, i) => {
        if (e.children != null && e.children.length > 0) {
          this.deviceTypeList.push({ value: e.id, label: e.deviceTypeName, children: []
          })
          e.children.map((item) => {
            this.deviceTypeList[i].children.push({ value: item.id, label: item.deviceName })
          })
        } else {
          this.deviceTypeList.push({ value: e.id, label: e.deviceTypeName })
        }
      })
    }).catch(err => console.log(err))
    // 设备配置类型列表
    api.GetDirectorList().then(res => {
      this.directorList = res.data
    }).catch(err => console.log(err))
    // 部门列表
    // api.GetTeamGroupList().then(res => {
    //   this.TeamGroupList = res.data
    // }).catch(err => console.log(err))
    // 班组加载
    apiOrg.getOrgAll().then(res => {
      this.teamList = res.data
    }).catch(err => console.log(err))
  },
  methods: { // 添加权限组
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      if (this.fileIDsEdit !== '') {
        let arr = this.fileIDsEdit.split(',')
        for (let i = 0; i < arr.length; i++) {
          if (arr[i] === '') {
            this.$message({
              message: '文件还未上传完成，请耐心等待',
              type: 'warning'
            })
            return
          }
        }
      }
      let model = {
        device_type_id: 0, // this.deviceType.text,
        device_id: 0, // this.device.text,
        team_group_path: this.teamPath.text.join(','),
        team_group_id: this.team_group.text,
        director_id: this.director.text,
        detail_desc: this.detail_desc.text,
        maintain_date: this.maintain_date.text,
        file_type: this.fileType,
        attch_file: this.fileIDsEdit,
        is_deleted: '0'
      }
      switch (this.deviceType.text.length) {
        case 0:
          model.device_type_id = 0
          model.device_id = 0
          break
        case 1:
          model.device_type_id = this.deviceType.text[0]
          break
        case 2:
          model.device_type_id = this.deviceType.text[0]
          model.device_id = this.deviceType.text[1]
          break
      }
      if (this.isShow === 'add') {
        // 添加站区
        api.Save(model).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'DeviceMaintainList'
                })
              }
            })
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else if (this.isShow === 'edit') {
        model.id = this.editID
        // 修改权限组
        api.Update(model).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'DeviceMaintainList'
                })
              }
            })
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    },
    // 修改权限组时获取权限组资料
    getEditData () {
      let id = this.editID
      api.GetDeviceMaintainRegById(id).then(res => {
        this.loading = false
        let _res = res.data
        this.deviceType.text = this.strToIntArr(_res.device_type_id + ',' + _res.device_id)
        this.fileIDs = _res.attch_file
        this.teamPath.text = this.strToIntArr(_res.team_group_path)
        this.team_group.text = _res.team_group_id
        this.director.text = _res.director_id
        this.detail_desc.text = _res.detail_desc
        this.maintain_date.text = _res.maintain_date
      }).catch(err => console.log(err))
    },
    strToIntArr (str) {
      let arr = str.split(',')
      let ret = []
      for (let i = 0; i < arr.length; i++) {
        if (arr[i] === 0) {
          continue
        }
        ret.push(parseInt(arr[i]))
      }
      return ret
    },
    cascader_change_copy (val) {
      var arr = this.teamPath.text
      let id = arr[arr.length - 1]
      this.team_group.text = id
      let pmObj = new Promise((resolve, reject) => {
        axios(
          {
            url: `http://10.89.36.204:5801/orgapi/org/node/${id}`,
            method: 'get'
          })
          .then(res => {
            resolve(res.data)
          })
          .catch(function (error) {
            reject(error)
          })
      })
      pmObj.then(data => {
        if (data.code === 0) {
          this.nodeType = data.data.nodeType
        }
      })
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
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
    validatedeviceTypeSelect () {
      if (this.deviceType.text === '') {
        this.deviceType.tips = '此项必选'
        return false
      } else {
        this.deviceType.tips = ''
        return true
      }
    },
    validateteam_groupSelect () {
      // if (this.team_group.text === '') {
      //   this.team_group.tips = '此项必选'
      //   return false
      // } else {
      //   this.team_group.tips = ''
      //   return true
      // }
      if (this.teamPath.text.length === 0) {
        this.teamPath.tips = '此项必选'
        return false
      }
      if (this.nodeType !== 3) {
        this.teamPath.tips = '您选的并非是班组，请选择班组'
        return false
      } else {
        this.teamPath.tips = ''
        return true
      }
    },
    validatedirectorSelect () {
      if (this.director.text === '') {
        this.director.tips = '此项必选'
        return false
      } else {
        this.director.tips = ''
        return true
      }
    },
    validatedeviceSelect () {
      if (this.device.text === '') {
        this.device.tips = '此项必选'
        return false
      } else {
        this.device.tips = ''
        return true
      }
    },
    validateNumber () {
      // validateNumberCommon(this.groupOrder)
    },
    validatechildSelect (val) {
      if (val === '') {
        this.devicelist = []
        this.device.text = ''
      }
      api.GetDeviceListByTypeId(val).then(res => {
        this.devicelist = res.data
        this.device.text = ''
      }).catch(err => {
        console.log(err)
      })
    },
    validateAll () {
      if (!this.validatedeviceTypeSelect()) return false
      // if (!this.validatedeviceSelect()) return false
      if (!this.validateteam_groupSelect()) return false
      if (!this.validatedirectorSelect()) return false
      if (!validateInputCommon(this.maintain_date)) return false
      if (!validateInputCommon(this.detail_desc)) return false
      return true
    },
    getFileID (val) {
      this.fileIDsEdit = val
    }
  }
}
</script>
<style lang="scss" scoped>
// 显示大图容器
.el-dialog__wrapper {
  width: 100%;
  height: 100%;

  .el-carousel {
    height: 100%;
  }
}

.header {
  display: flex;
  justify-content: space-between;
}

// 顶部信息
.middle {
  position: relative;
  margin-bottom: 10px;
  padding-bottom: 20px;

  .text-right {
    text-align: right !important;
  }

  [class*="list-wrap"] {
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list {
      margin-top: 20px;
      padding: 0 2%;
      border-right: 1px solid #c9cacd;
      text-align: center;

      &:first-of-type {
        padding-left: 0;
        text-align: left;
      }

      &:last-of-type {
        padding-right: 0;
        border: 0;
        text-align: right;
      }
    }
  }

  .list-wrap {
    .list {
      width: 16%;

      span {
        display: inline-block;
        width: 100%;
        @extend %ellipsis;
      }
    }
  }

  .sub-list-wrap {
    .list {
      margin-right: 40px;
      padding: 0;
      border: 0;
      text-align: left;
    }

    .text {
      color: $color-content-text;
    }

    &:after {
      content: "";
      flex: auto;
    }
  }
}

.scroll {
  /**
   * percent函数转换百分比
   * $content-height内容区域总高度
   * 页面标题栏高度：56
   */
  height: percent($content-height - 56, $content-height);
  .upload-wrap {
    display: flex;
    align-items: center;
  }
  /deep/ .el-collapse-item {
    .img-list {
      margin: 20px 10px 0 0;
      cursor: pointer;
    }
  }
}

.input-group {
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;

  .list {
    width: 30%;
    margin-top: PXtoEm(20);

    .inp-wrap {
      display: flex;
      align-items: center;
    }

    .text {
      width: 28%;
    }

    &:nth-of-type(3n + 1) {
      justify-content: flex-start;
    }

    &:nth-of-type(3n) {
      justify-content: flex-end;
    }
  }
}
.cause {
  display: flex;
  margin-top: 20px;
  align-items: center;

  .el-textarea {
    flex: 1;
    width: auto;
  }
}
// 图片预览
/deep/ .show-list-wrap {
  position: absolute;
  z-index: 9;
  top: 0;
  left: 0;
  overflow: hidden;
  width: 100%;
  height: 100%;
  background: #1c1c1c;
  transform: translate(0px);

  .el-carousel {
    height: 100%;
  }

  img {
    max-width: 100%;
    height: 100%;
  }

  .icon-close {
    position: absolute;
    z-index: 99;
    top: 3px;
    right: 3px;
    width: 27px;
    height: 27px;
    background: url(../../../../common/images/icon-close.png) no-repeat 0 0/100%
      100%;
    cursor: pointer;
  }

  .el-dialog__header {
    display: block;
    padding: 0;

    .el-dialog__headerbtn {
      top: -14px;
      right: -14px;
      width: 27px;
      height: 27px;
      background: url(../../../../common/images/icon-close.png) no-repeat 0 0/100%
        100%;
    }

    .el-dialog__close {
      display: none;
    }
  }

  .el-dialog__body {
    height: 100%;
  }
}

// 列表
.list-bottom-wrap {
  margin-top: 10px;

  .list {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    // @extend .con-padding-horizontal;
    background: rgba(186, 186, 186, 0.1);
    text-align: center;

    &:nth-of-type(odd) {
      background: rgba(128, 128, 128, 0.1);
    }

    &.list-header {
      height: 50px;
      background: $color-content-table-header;
    }

    &:not(.list-header) {
      padding: {
        top: 20px;
        bottom: 20px;
      }
    }
  }

  .list-con {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 20%;

    &:first-of-type {
      justify-content: flex-start;
    }
  }

  .textarea-wrap {
    width: 100%;
    margin-top: 10px;
  }
}

// 上传图片列表
.picture-list-wrap {
  .list {
    width: 80px;
    height: 80px;
    background-color: $color-white;
    border-radius: $border-radius;
    text-align: center;
    line-height: 80px;

    &:before {
      content: "\e6cc";
      font-size: 28px;
      font-family: "iconfont";
      color: #8c939d;
    }
  }
}

// 提交底部按钮
.btn-group {
  padding: 20px 0;
  text-align: center;
}

.lable {
  width: 100px;
}

.disabled {
  background: #8c939d;
}
</style>
