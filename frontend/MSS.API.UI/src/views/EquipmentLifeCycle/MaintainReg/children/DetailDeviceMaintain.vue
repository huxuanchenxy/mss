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
        <router-link :to="{name:'DeviceMaintainList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">设备类别</span>
            <div class="inp">
              <div class="inp">{{Device.deviceType}}</div>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">设备名称</span>
                <div class="inp">{{Device.deviceName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
             <span class="text">负责班组</span>
                <div class="inp">{{Device.teamGroupName}}</div>
          </div>
        </li>
         <li class="list" >
              <div class="inp-wrap">
                <span class="text">维护负责人</span>
                <div class="inp">{{Device.director}}</div>
              </div>
            </li>
         <li class="list2">
              <div class="inp-wrap">
                <span class="text">维护日期</span>
                <div class="inp">{{Device.maintain_date}}</div>
              </div>
            </li>
      </ul>
      <ul class="input-group">
        <li class="list1">
          <div class="inp-wrap">
            <span class="text">备件更换过程记录</span>
            <div class="inp" style="width:86.5%">
              <el-input type="textarea" v-model="Device.detail_desc"  :rows="7"></el-input>
            </div>
          </div>
        </li>
      </ul>
       <br/>
            <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div>
                <upload-pdf :fileType="fileType" label="附件" :fileIDs="fileattachIDs" :readOnly="true" @getFileIDs="getattachFileID"></upload-pdf>
              </div>
            </li>
              </ul>
            </div>
    </div>
  </div>
</template>
<script>
import { FileType } from '@/common/js/utils.js'
import api from '@/api/DeviceMaintainRegApi.js'
import XButton from '@/components/button'
import UploadPDF from '@/components/UploadPDF'
import apiOrg from '@/api/orgApi'
export default {
  name: 'addDeviceMaintain',
  components: {
    XButton,
    'upload-pdf': UploadPDF
  },
  data () {
    return {
      fileType: FileType.DeviceMaintain_attach,
      fileattachIDs: '',
      fileattachIDsEdit: '',
      title: '| 设备维修详情',
      loading: false,
      Device: {
        id: this.$route.params.id,
        deviceType: '',
        deviceName: '',
        teamGroupName: '',
        director: '',
        maintain_date: '',
        detail_desc: ''
      }
    }
  },
  created () {
    this.getviewData()
  },
  methods: {
    // 修改设备时获取设备资料
    getviewData () {
      let id = this.Device.id
      api.GetDeviceMaintainRegById(id).then(res => {
        this.loading = false
        let _res = res.data
        this.Device.deviceType = _res.device_type_name
        this.Device.deviceName = _res.device_name
        this.Device.teamGroupName = _res.team_group_name
        // this.getgroupNameById(_res.team_group_id)
        this.Device.director = _res.director_name
        this.Device.detail_desc = _res.detail_desc
        this.Device.maintain_date = _res.maintain_date
        this.fileattachIDs = _res.uploadFiles
        // this.fileattachIDsEdit = _res.attch_file
      }).catch(err => console.log(err))
    },
    getgroupNameById (id) {
      apiOrg.getOrgNode(id).then(res => {
        if (res.code === 0) {
          this.Device.teamGroupName = res.data.name
        }
      })
    },
    getattachFileID (val) {
      this.fileIDs = val
    }
  }
}
</script>
<style lang="scss" scoped>
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
    .upload-list{
      margin-bottom: PXtoEm(25);
      }
    }
   .list2{
      width: 65%;
      margin-top: PXtoEm(25);
      span{
        width: 10%;
      }
      .inp-wrap{
        display: flex;
        align-items: center;
      }
    }
   .list1{
      width: 100%;
      margin-top: PXtoEm(25);
      span{
        width: 8.3%;
      }
      .inp-wrap{
        display: flex;
        align-items: center;
      }
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

#responsearea{
    min-height: 93px;
}
</style>
<style>
  .el-textarea .el-textarea__inner{
    resize: none;
  }
</style>
