<template>
<div>
 <x-button @click.native="startprocess">启动工作流</x-button>
 <x-button @click.native="nextprocess">转到下一步</x-button>
 <x-button @click.native="withdrawprocess">撤销</x-button>
 <x-button @click.native="sendbackprocess">退回</x-button>
</div>
</template>
<script>
import XButton from '@/components/button'
import wfapi from '@/api/workflowApi'
export default {
  name: 'WorkflowModule',
  components: {
    XButton
  },
  props: {
    AppInstanceID: Number
    // fileType: Number,
    // fileIDs: String,
    // readOnly: Boolean
  },
  mounted () {
    this.getnextprocess()
    this.getcurrentprocess()
  },
  data () {
    return {
      NextActivityGUID: '',
      NextActivityPerformers: {},
      NextUsers: []
    }
  },
  methods: {
    startprocess () {
      let parm = {
        // 'AppName': '地铁18号线分支流程',
        'AppInstanceID': this.AppInstanceID,
        // 'ProcessGUID': '579089c9-46dd-4fd2-95f9-e19d33d5fc4b',
        'ProcessID': '204',
        'Conditions': {'days': '6'}
      }
      wfapi.startprocess(parm).then(res => {
        if (res.status === 1) {
          this.$router.push({name: 'SeeConstructionPlan'})
          this.$message({
            message: '启动成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.message === '' ? '启动失败' : res.message,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    nextprocess () {
      let parm = {
        // 'AppName': '地铁18号线分支流程',
        'AppInstanceID': this.AppInstanceID,
        // 'ProcessGUID': '579089c9-46dd-4fd2-95f9-e19d33d5fc4b',
        'ProcessID': '204',
        'Conditions': {'days': '6'},
        NextActivityPerformers: {}
      }
      parm.NextActivityPerformers[this.NextActivityGUID] = this.NextUsers
      wfapi.nextprocess(parm).then(res => {
        if (res.status === 1) {
          this.$router.push({name: 'SeeConstructionPlan'})
          this.$message({
            message: '转到下一步成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.message === '' ? '转到下一步失败' : res.message,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    getnextprocess () {
      let parm = {
        'AppInstanceID': this.AppInstanceID,
        'ProcessID': '204',
        'Conditions': {'days': '6'}
      }
      wfapi.getnextprocess(parm).then(res => {
        if (res.status === 1) {
          this.$message({
            message: '获取下一步流程成功',
            type: 'success'
          })
          let nextobj = res.entity
          if (nextobj !== null) {
            this.NextActivityGUID = nextobj[0].ActivityGUID
            // console.log('nextactiveguid:' + this.NextActivityGUID)
            this.NextUsers = nextobj[0].Users
          }
        } else {
          this.$message({
            message: res.message === '' ? '获取下一步流程失败' : res.message,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    withdrawprocess () {
      let parm = {
        'AppInstanceID': this.AppInstanceID,
        'ProcessID': '204',
        'Conditions': {'days': '6'}
      }
      wfapi.withdrawprocess(parm).then(res => {
        if (res.status === 1) {
          this.$message({
            message: '撤销流程成功',
            type: 'success'
          })
          let nextobj = res.entity
          if (nextobj !== null) {
            this.NextActivityGUID = nextobj[0].ActivityGUID
            console.log('nextactiveguid:' + this.NextActivityGUID)
          }
        } else {
          this.$message({
            message: res.message === '' ? '撤销流程失败' : res.message,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    sendbackprocess () {
      let parm = {
        'AppInstanceID': this.AppInstanceID,
        'ProcessID': '204',
        'Conditions': {'days': '6'}
      }
      wfapi.sendbackprocess(parm).then(res => {
        if (res.status === 1) {
          this.$message({
            message: '退回流程成功',
            type: 'success'
          })
          let nextobj = res.entity
          if (nextobj !== null) {
            this.NextActivityGUID = nextobj[0].ActivityGUID
            console.log('nextactiveguid:' + this.NextActivityGUID)
          }
        } else {
          this.$message({
            message: res.message === '' ? '退回流程失败' : res.message,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    getcurrentprocess () {
      let parm = {
        'AppInstanceID': this.AppInstanceID,
        'ProcessID': '204',
        'Conditions': {'days': '6'}
      }
      wfapi.getcurrentprocess(parm).then(res => {
        if (res.status === 1) {
          console.log(res)
        } else {
          this.$message({
            message: res.message === '' ? '获取当前流程失败' : res.message,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
</style>
