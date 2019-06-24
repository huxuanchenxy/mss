<template>
    <el-row style="height:100%;background:#20a0ff">
        <el-col :span="24">
            <div class="grid-content copyright">{{ copyright }}</div>
        </el-col>
    </el-row>
</template>
<script>
export default {
    name: 'myFooter',
    data () {
        return {
            copyright: '©2019 上海电气自动化设计研究所有限公司'
        }
    },
    methods: {
    }
}
</script>
<style>
.grid-content {
    border-radius: 4px;
    min-height: 40px;
    height: 40px;
}
.copyright{
    left: 50%;
    font-size: 13px;
    text-align: center;
    color: #fff;
    line-height: 40px;
}
</style>
