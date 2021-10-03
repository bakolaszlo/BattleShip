<template>
  <div class="backdrop" v-if="!modalLobbyId">
    <div class="modal-wait">
      <p>Lobby id: {{ $route.params.lobby_id }}</p>
      <div>
        <p>Waiting for opponent...</p>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ["modalLobbyId"],
  data() {
    return {
      nickname: "",
    };
  },
  methods: {},
};
</script>
<style scoped>
.modal-wait {
  width: 400px;
  padding: 20px;
  margin: 100px auto;
  background: white;
  border-radius: 10px;
  display: block;
}

.backdrop {
  top: 0;
  position: fixed;
  background: rgba(0, 0, 0, 0.5);
  width: 100%;
  height: 100%;
}
</style>
