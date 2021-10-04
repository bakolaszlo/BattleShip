<template>
  <div class="hello">
    <div v-if="showModal">
      <CreateModal @close="toggleModal" />
    </div>
    <div v-if="showJoinModal">
      <JoinModal @close="toggleJoinModal" />
    </div>
    <h3>Battleship</h3>
    <div>
      <h3>Play now!</h3>
      <div class="container">
        <li class="nav-item">
          <button class="nav-link text-dark" @click="toggleModal">
            Create Game
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link text-dark" @click="toggleJoinModal">
            Join Game
          </button>
        </li>
      </div>
    </div>
  </div>
</template>

<script>
import CreateModal from "./CreateModal.vue";
import JoinModal from "./JoinModal.vue";

export default {
  name: "Home",
  components: { CreateModal, JoinModal },
  data() {
    return {
      showModal: false,
      showJoinModal: false,
    };
  },
  methods: {
    toggleModal() {
      this.showModal = !this.showModal;
    },
    toggleJoinModal() {
      this.showJoinModal = !this.showJoinModal;
    },
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
