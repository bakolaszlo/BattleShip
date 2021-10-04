import { createWebHistory, createRouter } from "vue-router";
import Home from "@/components/Home.vue";
import Battleship from "@/components/Battleship.vue";

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
  },
  {
    path: "/battleship/:lobby_id",
    name: "Battleship",
    component: Battleship,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
