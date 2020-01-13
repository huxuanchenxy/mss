<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <!-- 内容 -->
    <div class="content-wrap">
      <el-container style="height:100%">
        <el-aside width="500px">
          <ul class="content-header">
            <el-input
              placeholder="输入关键字进行过滤"
              v-model="filterText">
            </el-input>
          </ul>
          <div class="scroll">
            <!-- <el-scrollbar> -->
              <div class="custom-tree-container">
                <div class="block">
                  <el-tree
                    :data="list"
                    node-key="id"
                    ref="tree"
                    :expand-on-click-node="false"
                    :filter-node-method="filterNode">
                    <span class="custom-tree-node" slot-scope="{ node, data }">
                      <li class="custom-tree-list">
                        <template>
                          <div class="left">
                            <el-checkbox v-model="treeCheckedID" :label="data.id" @change="onChange"
                              v-show="canShow(data)"></el-checkbox>
                            <span style="padding-left:10px;">{{ data.label }}</span>
                          </div>
                        </template>
                        <template>
                          <div class="right" style="padding-right:10px;">
                            <el-button size="mini" v-show="canDelBtnShow(data)" @click="delOrgNode(data, node)" :disabled="btn.delete">删除</el-button>
                          </div>
                        </template>
                      </li>
                    </span>
                  </el-tree>
                </div>
              </div>
            <!-- </el-scrollbar> -->
          </div>
        </el-aside>
        <el-main style="padding:0px">
          <el-container style="height:100%">
            <el-aside class="middleBar" width="100px">
                <el-button type="primary" style="width:40px; height:40px;top:50px"
                   icon="el-icon-back" @click="onSaveBindClick" circle :disabled="btn.save">
                </el-button>
            </el-aside>
            <el-main style="padding:0px">
              <ul class="content-header">
                <el-input placeholder="输入关键字进行过滤" clearable v-model="keyUser"> </el-input>
              </ul>
              <el-scrollbar>
                <div class="content-wrap">
                  <ul class="content-header">
                    <li class="list checkbox">
                      <el-checkbox v-model="bCheckAll" @change="checkAll"></el-checkbox>
                    </li>
                    <li class="list number">
                      工号
                    </li>
                    <li class="list name" >
                      人员名称
                    </li>
                  </ul>
                  <div class="scroll">
                    <el-scrollbar>
                      <ul class="list-wrap">
                        <li class="list" v-for="item in AvailableUsers" :key="item.key">
                          <div class="list-content">
                            <div class="list checkbox">
                              <el-checkbox v-model="checkedID" :label="item.id"></el-checkbox>
                            </div>
                            <div class="number">{{ item.job_number }}</div>
                            <div class="name">{{ item.user_name }}</div>
                          </div>
                        </li>
                      </ul>
                    </el-scrollbar>
                  </div>
                </div>
              </el-scrollbar>
            </el-main>
          </el-container>
        </el-main>
      </el-container>
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
          <el-button @click="dialogEnter">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
import { ApiRESULT } from '@/common/js/utils.js'
import XButton from '@/components/button'
// import OrgTreeRender from './TreeRenderForUser'
import api from '@/api/orgApi.js'
import { btn } from '@/element/btn.js'
// import eventBus from '@/components/Bus'
export default {
  name: 'OrgList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false,
        delete: false
      },
      loading: true,
      filterText: '',
      keyUser: '',
      list: [],
      treeCheckedID: [],
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      bCheckAll: false,
      checkedID: [],
      Users: [],
      AvailableUsers: [],
      originAvailabelUsers: []
    }
  },
  watch: {
    filterText (val) {
      this.$refs.tree.filter(val)
    },
    keyUser (val) {
      this.checkedID = []
      this.AvailableUsers = this.keyUser ? this.originAvailableUsers
        .filter(this.createStateFilter(this.keyUser)) : this.originAvailableUsers
    }
  },
  methods: {
    createStateFilter (queryString) {
      return (state) => {
        return (state.user_name.toLowerCase().indexOf(queryString.toLowerCase()) > -1 ||
        state.job_number.toLowerCase().indexOf(queryString.toLowerCase()) > -1)
      }
    },
    filterNode (value, data) {
      if (!value) return true
      return data.label.indexOf(value) !== -1
    },
    canShow (data) {
      let ret = true
      if (data.node_type && data.node_type !== 0) { // 组织节点
        if (!data.type.hasUsers) {
          ret = false
        }
        if (data.type.hasUsers && data.type.hasUsersLeafOnly) {
          if (data.children.length > 0) {
            if (data.children[0].node_type) {
              ret = false
            }
          }
        }
      } else {
        ret = false
      }
      return ret
    },
    canDelBtnShow (data) {
      if (!data.node_type && data.node_type !== 0) {
        return true
      }
    },
    onChange (val) {
      if (val) {
        // 最后一个
        let id = this.treeCheckedID[this.treeCheckedID.length - 1]
        this.treeCheckedID = [id]
      } else {
        this.treeCheckedID = []
      }
    },
    getList () {
      api.getOrgUserAll().then((res) => {
        if (res.code === ApiRESULT.Success) {
          this.list = res.data
          this.loading = false
        } else {
          this.$message.error('加载组织结构失败')
        }
      })
    },
    delOrgNode (data, node) {
      // this.showDialog({data: data})
      api.delOrgUser(data.dataID).then((res) => {
        if (res.code === ApiRESULT.Success) {
          // 更新用户列表
          this.getAllUsers()
          let parent = node.parent
          node.remove()
          if (parent.childNodes.length === 0 && parent.data.node_type === 0) {
            parent.remove()
          }
        }
      })
    },
    showDialog (val) {
      this.obj = val
      this.dialogVisible.isShow = true
      this.dialogVisible.btn = true
      this.dialogVisible.text = '确定删除该节点?'
    },
    // 弹框确认是否删除
    dialogEnter () {
      api.delOrgNode(this.obj.data.id).then((res) => {
        this.dialogVisible.isShow = false
        if (res.code === ApiRESULT.Success) {
          this.$refs.tree.remove(this.obj.data)
          this.$message.success('删除成功')
        } else {
          this.$message.error('删除失败')
        }
      })
    },
    // 全选
    checkAll () {
      this.bCheckAll ? this.Users.map(val => this.checkedID.push(val.id)) : this.checkedID = []
    },
    onSaveBindClick () {
      if (this.treeCheckedID.length === 0) {
        this.$message.error('请选择组织节点')
        return false
      }
      let data = {
        ID: this.treeCheckedID[0],
        UserIDs: []
      }
      this.checkedID.forEach(id => {
        data.UserIDs.push(id)
      })
      if (data.UserIDs.length === 0) {
        this.$message.error('请选择人员')
        return false
      }
      api.BindOrgUser(data).then((res) => {
        if (res.code === ApiRESULT.Success) {
          // 关联后更新节点、更新用户列表
          this.checkedID = []
          this.updateOrgNode(data.ID)
          this.getAllUsers()
        } else if (res.code === ApiRESULT.BindUserConflict) {
          this.$message.error('所选用户已被其他节点关联')
        } else if (res.code === ApiRESULT.CheckDataRulesFail) {
          this.$message.error('改节点不能绑定用户')
        } else {
          this.$message.error('关联人员失败')
        }
      })
    },
    updateOrgNode (id) {
      api.getOrgUserByNodeID(id).then((res) => {
        if (res.code === ApiRESULT.Success) {
          let node = this.$refs.tree.getNode(id)
          // node.setData(res.data)
          // this.$set(node, 'label', res.data[0].label)
          node.data = res.data[0]
          node.expanded = true
        } else {
          this.$message.error('关联人员失败')
        }
      })
    },
    getAllUsers () {
      this.bCheckAll = false
      // this.getOrgUser(this.$route.params.id)
      api.getAllUsers().then((res) => {
        if (res.code === 0) {
          this.Users = res.data
          this.getSelectedUsers()
        } else {
          this.$message.error('获取人员信息失败')
        }
      })
    },
    getSelectedUsers () {
      api.getSelectedUsers().then((res) => {
        if (res.code === ApiRESULT.Success) {
          // 剔除其他节点已选用户
          this.AvailableUsers = []
          let disabledUser = res.data
          for (let i = 0; i < this.Users.length; ++i) {
            let user = this.Users[i]
            let exist = false
            for (let i = 0; i < disabledUser.length; ++i) {
              if (disabledUser[i].userID === user.id) {
                exist = true
                break
              }
            }
            if (!exist) {
              this.AvailableUsers.push(user)
            }
          }
          this.originAvailableUsers = this.AvailableUsers
        } else {
          this.$message.error('获取节点用户失败')
        }
      })
    },
    initBtn () {
      let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
      if (!user.is_super) {
        let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
        this.btn.save = !actions.some((item, index) => {
          return item.actionID === btn.orguser.save
        })
        this.btn.delete = !actions.some((item, index) => {
          return item.actionID === btn.orguser.delete
        })
      }
    }
  },
  mounted () {
    // 设置本页面标题
    // this.$emit('title', '| 组织架构列表')
    this.$emit('pageInfo', {
      title: '| 组织架构人员配置',
      isShowBackBtn: false,
      url: ''
    })
    this.getList()
    this.getAllUsers()
    this.initBtn()
  }
}
</script>
<style lang="scss" scoped>
$height: $content-height - 56;
.middleBar {
  display: flex;
  width: 100px;
  border-left:1px solid rgb(30,59,87);
  border-right:1px solid rgb(30,59,87);
  justify-content:center;
  align-items:center;
}
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($height, $height);

  .name{
    text-align: left;
  }

  .operation{
    text-align: right;
  }

  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    // height: percent(50, $height - 80);
    height: 40px;
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);
  }

  .scroll{
    height: percent($height - 130, $height - 80);
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
      height: 50px;
      padding: 0 PXtoEm(24);
    }
    /deep/ .el-checkbox__label{
      display: none;
    }
  }
  .name,
  .number{
    width: 150px;
    word-break: break-all;
  }
}
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

/deep/ .el-button.is-disabled, .el-button.is-disabled:hover, .el-button.is-disabled:focus {
  opacity: .65;
  border-color: #c0c4cc !important;
  background: none !important;
  cursor: Default;
}
</style>
