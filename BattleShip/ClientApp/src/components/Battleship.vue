<template>
  <div v-if="!modalLobbyId && !isGameOverB">
    <WaitModal />
  </div>
  <div v-if="isGameOverB">
    <GameOverModal />
  </div>
  <div class="container">
    <div class="player-container">
      <div>Your board</div>
      <div class="grid grid-user" ref="usergrid">
        <div
          v-for="(n, index) in 100"
          :key="n"
          @click="putShip(index)"
          :id="(n - 1).toString()"
        >
          <div
            v-if="userSquares[index]"
            :class="userSquares[index].shipName"
            @click="removeFromGrid(index)"
          ></div>
        </div>
      </div>
    </div>

    <div class="enemy-container">
      <div>Enemy board</div>
      <div class="grid grid-enemy" ref="enemygrid">
        <div
          v-for="(n, index) in 100"
          :key="n"
          @click="attackEnemy(index)"
          :id="(n - 1).toString()"
        >
          <div v-if="enemySquares[index]" :class="enemySquares[index]"></div>
        </div>
      </div>
    </div>
  </div>

  <div class="hidden-info" v-if="!canGameStart">
    <button
      class="start"
      v-if="letShipsToSelectFrom.length == 0"
      @click="registerMatch"
      :disabled="isReadyDisabled"
    >
      I'm Ready
    </button>
    <button class="rotate" @click="rotateShip">Rotate</button>
  </div>
  <h3 class="info">{{ message }}</h3>
  <div v-if="canGameStart && !isGameWon">
    <div v-if="(isHost && isHostTurn) || (!isHost && !isHostTurn)">
      It's your turn!
    </div>
    <div v-else>It's {{ opponentName }} turn!</div>
  </div>
  <div class="grid-display" v-if="!canGameStart">
    Your ships:
    <div
      v-for="ship in letShipsToSelectFrom"
      :key="ship"
      :class="'ship ' + ship.name + '-container ' + ship.isSelected"
      @click="selectShip(ship)"
    >
      <div
        v-for="n in ship.shipLength"
        :key="n"
        :id="ship.name + '-' + (n - 1)"
      ></div>
    </div>
  </div>
</template>

<script>
import WaitModal from "./WaitModal.vue";
import GameOverModal from "./GameOverModal.vue";
import * as signalR from "@aspnet/signalr";
export default {
  components: { WaitModal, GameOverModal },
  data() {
    return {
      ships: [
        {
          name: "destroyer",
          shipLength: 2,
          rotation: 1,
          isInGrid: false,
          shipIndex: 0,
          isSelected: false,
        },
        {
          name: "submarine",
          shipLength: 3,
          rotation: 1,
          isInGrid: false,
          shipIndex: 1,
          isSelected: false,
        },
        {
          name: "cruiser",
          shipLength: 3,
          rotation: 1,
          isInGrid: false,
          shipIndex: 2,
          isSelected: false,
        },
        {
          name: "battleship",
          shipLength: 4,
          rotation: 1,
          isInGrid: false,
          shipIndex: 3,
          isSelected: false,
        },
        {
          name: "carrier",
          shipLength: 5,
          rotation: 1,
          isInGrid: false,
          shipIndex: 4,
          isSelected: false,
        },
      ],
      width: 10,
      enemySquares: [],
      userSquares: [],
      isShipSelected: false,
      selectedShip: null,
      selectedShipIndex: null,
      message: "",
      modalLobbyId: null,
      guestName: null,
      isHost: false,
      canGameStart: false,
      isHostTurn: true,
      opponentName: "opponent",
      isReadyDisabled: false,
      isGameWon: false,
      isGameOverB: false,
    };
  },
  computed: {
    letShipsToSelectFrom() {
      return this.ships.filter((x) => !x.isInGrid);
    },
  },
  mounted() {
    this.setDirectionsForShips();
    var connection = new signalR.HubConnectionBuilder()
      .withUrl("/messagehub")
      .build();

    connection
      .start()
      .then(function() {
        console.log("Connection established..");
      })
      .catch(function(err) {
        return console.error(err.toString());
      });
    connection.on("userJoinedLobby", (modalLobbyId, joinerNickname) => {
      if (modalLobbyId != this.$route.params.lobby_id) {
        return;
      }
      this.modalLobbyId = modalLobbyId;
      this.opponentName = joinerNickname;
      this.isHost = true;
    });
    connection.on("hostHitOn", (lobbyId, index) => {
      if (lobbyId != this.$route.params.lobby_id) {
        return;
      }
      if (this.isHost) {
        this.enemySquares[index] = "hit";
      } else {
        this.userSquares[index] = {};
        this.userSquares[index].shipName = "hit";
      }
      this.isHostTurn = !this.isHostTurn;
      console.log("hostHitOn", this.isHostTurn);
    });

    connection.on("hostAttackedOn", (lobbyId, index) => {
      if (lobbyId != this.$route.params.lobby_id) {
        return;
      }
      if (this.isHost) {
        this.enemySquares[index] = "miss";
      } else {
        this.userSquares[index] = {};
        this.userSquares[index].shipName = "miss";
      }
      this.isHostTurn = !this.isHostTurn;
      console.log("hostAttackedOn", this.isHostTurn);
    });

    connection.on("guestHitOn", (lobbyId, index) => {
      if (lobbyId != this.$route.params.lobby_id) {
        return;
      }
      if (!this.isHost) {
        this.enemySquares[index] = "hit";
      } else {
        this.userSquares[index] = {};
        this.userSquares[index].shipName = "hit";
      }
      this.isHostTurn = !this.isHostTurn;
      console.log("guestHitOn", this.isHostTurn);
    });

    connection.on("guestAttackedOn", (lobbyId, index) => {
      if (lobbyId != this.$route.params.lobby_id) {
        return;
      }
      if (!this.isHost) {
        this.enemySquares[index] = "miss";
      } else {
        this.userSquares[index] = {};
        this.userSquares[index].shipName = "miss";
      }
      this.isHostTurn = !this.isHostTurn;
      console.log("guestAttackedOn", this.isHostTurn);
    });

    connection.on("winner", (lobbyId, whoWon) => {
      if (lobbyId != this.$route.params.lobby_id) {
        return;
      }
      if (window.localStorage.getItem("playerId") == whoWon) {
        this.message = "You are winner !";
        this.isGameWon = true;
      } else {
        this.message = "Your opponent has won!";
        this.isGameWon = true;
      }
      this.canGameStart = false;
    });

    connection.on("bothPlayersAreReady", (host, guest) => {
      let playerId = window.localStorage.getItem("playerId");
      if (playerId == host) this.canGameStart = true;
      if (playerId == guest) this.canGameStart = true;
    });
    this.isGameOver();
    this.isGuest();
  },
  methods: {
    isGameOver() {
      var axios = require("axios");

      var config = {
        method: "get",
        url: "/api/lobbies/" + this.$route.params.lobby_id,
      };

      axios(config)
        .then((response) => {
          if (response.data.isOver) {
            this.isGameOverB = true;
          }
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    registerMatch() {
      if (this.isHost) {
        this.registerHost();
      } else {
        this.registerGuest();
      }
    },
    registerGuest() {
      let boardString = "";
      for (let i = 0; i < this.width * this.width; ++i) {
        if (this.userSquares[i]) {
          boardString += this.userSquares[i].shipIndex;
        } else {
          boardString += "w";
        }
      }
      var axios = require("axios");
      var data = JSON.stringify({
        LobbyId: this.$route.params.lobby_id,
        GuestId: window.localStorage.getItem("playerId"),
        GuestBoard: boardString,
      });

      var config = {
        method: "post",
        url: "/api/matches",
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      };

      axios(config)
        .then((response) => {
          console.log(JSON.stringify(response.data));
          this.isReadyDisabled = true;
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    registerHost() {
      let boardString = "";
      for (let i = 0; i < this.width * this.width; ++i) {
        if (this.userSquares[i]) {
          boardString += this.userSquares[i].shipIndex;
        } else {
          boardString += "w";
        }
      }
      var axios = require("axios");
      var data = JSON.stringify({
        LobbyId: this.$route.params.lobby_id,
        HostId: window.localStorage.getItem("playerId"),
        HostBoard: boardString,
        IsHostTurn: true,
      });

      var config = {
        method: "post",
        url: "/api/matches",
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      };

      axios(config)
        .then(function(response) {
          console.log(JSON.stringify(response.data));
          this.isReadyDisabled = true;
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    isGuest() {
      var axios = require("axios");

      var config = {
        method: "get",
        url: "/api/lobbies/" + this.$route.params.lobby_id,
        headers: {},
      };

      axios(config)
        .then((response) => {
          if (!response.data.isOver && response.data.guest) {
            this.modalLobbyId = this.$route.params.lobby_id;
            this.getOpponentNameAsGuest(response.data.host);
          }
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    getOpponentNameAsGuest(hostId) {
      var axios = require("axios");

      var config = {
        method: "get",
        url: "/api/players/" + hostId,
        headers: {},
      };

      axios(config)
        .then((response) => {
          this.opponentName = response.data.nickname;
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    attackEnemy(index) {
      if (!this.canGameStart) {
        this.message = "The game is not started yet.";
        return;
      }
      if (this.isGameWon) {
        return;
      }
      if (this.isHostTurn && this.isHost) {
        this.attack(index);
      } else if (!this.isHostTurn && !this.isHost) {
        this.attack(index);
      } else {
        this.message = "It's not your turn.";
      }
    },
    attack(index) {
      var axios = require("axios");
      var data = {
        LobbyId: this.$route.params.lobby_id,

        AttackIndex: index,
      };
      if (this.isHost) {
        data.hostId = window.localStorage.getItem("playerId");
      } else {
        data.guestId = window.localStorage.getItem("playerId");
      }
      data = JSON.stringify(data);
      var config = {
        method: "put",
        url: "/api/matches/" + this.$route.params.lobby_id,
        headers: {
          "Content-Type": "application/json",
        },
        data: data,
      };

      axios(config)
        .then((response) => {
          console.log(JSON.stringify(response.data));
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    removeFromGrid(clickedIndex) {
      if (this.canGameStart) {
        return;
      }
      let startPoint = this.userSquares[clickedIndex].start;
      let index = startPoint;
      let shipIndex = this.userSquares[startPoint].shipIndex;
      let nextIndex = this.userSquares[index].next;
      while (this.userSquares[index].next) {
        nextIndex = this.userSquares[index].next;
        this.userSquares[index] = null;
        index = nextIndex;
      }
      this.userSquares[nextIndex] = null;
      this.ships[shipIndex].isInGrid = false;
      this.ships[shipIndex].rotation = 1;
    },
    rotateShip() {
      if (!this.isShipSelected) {
        return;
      }
      if (this.ships[this.selectedShipIndex].rotation == 1) {
        this.ships[this.selectedShipIndex].rotation = 10;
      } else {
        this.ships[this.selectedShipIndex].rotation = 1;
      }
    },
    selectShip(ship) {
      this.selectedShip = ship;
      this.isShipSelected = true;
      this.selectedShipIndex = ship.shipIndex;
      this.setSelectedShip();
    },
    setSelectedShip() {
      this.ships.forEach((ship) => {
        if (ship.shipIndex == this.selectedShipIndex) {
          ship.isSelected = "selected";
        } else {
          ship.isSelected = null;
        }
      });
    },
    putShip(index) {
      if (!this.isShipSelected) {
        return;
      }
      let isOverTheGrid = this.selectedShip.shipLength + index > 100;
      if (isOverTheGrid) {
        this.message =
          "Can't place the ship over there. It would go past grid.";
        return;
      }
      let isHorizontal = this.selectedShip.rotation == 1;
      let isAtRightEdge = this.selectedShip.shipLength + (index % 10) > 10;
      if (isAtRightEdge && isHorizontal) {
        this.message =
          "Can't place the ship over there. It would go past grid.";
        return;
      }
      let isBottom = this.selectedShip.shipLength * 10 + index > 109;
      if (isBottom && !isHorizontal) {
        this.message =
          "Can't place the ship over there. It would go past grid.";
        return;
      }
      if (!this.userSquares[index]) {
        for (let i = 0; i < this.selectedShip.shipLength; ++i) {
          if (this.userSquares[index + i * this.selectedShip.rotation]) {
            this.message = "The space is already occupied.";
            return;
          }
        }
        for (let i = 0; i < this.selectedShip.shipLength; ++i) {
          let cell = {};
          cell.shipName = "taken " + this.selectedShip.name;
          cell.shipIndex = this.selectedShip.shipIndex;
          cell.start = index;
          if (i + 1 < this.selectedShip.shipLength) {
            cell.next = index + (i + 1) * this.selectedShip.rotation;
          }
          this.userSquares[index + i * this.selectedShip.rotation] = cell;
        }
      } else {
        return;
      }
      this.ships[this.selectedShipIndex].isInGrid = true;
      this.isShipSelected = false;
    },

    setDirectionsForShips() {
      this.ships.forEach((ship) => {
        let xAxis = [];
        let yAxis = [];
        for (let index = 0; index < ship.shipLength; index++) {
          xAxis.push(index);
          yAxis.push(index * this.width);
        }
        ship.directions = [];
        ship.directions.push(xAxis);
        ship.directions.push(yAxis);
      });
    },
  },
};
</script>

<style>
.grid div {
  border: 1px solid hsla(0, 0%, 100%, 0.2);
}

.grid {
  margin: 2vmin;
  display: grid;
  background-color: hsl(200, 100%, 50%);
  grid-template-rows: repeat(10, 4.6vmin);
  grid-template-columns: repeat(10, 4.6vmin);
}

.hit {
  background-color: red;
  height: stretch;
}

.miss {
  background-color: gray;
  height: stretch;
}

.container {
  display: flex;
  justify-content: center;
  width: 100%;
}

.grid-display {
  width: 400px;
  height: 400px;
  background-color: yellow;
  margin: 20px;
}

.destroyer-container {
  width: 80px;
  height: 40px;
  background-color: orange;
  margin: 10px;
  display: flex;
  border-radius: 20px;
}

.taken {
  width: stretch;
  height: stretch;
}

#selected {
  width: 40px;
  height: 40px;
  color: black;
}

.submarine-container {
  width: 120px;
  height: 40px;
  background-color: pink;
  margin: 10px;
  display: flex;
  border-radius: 20px;
}

.cruiser-container {
  width: 120px;
  height: 40px;
  background-color: purple;
  margin: 10px;
  display: flex;
  border-radius: 20px;
}

.battleship-container {
  width: 160px;
  height: 40px;
  background-color: aqua;
  margin: 10px;
  display: flex;
  border-radius: 20px;
}

.carrier-container {
  width: 200px;
  height: 40px;
  background-color: green;
  margin: 10px;
  display: flex;
  border-radius: 20px;
}

.ship div {
  width: 40px;
  height: 40px;
}

.destroyer {
  background-color: orange;
}

.battleship {
  background-color: aqua;
}

.cruiser {
  background-color: purple;
}

.submarine {
  background-color: pink;
}

.carrier {
  background-color: green;
}

.selected {
  border: 3px solid red;
}
</style>
