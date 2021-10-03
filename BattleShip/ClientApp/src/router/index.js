import { createWebHistory, createRouter } from "vue-router";
import Home from "@/components/Home.vue";
import Login from "@/components/Auth/Login.vue";
import Register from "@/components/Auth/Register.vue";
import Battleship from "@/components/Battleship.vue";

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
  },
  {
    path: "/Login",
    name: "Login",
    component: Login,
  },
  {
    path: "/Register",
    name: "Register",
    component: Register,
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
