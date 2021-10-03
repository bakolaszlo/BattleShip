<template>
  <div v-if="!modalLobbyId">
    <WaitModal />
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

  <div class="hidden-info">
    <button class="start" v-if="letShipsToSelectFrom.length == 0">
      I'm Ready
    </button>
    <button class="rotate" @click="rotateShip">Rotate</button>
    <h3 class="info">{{ message }}</h3>
  </div>
  <div class="grid-display">
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
import * as signalR from "@aspnet/signalr";
export default {
  components: { WaitModal },
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
      this.modalLobbyId = modalLobbyId;
      this.guestName = joinerNickname;
      console.log(this.modalLobbyId);
    });
    this.isGuest();
  },
  methods: {
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
          }
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    attackEnemy(index) {
      if (this.hitOn(index)) {
        this.enemySquares[index] = "hit";
      } else {
        this.enemySquares[index] = "miss";
      }
    },
    hitOn(index) {
      console.log(index);
      if (Math.random() > 0.5) {
        return true;
      }
      return false;
    },
    removeFromGrid(clickedIndex) {
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
      let isBottom = this.selectedShip.shipLength * 10 + index > 110;
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
          this.userSquares[index + i * this.selectedShip.rotation] = {};
          this.userSquares[index + i * this.selectedShip.rotation].shipName =
            "taken " + this.selectedShip.name;
          this.userSquares[
            index + i * this.selectedShip.rotation
          ].shipIndex = this.selectedShip.shipIndex;
          this.userSquares[
            index + i * this.selectedShip.rotation
          ].start = index;
          if (i + 1 < this.selectedShip.shipLength) {
            this.userSquares[index + i * this.selectedShip.rotation].next =
              index + (i + 1) * this.selectedShip.rotation;
          }
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
