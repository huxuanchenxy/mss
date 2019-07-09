<template>
  <ul class="custom-tree-node">
    <li class="custom-tree-list" :class="{ 'is-top': data.isTop }">
      <template>
        <div class="left">
          <el-checkbox v-model="checkedID" :label="data.id" @change="onChange" v-show="canShow(data)"></el-checkbox>
          <span style="padding-left:10px;">{{ data.label }}</span>
        </div>
      </template>
      <template>
        <div class="right">
          <el-button size="mini" @click="delOrgNode(data, node)">删除</el-button>
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
    }
  },
  props: ['data', 'node', 'store', 'tree', 'getList', 'showDialog', 'checkedID'],
  created () {
    this.data.oldName = this.data.label
  },
  methods: {
    canShow (data) {
      if (data.node_type && data.node_type !== 0) {
        return true
      }
    },
    onChange (val) {
      console.log(this.checkedID)
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
  /deep/ .el-checkbox__label{
    display: none;
  }
}

.right{
  display: none;
}
</style>
