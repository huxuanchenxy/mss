<template>
<div>
    <h2 class="title">
      <img :src="{ titlepic }" alt="" class="icon">{{ title }}
    </h2>
</div>
</template>
<script>
import XButton from '@/components/button'
export default {
  name: 'TitleModule',
  components: {
    XButton
  },
  mounted () {
    this.InitTitle()
  },
  data () {
    return {
      title: '',
      titlepic: ''
    }
  },
  methods: {
    InitTitle () {
      let navlist = this.$router.navList
      if (navlist !== undefined) {
        let r = this.$router.navList[this.$route.matched[0].path]
        let rc = r.children
        this.titlepic = this.$router.navList[this.$route.matched[0].path].iconClsActive
        let thisroutepath = this.$route.path
        let t2 = ''
        for (let index in rc) {
          let comp = thisroutepath.indexOf(rc[index].path)
          if (comp >= 0) {
            t2 = rc[index].name
            break
          }
        }
        console.log(t2)
        this.title = this.$router.navList[this.$route.matched[0].path].name + ' | ' + t2
        window.sessionStorage.setItem('title', this.title)
        window.sessionStorage.setItem('titlepic', this.titlepic)
      } else {
        this.title = window.sessionStorage.getItem('title')
        this.titlepic = window.sessionStorage.getItem('titlepic')
      }
    }
  }
}
</script>
<style lang="scss" scoped>
</style>
