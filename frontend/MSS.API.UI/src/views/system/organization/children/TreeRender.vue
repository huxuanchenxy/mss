<template>
  <ul class="custom-tree-node">
    <li class="custom-tree-list" :class="{ 'is-top': data.isTop }">
      <template>
        <div class="left">
          <span>{{ data.label }}</span>
        </div>
      </template>
      <template>
        <div class="right">
          <el-button size="mini" v-show="data.type.hasChildren" @click="addOrgNode(data, node)" :disabled="btn.save">添加</el-button>
          <el-button size="mini" @click="delOrgNode(data, node)" :disabled="btn.delete">删除</el-button>
          <el-button size="mini" @click="updateOrgNode(data)" :disabled="btn.update">修改</el-button>
          <el-button size="mini" @click="seeOrgNode(data)">查看</el-button>
          <!-- <el-button size="mini">
            <router-link class="color-white block" :to="{ name: 'OrgUser', params: { id: data.id, label: data.label } }">人员配置</router-link>
          </el-button> -->
        </div>
      </template>
    </li>
  </ul>
</template>
<script>
// import { RESULT } from '@/common/js/utils.js'
// import api from '@/api/orgApi'
export default {
  name: 'OrgTreeRender',
  data () {
    return {
      nodeId: 1000,
      tips: ''
    }
  },
  props: ['data', 'node', 'store', 'tree', 'getList', 'showDialog', 'btn'],
  created () {
    this.data.oldName = this.data.label
  },
  methods: {
    addOrgNode (data, node) {
      this.CurNode = {
        id: 0,
        parentID: data.id
      }
      this.$router.push({
        name: 'OrgNode',
        query: this.CurNode
      })
    },
    updateOrgNode (data) {
      this.CurNode = {
        id: data.id,
        parentID: null
      }
      this.$router.push({
        name: 'OrgNode',
        query: this.CurNode
      })
    },
    seeOrgNode (data) {
      this.CurNode = {
        id: data.id,
        readonly: true,
        parentID: null
      }
      this.$router.push({
        name: 'OrgNode',
        query: this.CurNode
      })
    },
    // 删除
    delOrgNode (data, node) {
      this.showDialog({data: data})
      // api.delOrgNode(data.id).then((res) => {
      //   if (res.result === RESULT.Success) {
      //     this.$message.error('删除成功')
      //   } else {
      //     this.$message.error('删除失败')
      //   }
      // })
    }
  }
}
</script>
<style lang="scss" scoped>
.custom-tree-list{
  &:hover{
    .right{
      display: block;
    }
  }
}

.left{
  display: flex;

  .inp-wrap{
    margin-right: 10px;
  }

  .validate-tips{
    height: 18px;
    margin-top: 3px;
    font-size: 12px;
    text-indent: 10px;
    color: $color-warning;
  }
}

.right{
  display: none;
}
/deep/ .el-button.is-disabled, .el-button.is-disabled:hover, .el-button.is-disabled:focus {
  opacity: .65;
  border-color: #c0c4cc !important;
  background: none !important;
  cursor: Default;
}
</style>
