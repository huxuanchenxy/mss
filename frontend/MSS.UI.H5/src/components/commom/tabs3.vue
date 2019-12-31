<template>
    <div class="wrap">
        <!-- tabs -->
        <mu-tabs class="tabs" :value="activeTab" @change="handleTabChange">
            <mu-tab value="notification" title="通知" />
            <mu-tab value="alarm" title="报警" />
            <mu-tab value="prealarm" title="预警" />
            <!-- <mu-tab value="share" title="分享" />
            <mu-tab value="ask" title="问答" />
            <mu-tab value="job" title="招聘" /> -->
        </mu-tabs>
    </div>
</template>
<script>
    import Bus from './Bus'
    import axios from 'axios'
    import timeago from 'timeago.js'
    import api from '@/api/eqpApi'
    import { systemResource } from '@/common/js/dictionary.js'
    export default {
        components: {
            
        },
        data() {
            return {
                loading: false,
                scroller: null,
                nomore: false,
                activeTab: 'notification', //当前选中tab项
                items: [],
                styleObj: {
                    backgroundColor: '#999'
                },
                url: 'http://www.vue-js.com/api/v1/topics?tab=all',
                page: 1,
                eqpTypeOnly: '',
                eqpTypeList: [],  
                unSelectedEqp: systemResource.eqp,
                saveEqpLoading: false,
                unSelectedEqpType: systemResource.eqpType,
                systemResource: systemResource.eqpType,
                eqpTypeFileIDs: '',
                readOnly: false
            }
        },
        created() {
            // this.getData()
            // this.InitSelect()
        },
        mounted() {
            this.scroller = this.$el
        },
        filters: {
            timeago(val) {
                let time = new Date(val)
                var thistime = timeago()
                return thistime.format(time, 'zh_CN') //将UTC时间转换格式---> 几天前,几小时前...
            }
        },
        methods: {
            handleTabChange(val) {
                this.page = 1 //切换tab，页数重置为第一页
                this.nomore = false //切换tab，重置
                this.activeTab = val
                this.url = 'http://www.vue-js.com/api/v1/topics?tab=' + val
                Bus.$emit('addattr', val)
            },
            getData() {
                let that = this
                    // let url = 'http://www.vue-js.com/api/v1/topics?tab=all'
                    // let url = this.url + '&page=' + this.num
                axios.get(this.url).then(function(response) {
                    that.items = response.data.data
                        // console.log(that.items)
                })
            }
        }
    }
</script>
<style lang="scss" scoped>
    .main {
        /* overflow: auto; */
        /*overflow-scrolling: touch;*/
        border: 1px solid #d9d9d9;
        /* width:400px; */
    }

    .tab .tabs {
        margin-top: 58px;
        height: 52px;
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        border-top: 1px solid rgba(255,255,255,.1);
        display: flex;
        justify-content: space-around;
        z-index:0;
    }
    .mu-tab-link,.mu-tab-active{
        white-space: nowrap;
    }
    .list {
        display: flex;
        border-bottom: 1px solid #999;
        padding: 1rem;
    }

    .list>img {
        width: 4rem;
        height: 4rem;
        border-radius: 50%;
        margin-right: 1rem;
    }

    .content {
        flex: 1;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
    }

    .list_title {
        /*display: flex;*/
    }

    .list_title>span {
        /*white-space:nowrap;*/
        background-color: #009688;
        color: #fff;
        padding: 0.2rem;
        border-radius: 0.2rem;
    }

    .list_title>h3 {
        display: inline;
        font-weight: 700;
    }

    .timer {
        display: flex;
        justify-content: space-between;
        color: #999;
    }

    .nomore {
        text-align: center;
        padding: 1rem 0;
    }
    .el-scrollbar {
        width:100%;
        position: absolute;
    }
    .scroll {
        /* width:400px; */
    }
    .wrapper {
        /* width:400px; */
        touch-action:none !important;
        touch-action:pan-y !important;
    }
    body {
        background:unset !important;
    }
    .wrap{
        position: relative;
        width: 100%;
        height: 100%;
    }
</style>
