<template>
  <header-vue></header-vue>
  <router-view />
</template>

<script>
import HeaderVue from "./components/Header.vue";

export default {
  name: "App",
  components: {
    HeaderVue,
  },
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>
