<template>
  <header>
    <nav
      class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"
    >
      <div class="container">
        <div
          class="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse"
        >
          <ul class="navbar-nav flex-grow">
            <li class="nav-item">
              <router-link :to="{ name: 'Home' }" class="nav-link text-dark"
                >Home</router-link
              >
            </li>
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
          </ul>
        </div>
      </div>
    </nav>
  </header>
</template>

<style>
a.navbar-brand {
  white-space: normal;
  text-align: center;
  word-break: break-all;
}

@media (min-width: 768px) {
  html {
    font-size: 16px;
  }
}

.box-shadow {
  box-shadow: 0 0.25rem 0.75rem rgba(0, 0, 0, 0.05);
}
</style>
<script>
export default {
  name: "NavMenu",
  data() {
    return {};
  },
  methods: {},
};
</script>
