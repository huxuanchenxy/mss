<template>
  <button class="btn">
    <slot></slot>
  </button>
</template>
<script>
export default {
  name: 'XButton',
  data () {
    return {

    }
  }
}
</script>
<style lang="scss" scoped>
.btn{
  min-width: $btn-width-base - 16;
  height: $btn-height-base;
  margin-left: 10px;
  padding: 0 8px;
  border: 1px solid $color-main-btn-border;
  border-radius: $border-radius;
  color: $color-white;
  cursor: pointer;

  &:first-of-type{
    margin-left: 0;
  }

  &.small{
    min-width: $btn-width-small;
    height: $btn-height-small;
    padding: 0;
  }

  &.close{
    border-color:  #C64A24;
    background-color:  #C64A24;
    &:hover{
      border-color:  #C64A24;
      background-color:  #C64A24;
    }
  }

  &.active,
  &:hover{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }

  a{
    display: block;
    color: $color-white;
  }
}
</style>
