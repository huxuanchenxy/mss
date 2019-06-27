<template>
  <div class="wrap height-full">
    <!-- 搜索框 -->
    <div class="con-padding-horizontal middle">
      <div class="left">名称：{{ $route.params.label }}</div>
      <div class="right">
        <x-button @click.native="onSaveBindClick">保存</x-button>
      </div>
    </div>
    <!-- 内容 -->
    <div class="content-wrap height-full">
      <div id="use-scroll" class="scroll">
        <el-scrollbar>
          <ul class="content-header">
            <li class="list number">
              人员编号
            </li>
            <li class="list name" >
              人员名称
            </li>
            <li class="list checkbox"><el-checkbox v-model="bCheckAll" @change="checkAll"></el-checkbox></li>
          </ul>
          <div class="scroll-1">
            <el-scrollbar>
              <ul class="list-wrap">
                <li class="list" v-for="item in AvailableUsers" :key="item.key">
                  <div class="list-content">
                    <div class="number">{{ item.id }}</div>
                    <div class="name">{{ item.user_name }}</div>
                    <div class="list checkbox">
                      <el-checkbox v-model="checkedID" :label="item.id"></el-checkbox>
                    </div>
                  </div>
                </li>
              </ul>
            </el-scrollbar>
          </div>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
import {RESULT} from '@/common/js/utils.js'
import api from '@/api/orgApi'
export default {
  name: 'OrgSee',
  nodeID: '',
  components: {
    XButton
  },
  data () {
    return {
      bCheckAll: false,
      checkedID: [],
      disabledID: [1],
      Users: [],
      AvailableUsers: []
    }
  },
  methods: {
    // 全选
    checkAll () {
      this.bCheckAll ? this.Users.map(val => this.checkedID.push(val.id)) : this.checkedID = []
    },
    onSaveBindClick () {
      let data = {
        ID: this.$route.params.id,
        UserIDs: []
      }
      this.checkedID.forEach(id => {
        data.UserIDs.push(id)
      })
      api.BindOrgUser(data).then((res) => {
        if (res.result === RESULT.Success) {
          this.$router.push({
            name: 'OrgList'
          })
          // 重新加载关联的人员
          // this.getOrgUser(this.$route.params.id)
        } else {
          this.$message.error('关联人员失败')
        }
      })
    },
    getCanSelectedUser (id) {
      api.getCanSelectedUser(id).then((res) => {
        if (res.result === RESULT.Success) {
          this.Users = res.data
          this.getOrgUser(id)
        } else {
          this.$message.error('获取节点信息失败')
        }
      })
    },
    getAllUsers () {
      // this.getOrgUser(this.$route.params.id)
      api.getAllUsers().then((res) => {
        if (res.code === 0) {
          this.Users = res.data
          this.getOrgUser(this.$route.params.id)
        } else {
          this.$message.error('获取人员信息失败')
        }
      })
    },
    getOrgUser (id) {
      api.getOrgUser(id).then((res) => {
        if (res.result === RESULT.Success) {
          // 剔除其他节点已选用户
          this.AvailableUsers = []
          let disabledUser = res.data.disabledUsers
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

          let users = res.data.users
          for (let i = 0; i < users.length; ++i) {
            let orguser = users[i]
            this.checkedID.push(orguser.userID)
          }
        } else {
          this.$message.error('获取节点关联失败')
        }
      })
    }
  },
  created () {
    // this.$emit('title', '组织架构列表 | 添加')
    this.$emit('pageInfo', {
      title: ' | 组织架构 | 人员配置',
      isShowBackBtn: true,
      url: 'OrgList'
    })
    // this.getCanSelectedUser(this.$route.params.id)
    this.getAllUsers()
  }
}
</script>
<style lang="scss" scoped>
$height: $content-height - 56;
.wrap{
  // 搜索组
  .middle{
    position: relative;
    display: flex;
    align-items: center;
    justify-content: space-between;
    height: percent(80, $height);
    background: rgba(128, 128, 128, 0.1);
    color: $color-white;

    .left,
    .right{
      width: 50%;
    }

    .right{
      display: inherit;
      justify-content: flex-end;
    }

    .inp-wrap{
      width: 210px;
    }

    .btn{
      margin-left: PXtoEm(24);
      border: 0;
      background: $color-main-btn;
    }
  }
}

// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($height - 80, $height);

  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(50, $height - 80);
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);
    .gallery{
      p{
        margin-left: 10px;
      }
      text-align: center;
    }

    .gallery-list{
      height: 20px;
      font-size: $font-size-small;
    }
  }

  .scroll{
    overflow-y: hidden;
    height: 100%;

    /deep/ .el-scrollbar__view{
      overflow-y: hidden;
      width: 100%;
      height: 100%;

      .el-scrollbar__view{
        height: auto;
      }

      .el-scrollbar__bar{
        &.is-vertical{
          display: block;
          position: fixed;
          bottom: 0 !important;
          top: initial;
          height: percent($height - 80 - 50, $content-height);
        }
      }
    }

    /deep/ .el-scrollbar__bar{
      &.is-horizontal{
        display: block !important;
      }

      &.is-vertical{
        display: none;
      }
    }
  }

  // 列表滚动区域
  .scroll-1{
    height: percent($height - 80 - 50, $height - 80);
  }
  // 所属管廊
  .gallery-list-wrap{
    display: flex;
    justify-content: space-between;
    margin-left: 10px;

    .gallery-list{
      width: 120px;
      text-align: center;
      .con{
        width: 100%;
        @extend %ellipsis;
      }
    }
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
  }
  .name,
  .number{
    width: 150px;
    word-break: break-all;
  }
  /deep/ .el-checkbox__label{
    display: none;
  }
}
// $height: $content-height - 56;
// .wrap{
//   // 搜索组
//   .middle{
//     position: relative;
//     display: flex;
//     align-items: center;
//     justify-content: space-between;
//     height: percent(80, $height);
//     background: rgba(128, 128, 128, 0.1);
//     color: $color-white;

//     .left,
//     .right{
//       width: 50%;
//     }

//     .right{
//       display: inherit;
//       justify-content: flex-end;
//     }

//     .inp-wrap{
//       width: 210px;
//     }

//     .btn{
//       margin-left: PXtoEm(24);
//       border: 0;
//       background: $color-main-btn;
//     }
//   }
// }

// // 内容区
// .content-wrap{
//   overflow: hidden;
//   height: percent($height - 80, $height);

//   .content-header,
//   .list-con{
//     display: flex;
//     justify-content: space-between;
//     align-items: center;
//     height: percent(50, $content-height);
//     padding: 0 PXtoEm(24);

//     .list{
//       width: 10%;
//       text-align: left;
//       word-break: break-word;
//     }

//     .checkbox{
//       width: 5%;
//     }

//     .time{
//       width: 15%;
//     }
//   }

//   .content-header{
//     background: rgba(36,128,198,.5);
//   }

//   .scroll{
//     /**
//     * percent函数转换百分比
//     * $content-height内容区域总高度
//     * 页面标题栏高度：56
//     * 查询条件高度：140
//     * 操作按钮组：70
//     * 表头高度：50
//     */
//     height: percent($content-height - 56 - 140 - 70 - 50, $content-height);

//     .list-con{
//       height: initial;
//       padding: {
//         top: PXtoEm(15);
//         bottom: PXtoEm(15);
//       }
//       &:nth-of-type(even){
//         background: rgba(186,186,186,.5);
//       }

//       /deep/ .el-checkbox__label{
//         display: none;
//       }
//     }
//   }

//   .scroll{
//     overflow-y: hidden;
//     height: 100%;

//     /deep/ .el-scrollbar__view{
//       overflow-y: hidden;
//       width: 100%;
//       height: 100%;

//       .el-scrollbar__view{
//         height: auto;
//       }

//       .el-scrollbar__bar{
//         &.is-vertical{
//           display: block;
//           position: fixed;
//           bottom: 0 !important;
//           top: initial;
//           height: percent($height - 80 - 50, $content-height);
//         }
//       }
//     }

//     /deep/ .el-scrollbar__bar{
//       &.is-horizontal{
//         display: block !important;
//       }

//       &.is-vertical{
//         display: none;
//       }
//     }
//   }

//   // 列表滚动区域
//   .scroll-1{
//     height: percent($height - 80 - 50, $height - 80);
//   }
//   // 所属管廊
//   .gallery-list-wrap{
//     display: flex;
//     justify-content: space-between;
//     margin-left: 10px;

//     .gallery-list{
//       width: 120px;
//       text-align: center;
//       .con{
//         width: 100%;
//         @extend %ellipsis;
//       }
//     }
//   }

//   .list-wrap{
//     .list{
//       &:nth-of-type(even){
//         .list-content{
//           background: rgba(186,186,186,.5);
//         }
//       }
//     }

//     .list-content{
//       display: flex;
//       justify-content: space-between;
//       align-items: center;
//       height: 50px;
//       padding: 0 PXtoEm(24);
//     }
//   }
//   .name,
//   .number{
//     width: 150px;
//     word-break: break-all;
//   }
// }

</style>
