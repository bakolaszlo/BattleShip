<template>
  <div class="hello">
    <h3>Battleship</h3>
    <div>
      <h3>Play now!</h3>
      <div class="container">
        <li class="nav-item">
          <router-link :to="{ name: 'Login' }" class="nav-link text-dark"
            >Login</router-link
          >
        </li>
        <li class="nav-item">
          <router-link :to="{ name: 'Register' }" class="nav-link text-dark"
            >Register</router-link
          >
        </li>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "Home",
  props: {
    msg: String,
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
