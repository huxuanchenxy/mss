<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <div class="troublescroll">
      <HotTable :settings="settings" licenseKey="non-commercial-and-evaluation" v-show="isShowList"/>
      <div style="text-align: center;margin-top:10px">
      <el-button type="primary" class="focus" @click="save(false)"  v-show="isShowList">保存</el-button>
      <el-button type="primary" @click="save(true)" style="margin-left: 20px;"  v-show="isShowList">提交</el-button>
      </div>
    </div>
    <!-- dialog对话框 -->
    <el-dialog
      :visible.sync="dialogVisible.isShow"
      :modal-append-to-body="false"
      :show-close="false">
      {{ dialogVisible.text }}
      <template slot="footer" class="dialog-footer">
        <template v-if="dialogVisible.btn">
          <el-button @click="dialogVisible.isShow = false">否</el-button>
          <el-button @click="submit(true)">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
    <BottomNavigation></BottomNavigation>
  </div>
</template>
<script>
import isheader from "./commom/header3.vue"
import { HotTable } from '@handsontable/vue'
import BottomNavigation from "./commom/bottom.vue"
import api from '@/api/workflowApi'
import Bus from './commom/Bus'
export default {
  name: "inputCheck",
  components: {
    isheader,
    HotTable,
    BottomNavigation
  },
  data() {
    return {
      settings: {},
      isShowList: false,
      dialogVisible: {
        isShow: false,
        text: '',
        btn: true
      },
      editModuleID: '',
      selectList: [],
      editMonth: '',
      editID: ''
    }
  },
  mounted() {
    this.editID = this.$route.params.id
    Bus.$emit('addattr', this.$route.params.attr)
    api.getEntityByID(this.editID, true).then(res => {
      let _data = res.data
      // this.loading = false
      this.editModuleID = _data.module.id

      this.selectList = _data.cpmd
      this.editMonth = _data.cpmd
      let clientHeight = `${document.documentElement.clientHeight}`
      this.settings = {
        readOnly: false,
        // fixedColumnsLeft: 3,
        data: _data.showData,
        tableClassName: ['htMiddle', 'htCenter'],
        width: '100%',
        height: clientHeight - 90 - 100,
        mergeCells: _data.showMerge
      }
      this.isShowList = true
    }).catch(err => console.log(err))
    // this.InitSelect();
  },
  methods: {
    save (isSubmit) {
      if (isSubmit) {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '提交后将不可修改，是否确认?'
      } else {
        this.submit(false)
      }
    },
    submit (isSubmit) {
      let items = this.editMonth
      let ids = []
      items.map(val => {
        ids.push(val.id)
      })
      let pmEntity = {
        title: this.settings.data[0][0] + '(' + items[0].pmTypeName + ')',
        team: items[0].team,
        planDate: items[0].planDate,
        location: items[0].location,
        locationBy: items[0].locationBy,
        module: this.editModuleID,
        contents: this.settings.data,
        eqp: this.eqpID,
        isFinished: isSubmit,
        PMMonthDetails: ids
      }
      pmEntity.id = this.editID
      pmEntity.isPlanChanged = false
      api.updatePMEntity(pmEntity).then(res => {
        if (res.code === 0) {
          this.$message({
            message: '操作成功',
            type: 'success'
          })
          if (isFinished) {
            this.$router.push({
              name: 'EntityList'
            })
            this.dialogVisible.isShow = false
          }
        } else {
          this.$message({
            message: res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    }
  }
};
</script>
<style>
.wrapper {
  /* display: flex;
        flex-direction: column;
        height: 100vh; */
  position: fixed;
  width: 100%;
  height: 100%;
}
.tab {
  flex: 1;
  margin: 9rem 0 4rem 0;
}
.header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1;
}
.troublescroll {
  position: absolute;
  top: 80px;
  width: 100%;
  height: 100%;
}
.el-button:hover, .el-button:focus{
  background: #1871b5!important;
  border-color: #1871b5!important;
  color: #fff!important;
}
.handsontable td{
  color: #FFF!important;
  background-color: #212028!important;
  text-align: center!important;
}
.handsontableInput{
  color: #fff!important;
  background-color: #212028!important
}
/*.ht_master .wtHolder{
  overflow: hidden!important;
}*/
</style>
