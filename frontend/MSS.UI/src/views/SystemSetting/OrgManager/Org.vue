<template>
<el-container style="height:100%">
    <el-header>
        <el-form :inline="true">
            <el-form-item>
            <el-button type="primary" icon="el-icon-plus" @click="addOrgNode()">新增</el-button>
            <el-dropdown @command="handleCommand">
                <el-button type="primary">
                    新增<i class="el-icon-arrow-down el-icon--right"></i>
                </el-button>
                <el-dropdown-menu slot="dropdown">
                    <el-dropdown-item command="1">公司</el-dropdown-item>
                    <el-dropdown-item command="2">部门</el-dropdown-item>
                    <el-dropdown-item command="3">班组</el-dropdown-item>
                </el-dropdown-menu>
            </el-dropdown>
            <el-button type="primary" icon="el-icon-edit" @click="updateOrgNode()">修改</el-button>
            <el-button type="primary" icon="el-icon-delete">删除</el-button>
            </el-form-item>
        </el-form>
    </el-header>
    <el-main class="main">
        <el-container style="height:100%">
            <el-aside width="200px">
                <el-tree ref="orgtree" :data="data" :props="defaultProps"
                    show-checkbox check-strictly node-key="id"
                    @node-click="onNodeClick" ></el-tree>
            </el-aside>
            <el-main>
                <router-view :key="curID"></router-view>
            </el-main>
        </el-container>
    </el-main>
</el-container>
</template>
<script>
import api from '@/api/OrgApi'
import {RESULT} from '@/common/js/utils.js'
import eventBus from '@/components/EventBus'
export default {
    data () {
        return {
            curID: null,
            data: [],
            defaultProps: {
                children: 'children',
                label: 'label'
            }
        }
    },
    created () {
        eventBus.$on('submit', data => {
            this.submit(data)
        })
        this.getOrgAll()
    },
    methods: {
        onNodeClick (data) {
            this.curID = data.id
            this.$router.push({
                name: 'see',
                query: {id: data.id}
            })
        },
        getOrgAll (val) {
            api.getOrgAll().then((res) => {
                if (res.result === RESULT.Success) {
                    this.data = res.data
                    this.$refs.orgtree.setCurrentKey(13)
                } else {
                    this.$message.error('加载组织结构失败')
                }
            })
        },
        submit (val) {
            if ((val.ID + '') === '0') {
                api.addOrgNode(val).then((res) => {
                    if (res.result === RESULT.Success) {
                        this.$refs.orgtree.append({
                            id: res.data.id,
                            label: res.data.name,
                            node_type: res.data.nodeType
                        }, res.data.parentID)
                        this.$message.success('保存成功')
                    } else if (res.result === RESULT.Reinsert) {
                        this.$message.error('名称重复')
                    } else {
                        this.$message.error('保存失败')
                    }
                })
            } else {
                api.updateOrgNode(val.ID, val).then((res) => {
                    if (res.result === RESULT.Success) {
                        let node = this.$refs.orgtree.getNode(val.ID)
                        node.setData({
                            id: res.data.id,
                            label: res.data.name,
                            node_type: res.data.nodeType
                        })
                        this.$set(this.data, 'label', node.name)
                        this.$message.success('更新成功')
                    } else if (res.result === RESULT.Reinsert) {
                        this.$message.error('名称重复')
                    } else {
                        this.$message.error('保存失败')
                    }
                })
            }
        },
        handleCommand (command) {
            let nodes = this.$refs.orgtree.getCheckedNodes()
            let parentID = null
            if (nodes.length !== 0) {
                if (nodes.length > 1) {
                    this.$message.error('只能选择一个父节点')
                    return
                } else {
                    parentID = nodes[0].id
                }
            }
            this.curID = 0
            this.CurNode = {
                id: 0,
                parentID: parentID
            }
            switch (command) {
            case '1':
                this.$router.push({
                    name: 'company',
                    query: this.CurNode
                })
                break
            case '2':
                this.$router.push({
                    name: 'section',
                    query: this.CurNode
                })
                break
            case '3':
                this.$router.push({
                    name: 'company',
                    query: this.CurNode
                })
                break
            }
        },
        addOrgNode () {
            let nodes = this.$refs.orgtree.getCheckedNodes()
            let parentID = null
            if (nodes.length !== 0) {
                if (nodes.length > 1) {
                    this.$message.error('只能选择一个父节点')
                    return
                } else {
                    parentID = nodes[0].id
                }
            }
            let param = {
                id: 0,
                nodeType: '1',
                parentID: parentID
            }
            this.$router.push({
                name: 'company',
                query: param
            })
        },
        updateOrgNode () {
            let nodes = this.$refs.orgtree.getCheckedNodes()
            if (nodes.length !== 0) {
                if (nodes.length > 1) {
                    this.$message.error('只能选择一个节点')
                } else {
                    this.curID = nodes[0].id
                    let param = {
                        id: this.curID
                    }
                    switch (nodes[0].node_type) {
                    case 1:
                        this.$router.push({
                            name: 'company',
                            query: param
                        })
                        break
                    case 2:
                        this.$router.push({
                            name: 'section',
                            query: param
                        })
                        break
                    case 3:
                        this.$router.push({
                            name: 'team',
                            query: param
                        })
                        break
                    }
                }
            }
        }
    }
}

</script>
<style lang="scss" scoped>
</style>
