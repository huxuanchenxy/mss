<template>
  <div v-if="selfShow">
    <button @click="changeShowChild" >to hidden</button>
  </div>
</template>
<script>
export default {
  methods: {
    changeShowChild () {
      this.$emit('changeShowChild')
    }
  },
  computed: {
    selfShow () {
      return this.$attrs.name
    }
  }
}
</script>
<style>
</style>
