<template>
  <div class="backdrop" @click.self="closeModal">
    <div class="modal-create">
      <p>Enter lobby id</p>
      <input v-model="lobbyId" />
      <p>Enter your Nickname:</p>
      <input type="text" v-model="nickname" required />
      <button @click="createPlayer">Start Game</button>
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      nickname: "",
      lobbyId: null,
    };
  },
  methods: {
    closeModal() {
      this.$emit("close");
    },
    createPlayer() {
      if (window.localStorage.getItem("playerId") == null) this.joinLobby();
      var axios = require("axios");
      var data = JSON.stringify({
        Nickname: this.nickname,
      });

      var config = {
        method: "post",
        url: "/api/players",
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      };

      axios(config)
        .then((response) => {
          window.localStorage.setItem("playerId", response.data);
          this.joinLobby();
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    joinLobby() {
      var axios = require("axios");
      var data = JSON.stringify({
        Guest: window.localStorage.getItem("playerId"),
      });

      var config = {
        method: "put",
        url: "/api/lobbies/" + this.lobbyId,
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      };

      axios(config)
        .then(() => {
          this.$router.push({
            name: "Battleship",
            params: { lobby_id: this.lobbyId },
          });
        })
        .catch(function(error) {
          console.log(error);
        });
    },
  },
};
</script>
<style scoped>
.modal-create {
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
