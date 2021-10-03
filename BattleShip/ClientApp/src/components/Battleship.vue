<template>
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
      <div class="grid grid-enemy" ref="enemygrid"></div>
    </div>
  </div>

  <div class="hidden-info">
    <button class="start">Start game</button>
    <button class="rotate" @click="rotateShip">Rotate</button>
    <h3 class="whose-go">Your go</h3>
    <h3 class="info"></h3>
  </div>
  <div class="grid-display">
    Your ships:
    <div
      v-for="ship in letShipsToSelectFrom"
      :key="ship"
      :class="'ship ' + ship.name + '-container'"
      draggable="true"
      :ondragstart="dragStartShip"
      :onmousedown="helperFunction"
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
export default {
  data() {
    return {
      ships: [
        {
          name: "destroyer",
          shipLength: 2,
          rotation: 1,
          isInGrid: false,
          shipIndex: 0,
        },
        {
          name: "submarine",
          shipLength: 3,
          rotation: 1,
          isInGrid: false,
          shipIndex: 1,
        },
        {
          name: "cruiser",
          shipLength: 3,
          rotation: 1,
          isInGrid: false,
          shipIndex: 2,
        },
        {
          name: "battleship",
          shipLength: 4,
          rotation: 1,
          isInGrid: false,
          shipIndex: 3,
        },
        {
          name: "carrier",
          shipLength: 5,
          rotation: 1,
          isInGrid: false,
          shipIndex: 4,
        },
      ],
      width: 10,
      enemySquares: [],
      userSquares: [],
      isShipSelected: false,
      selectedShip: null,
      selectedShipIndex: null,
    };
  },
  computed: {
    letShipsToSelectFrom() {
      return this.ships.filter((x) => !x.isInGrid);
    },
  },
  mounted() {
    this.setDirectionsForShips();
    this.createBoard(this.$refs.enemygrid, this.enemySquares);
    this.ships.forEach((ship) => {
      this.generateRandomShipPosition(ship);
    });
  },
  methods: {
    removeFromGrid(clickedIndex) {
      console.log("clicked index", clickedIndex);
      let startPoint = this.userSquares[clickedIndex].start;
      console.log(startPoint, "startpoint");
      console.log(this.userSquares[clickedIndex].next, "next");
      let index = startPoint;
      let shipIndex = this.userSquares[startPoint].shipIndex;
      try {
        while (this.userSquares[index].next) {
          let nextIndex = this.userSquares[index].next;
          this.userSquares[index] = null;
          index = nextIndex;
        }
      } catch {
        this.ships[shipIndex].isInGrid = false;
        this.ships[shipIndex].rotation = 1;
      }
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
    },
    putShip(index) {
      if (!this.isShipSelected) {
        return;
      }
      let isOverTheGrid = this.selectedShip.shipLength + index > 100;
      if (isOverTheGrid) {
        console.log(isOverTheGrid, "Is over grid");
        return;
      }
      let isHorizontal = this.selectedShip.rotation == 1;
      let isAtRightEdge = this.selectedShip.shipLength + (index % 10) > 10;
      if (isAtRightEdge && isHorizontal) {
        console.log(isAtRightEdge, "Is at right edge");
        return;
      }
      let isBottom = this.selectedShip.shipLength * 10 + index > 110;
      if (isBottom && !isHorizontal) {
        console.log("Ship long", this.selectedShip.shipLength * 10 + index);
        return;
      }
      if (!this.userSquares[index]) {
        for (let i = 0; i < this.selectedShip.shipLength; ++i) {
          if (this.userSquares[index + i * this.selectedShip.rotation]) {
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
          if (i < this.selectedShip.shipLength) {
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
    helperFunction(e) {
      this.selectedShipNameWithIndex = e.target.id;
      console.log(this.selectedShipNameWithIndex);
    },
    dragStartShip(e) {
      this.draggedShip = e.target;
      this.draggedShipLength = this.draggedShip.childNodes.length;
      console.log(this.draggedShip);
    },
    dragStart(e) {
      console.log(e.target);
    },
    dragOver(e) {
      e.preventDefault();
    },
    dragEnter(e) {
      e.preventDefault();
    },
    dragLeave(e) {
      e.preventDefault();
    },
    dragDrop() {
      let shipNameWithLastId = this.draggedShip.lastElementChild.id;
      let shipClass = shipNameWithLastId.slice(0, -2);
      console.log(shipClass);
    },
    dragEnd(e) {
      console.log(e.target);
    },
    createBoard(grid, squares) {
      for (let i = 0; i < this.width * this.width; ++i) {
        const square = document.createElement("div");
        square.dataset.id = i;
        if (grid === this.$refs.usergrid) {
          square.ondragstart = this.dragStart;
          square.ondragover = this.dragOver;
          square.ondragenter = this.dragEnter;
          square.ondragleave = this.dragLeave;
          square.ondrop = this.dragDrop;
          square.ondragend = this.dragEnd;
        }
        grid.appendChild(square);
        squares.push(square);
      }
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
    generateRandomShipPosition(ship) {
      let randomDirection = Math.floor(Math.random() * 2);
      let current = ship.directions[randomDirection];

      let direction;
      if (randomDirection === 0) {
        direction = 1;
      } else {
        direction = 10;
      }
      let randomStart = Math.abs(
        Math.floor(Math.random() * 100 - ship.directions[0].length * direction)
      );

      const isTaken = current.some((index) =>
        this.enemySquares[randomStart + index].classList.contains("taken")
      );
      const isAtRightEdge = current.some(
        (index) => (randomStart + index) % this.width === this.width - 1
      );
      const isAtLeftEdge = current.some(
        (index) => (randomStart + index) % this.width === 0
      );

      if (!isTaken && !isAtRightEdge && !isAtLeftEdge) {
        current.forEach((index) => {
          this.enemySquares[randomStart + index].classList.add(
            "taken",
            ship.name
          );
        });
      } else {
        this.generateRandomShipPosition(ship);
      }
    },
  },
};
</script>

<style>
.grid-user {
  width: 400px;
  height: 400px;
  display: flex;
  flex-wrap: wrap;
  background-color: blue;
  margin: 20px;
}

.grid div {
  width: 40px;
  height: 40px;
}

.grid-enemy {
  width: 400px;
  height: 400px;
  display: flex;
  flex-wrap: wrap;
  background-color: lightgreen;
  margin: 20px;
}

.container {
  display: flex;
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
}

.submarine-container {
  width: 120px;
  height: 40px;
  background-color: pink;
  margin: 10px;
  display: flex;
}

.cruiser-container {
  width: 120px;
  height: 40px;
  background-color: purple;
  margin: 10px;
  display: flex;
}

.battleship-container {
  width: 160px;
  height: 40px;
  background-color: aqua;
  margin: 10px;
  display: flex;
}

.carrier-container {
  width: 200px;
  height: 40px;
  background-color: green;
  margin: 10px;
  display: flex;
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
</style>
