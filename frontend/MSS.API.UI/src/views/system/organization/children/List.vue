<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <!-- 搜索框 -->
    <div class="con-padding-horizontal middle">
      <div class="left">
      </div>
      <div class="right">
        <x-button @click.native="addOrgNode" :disabled="btn.save">添加</x-button>
      </div>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list name">组织名称</li>
        <li class="list operation">操作</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <div class="custom-tree-container">
            <div class="block">
              <el-tree
                :data="list"
                node-key="id"
                ref="tree"
                :expand-on-click-node="false"
                :render-content="renderContent">
              </el-tree>
            </div>
          </div>
        </el-scrollbar>
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
          <el-button @click="dialogEnter">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
import { vInput, ApiRESULT } from '@/common/js/utils.js'
import XButton from '@/components/button'
import OrgTreeRender from './TreeRender'
import api from '@/api/orgApi.js'
import eventBus from '@/components/Bus'
import { btn } from '@/element/btn.js'
export default {
  name: 'OrgList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false,
        delete: false,
        update: false
      },
      loading: true,
      list: [],
      tips: '',
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      obj: ''
    }
  },
  methods: {
    getList () {
      api.getOrgAll().then((res) => {
        if (res.code === ApiRESULT.Success) {
          this.list = res.data
          this.loading = false
        } else {
          this.$message.error('加载组织结构失败')
        }
      })
    },

    renderContent (h, { node, data, store }) {
      return h(OrgTreeRender, {
        props: {
          data,
          node,
          store,
          tree: this.$refs.tree,
          getList: this.getList,
          showDialog: this.showDialog,
          btn: this.btn
        }
      })
    },

    addOrgNode () {
      this.CurNode = {
        id: 0,
        parentID: null
      }
      this.$router.push({
        name: 'OrgNode',
        query: this.CurNode
      })
    },

    // 验证
    validateInput (val) {
      val = val.replace(/\s*/g, '')
      if (val === '') {
        this.tips = '此项必填'
        return false
      } else if (!vInput(val)) {
        this.tips = '此项含有非法字符'
        return false
      } else {
        this.tips = ''
        return true
      }
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
    submit (val) {
    },
    initBtn () {
      let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
      if (!user.is_super) {
        let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
        this.btn.save = !actions.some((item, index) => {
          return item.actionID === btn.org.save
        })
        this.btn.delete = !actions.some((item, index) => {
          return item.actionID === btn.org.delete
        })
        this.btn.update = !actions.some((item, index) => {
          return item.actionID === btn.org.update
        })
      }
    }
  },
  mounted () {
    // 设置本页面标题
    // this.$emit('title', '| 组织架构列表')
    this.$emit('pageInfo', {
      title: '| 组织架构',
      isShowBackBtn: false,
      url: ''
    })
    eventBus.$on('submit', data => {
      this.submit(data)
    })
    this.getList()
    this.initBtn()
  }
}
</script>
<style lang="scss" scoped>
$height: $content-height - 56;
.wrap{
  .middle{
    display: flex;
    align-items: center;
    justify-content: space-between;
    height: percent(80, $height);
    background: rgba(128, 128, 128, 0.1);
    color: $color-white;

    .left{
      display: flex;
      align-items: center;
    }

    .validate-tips{
      height: 18px;
      margin-top: 3px;
      font-size: 12px;
      text-indent: 10px;
      color: $color-warning;
    }
  }
}

// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($height - 80, $height);

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
    height: percent(50, $height - 80);
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);
  }

  .scroll{
    height: percent($height - 130, $height - 80);
  }
}
</style>
