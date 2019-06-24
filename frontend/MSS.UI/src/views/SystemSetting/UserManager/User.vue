<template>
<el-container style="height:100%">
    <el-header>
        <el-form :inline="true">
            <el-form-item>
            <el-input v-model="searchName" placeholder="用户名称"></el-input>
            </el-form-item>
            <el-form-item>
            <el-cascader  clearable placeholder="权限"
                :props="defaultParams"
                :options="actionTreeInit"
                :show-all-levels="false"
                v-model="searchType">
            </el-cascader>
            </el-form-item>
            <el-form-item>
            <el-button type="primary" icon="el-icon-search" v-on:click="queryUserList">查询</el-button>
            <el-button type="primary" icon="el-icon-plus" @click="handleAdd(1)" >新增</el-button>
            <el-button type="primary" icon="el-icon-edit" @click="handleAdd(2)" :loading="formLoading">修改</el-button>
            <el-button type="primary" icon="el-icon-delete" @click="handleDel">删除</el-button>
            <el-button type="primary" icon="el-icon-setting" @click="handleReSetPwd">密码重置</el-button>
            </el-form-item>
        </el-form>
    </el-header>
    <el-main class="main">
        <el-table v-loading="tableLoading" stripe @sort-change="handleSortChange"
        @expand-change="handleExpandChange"
        :row-key='getRowKeys'
        :expand-row-keys="expands"
        :data="tableData" border style="width:100%;" @selection-change="handleSelectionChange">
            <el-table-column type="selection" width="50" align="center"></el-table-column>
            <el-table-column type="expand">
            <template slot-scope="props">
                <el-row class="row-left" :gutter="20">
                    <div class="row-space" v-for="(ag) in props.row.ActionTrees" :key="ag.id">
                        <el-col :span="spanFirstTitle">{{ag.text}}：</el-col>
                        <el-col :span="spanFirstContent">
                            <el-col :span="spanSecond" v-for="(a) in ag.children" :key="a.id">{{a.text}}</el-col>
                        </el-col>
                    </div>
                </el-row>
            </template>
            </el-table-column>
            <el-table-column
            prop="UserID"
            label="用户ID"
            sortable="custom"
            width="80">
            </el-table-column>
            <el-table-column
            prop="UserName"
            label="用户名称"
            sortable="custom"
            width="150">
            </el-table-column>
            <el-table-column
            prop="Mobile"
            label="电话"
            width="100">
            </el-table-column>
            <el-table-column
            prop="Email"
            label="邮件"
            sortable="custom"
            width="150">
            </el-table-column>
            <el-table-column
            prop="LmTime"
            label="更新时间"
            sortable="custom"
            :formatter="formatDate"
            width="150">
            </el-table-column>
            <el-table-column
            prop="LmName"
            label="更新人"
            sortable="custom">
            </el-table-column>
        </el-table>
        <!-- 分页处理-->
        <div style="text-align: center;margin-top: 30px;">
            <el-pagination background  layout="total,prev, pager, next, jumper"
            :total="total"
            :page-size="pageSize"
            :pager-count="pagerCount"
            :current-page.sync="cur_page"
            @current-change="handleCurrentChange">
            </el-pagination>
        </div>
        <!--新增修改界面-->
        <el-dialog  :title="title"  :visible="dialogVisible === 1 || dialogVisible === 2" :before-close="handleClose">
            <el-form :model="editForm" label-width="150px" :rules="editFormRules" ref="editForm">
                <el-form-item label="用户ID" prop="UserID">
                    <el-input  v-model="editForm.UserID" :disabled="dialogVisible === 2"></el-input>
                </el-form-item>
                <el-form-item label="用户名称" prop="UserName">
                    <el-input  v-model="editForm.UserName"></el-input>
                </el-form-item>
                <el-form-item label="电话" prop="Mobile">
                    <el-input  v-model="editForm.Mobile"></el-input>
                </el-form-item>
                <el-form-item label="邮件" prop="Email">
                    <el-input  v-model="editForm.Email"></el-input>
                </el-form-item>
                <el-form-item label="参考角色">
                <el-select v-model="roleID" clearable placeholder="参考角色"  @change="handleRoleChange">
                    <el-option
                    v-for="item in roleList"
                    :key="item.RoleID"
                    :label="item.RoleName"
                    :value="item.RoleID">
                    </el-option>
                </el-select>
                </el-form-item>
                <el-form-item label="权限">
                    <el-checkbox :indeterminate="isIndeterminate" v-model="checkAll" @change="handleCheckAllChange">全选</el-checkbox>
                </el-form-item>
                <el-form-item v-for="ag in actionTreeEdit" :key="ag.id">
                    <el-checkbox-group v-model="checkedGroup">
                        <el-checkbox :indeterminate="ag.isIndeterminate"
                        :label="ag.id" :key="ag.id"
                        @change="handleCheckActionAllChange(ag)">{{ag.text}}</el-checkbox>
                        <el-checkbox-group v-model="ag.checkedChildren" @change="handleCheckedActionsChange(ag)">
                            <el-col :span="spanSecondCheck" v-for="a in ag.children" :key="a.id"><el-checkbox :label="a.id">{{a.text}}</el-checkbox></el-col>
                        </el-checkbox-group>
                    </el-checkbox-group>
                </el-form-item>
            </el-form>
            <div slot="footer" class="dialog-footer">
                <el-button @click.native="handleClose">取消</el-button>
                <el-button type="primary" @click.native="addSubmit" :loading="editLoading">提交</el-button>
            </div>
        </el-dialog>
    </el-main>
</el-container>
</template>
<script>
import api from '@/api/SystemSettingApi'
import {PageSize, PagerCount, transformDate, ERR_SHOW, SUCCESS_SHOW, FORM_TITLE} from '@/common/js/utils.js'
export default {
    data () {
        return {
            // 分页
            pageSize: PageSize,
            pagerCount: PagerCount,
            cur_page: 1,
            total: 0,
            // 参考角色下拉
            roleID: '',
            roleList: [],
            // 级联下拉
            // 也是新增默认的权限结构
            actionTreeInit: [],
            defaultParams: {
                label: 'text',
                value: 'id',
                children: 'children'
            },
            // 查询条件
            searchName: '',
            searchType: [],
            sort: 'UserID',
            order: 'asc',
            // table
            tableData: [],
            multipleSelection: [],
            tableLoading: false,
            expands: [],
            getRowKeys (row) {
                return row.UserID
            },
            // 展开项
            spanFirstTitle: 3,
            spanFirstContent: 21,
            spanSecond: 4,
            // dialog
            // 1新增，2修改，0全不显示
            dialogVisible: 0,
            title: '',
            formLoading: false,
            editLoading: false,
            editForm: {
                UserID: '',
                UserName: '',
                Mobile: '',
                Email: '',
                Actions: ''
            },
            editFormRules: {
                UserName: [
                    { required: true, message: '请输入用户名', trigger: 'blur' }
                ],
                UserID: [
                    { required: true, message: '请输入用户ID', trigger: 'blur' }
                ]
            },
            // checkAll
            actionTreeEdit: [],
            checkAll: false,
            checkedGroup: [],
            isIndeterminate: false,
            spanSecondCheck: 6
        }
    },
    created () {
        this.getActionTree()
        this.getUserList()
        this.getRoleList()
    },
    methods: {
        formatDate (row, column) {
            var date = row[column.property]
            if (date === '/Date(1551232934000)/') {
                return ''
            } else {
                return transformDate(date)
            }
        },
        getActionTree: function () {
            api.getActionTree().then((res) => {
                if (res.isSuccess) {
                    this.actionTreeInit = res.queryList
                    this.actionTreeEdit = res.queryList
                } else {
                    this.$message.error(ERR_SHOW[res.errType])
                }
            })
        },
        getRoleList: function () {
            api.getRoleAll().then((res) => {
                if (res.isSuccess) {
                    this.roleList = res.queryList
                } else {
                    this.$message.error(ERR_SHOW[res.errType])
                }
            })
        },
        queryUserList () {
            this.cur_page = 1
            this.getUserList()
        },
        getUserList () {
            let params = {
                page: this.cur_page,
                rows: this.pageSize,
                sort: this.sort,
                order: this.order,
                SearchName: this.searchName.trim(),
                SearchType: this.searchType[1]
            }
            this.tableLoading = true
            api.getUserList(params).then((res) => {
                this.tableData = res.rows
                this.total = res.total
                this.tableLoading = false
            })
        },
        handleCurrentChange (val) {
            this.cur_page = val
            this.getUserList()
        },
        handleSelectionChange (val) {
            this.multipleSelection = val
        },
        handleSortChange (val) {
            this.sort = val.prop
            if (val.order === 'descending') {
                this.order = 'desc'
            } else {
                this.order = 'asc'
            }
            this.getUserList()
        },
        handleDel () {
            if (this.multipleSelection.length === 0) {
                this.$message.warning('请选择您要删除的数据!')
            } else {
                // this.dialogVisible = 3
                this.$confirm('此操作将永久删除该记录, 是否继续?', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then(() => {
                    this.deleteRow()
                })
            }
        },
        deleteRow () {
            var strids = ''
            for (var i = 0; i < this.multipleSelection.length; i++) {
                strids = strids + this.multipleSelection[i].UserID + ','
            }
            strids = strids.slice(0, strids.length - 1)
            var params = {
                ids: strids
            }
            api.deleteUser(params).then((res) => {
                if (res.isSuccess) {
                    this.$message.success(SUCCESS_SHOW.Delete)
                    this.multipleSelection = []
                    this.cur_page = 1
                    this.getUserList()
                } else {
                    this.$message.error(ERR_SHOW[res.errType])
                }
            })
        },
        handleClose () {
            this.dialogVisible = 0
        },
        handleReSetPwd () {
            if (this.multipleSelection.length === 0) {
                this.$message.warning('请选择您要重置密码的用户!')
            } else {
                // this.dialogVisible = 3
                this.$confirm('您所选择的用户密码将被重置为初始密码, 是否继续?', '提示', {
                    confirmButtonText: '确定',
                    cancelButtonText: '取消',
                    type: 'warning'
                }).then(() => {
                    this.reSetPwd()
                })
            }
        },
        reSetPwd () {
            var strids = ''
            for (var i = 0; i < this.multipleSelection.length; i++) {
                strids = strids + this.multipleSelection[i].UserID + ','
            }
            strids = strids.slice(0, strids.length - 1)
            var params = {
                ids: strids
            }
            api.reSetPwd(params).then((res) => {
                if (res.isSuccess) {
                    this.$message.success('密码重置成功')
                    this.multipleSelection = []
                    this.getUserList()
                } else {
                    this.$message.error(ERR_SHOW[res.errType])
                }
            })
        },
        handleAdd (isAdd) {
            if (this.$refs['editForm'] !== undefined) {
                this.$refs['editForm'].resetFields()
            }
            if (isAdd === 1) {
                this.dialogVisible = 1
                this.title = FORM_TITLE.Add
                this.editForm = {
                    UserID: '',
                    UserName: '',
                    Mobile: '',
                    Email: ''
                }
                this.actionTreeEdit = [].concat(this.actionTreeInit)
                this.isIndeterminate = false
                this.checkAll = false
                this.checkedGroup = []
            } else if (isAdd === 2) {
                if (this.multipleSelection.length === 1) {
                    this.title = FORM_TITLE.Update
                    let item = this.multipleSelection[0]
                    this.editForm = {
                        UserID: item.UserID,
                        UserName: item.UserName,
                        Mobile: item.Mobile,
                        Email: item.Email
                    }
                    this.formLoading = true
                    api.getActionTreeByUserID({ID: item.UserID}).then((res) => {
                        this.loadAction(res)
                        this.dialogVisible = 2
                    })
                } else {
                    this.$message.warning('每次只能修改一行数据，你已经选择了' + this.multipleSelection.length + '行！')
                }
            }
        },
        handleRoleChange (val) {
            if (val !== '') {
                api.getActionTreeByRoleID({ID: val}).then((res) => {
                    this.loadAction(res)
                })
            } else {
                this.actionTreeEdit = [].concat(this.actionTreeInit)
                this.isIndeterminate = false
                this.checkAll = false
                this.checkedGroup = []
            }
        },
        loadAction (res) {
            if (res.isSuccess) {
                this.actionTreeEdit = res.queryList
                let mycheck = true
                this.checkedGroup = []
                this.checkAll = false
                this.isIndeterminate = false
                this.actionTreeEdit.forEach(at => {
                    if (at.isIndeterminate) {
                        mycheck = false
                        this.isIndeterminate = true
                        this.checkedGroup.push(at.id)
                    } else if (at.checkedChildren.length > 0) {
                        this.checkedGroup.push(at.id)
                    }
                })
                if (mycheck) {
                    this.checkAll = true
                    this.isIndeterminate = false
                }
            } else {
                this.$message.error(ERR_SHOW[res.errType])
            }
            this.formLoading = false
        },
        addSubmit () {
            this.$refs['editForm'].validate((valid) => {
                if (valid) {
                    this.editForm.Actions = ''
                    this.actionTreeEdit.forEach(at => {
                        at.checkedChildren.forEach(c => {
                            this.editForm.Actions += c + ','
                        })
                    })
                    if (this.editForm.Actions === '') {
                        this.$message.warning('至少选择一个权限')
                        return
                    }
                    this.editLoading = true
                    this.editForm.Actions = this.editForm.Actions.slice(0, this.editForm.Actions.length - 1)
                    if (this.dialogVisible === 1) {
                        api.addUser(this.editForm).then((res) => {
                            if (res.isSuccess === true) {
                                this.$message.success(SUCCESS_SHOW.Add)
                                // 更新table
                                this.getUserList()
                            } else {
                                this.$message.error(ERR_SHOW[res.errType])
                            }
                            this.dialogVisible = 0
                            this.editLoading = false
                        })
                    } else if (this.dialogVisible === 2) {
                        api.updateUser(this.editForm).then((res) => {
                            if (res.isSuccess === true) {
                                this.$message.success(SUCCESS_SHOW.Update)
                                // 更新table
                                this.getUserList()
                            } else {
                                this.$message.error(ERR_SHOW[res.errType])
                            }
                            this.dialogVisible = 0
                            this.editLoading = false
                        })
                    }
                } else {
                    this.editLoading = false
                    return false
                }
            })
        },
        handleCheckAllChange (val) {
            this.checkedGroup = []
            this.actionTreeEdit.forEach(at => {
                at.isIndeterminate = false
                at.checkedChildren = []
                if (val) {
                    this.checkedGroup.push(at.id)
                    at.children.forEach(c => {
                        at.checkedChildren.push(c.id)
                    })
                }
            })
            this.isIndeterminate = false
        },
        handleCheckedActionsChange (ag) {
            let checkedCount = ag.checkedChildren.length
            this.isIndeterminate = false
            let index = this.checkedGroup.findIndex(v => {
                return v === ag.id
            })
            if (checkedCount > 0) {
                if (index === -1) {
                    this.checkedGroup.push(ag.id)
                }
            } else {
                this.checkedGroup.splice(index, 1)
            }
            ag.isIndeterminate = checkedCount > 0 && checkedCount < ag.children.length
            let myIndex = this.actionTreeEdit.findIndex(at => {
                this.isIndeterminate = true
                return at.checkedChildren.length !== at.children.length
            })
            if (myIndex === -1) {
                this.checkAll = true
                this.isIndeterminate = false
            } else if (this.checkedGroup.length === 0) {
                this.checkAll = false
                this.isIndeterminate = false
            }
        },
        handleCheckActionAllChange (ag) {
            let checkedCount = this.checkedGroup.length
            this.checkAll = checkedCount === this.actionTreeEdit.length
            this.isIndeterminate = checkedCount > 0 && checkedCount < this.actionTreeEdit.length
            if (this.checkedGroup.findIndex(v => {
                return v === ag.id
            }) > -1) {
                ag.children.forEach(c => {
                    ag.checkedChildren.push(c.id)
                })
            } else {
                ag.checkedChildren = []
            }
            ag.isIndeterminate = false
        },
        // 折叠面板每次只能展开一行
        handleExpandChange (row, expandedRows) {
            if (expandedRows.length) {
                this.expands = []
                if (row) {
                    this.expands.push(row.UserID)
                }
            } else {
                this.expands = []
            }
        }
    }
}

</script>
<style lang="scss" scoped>
.row-space {
    padding-bottom: 40px;
}
.row-left {
    padding-left: 60px;
}
.box-card {
    margin-left: 110px;
}
</style>
