import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeView,
  },
  {
    path: "/about",
    name: "about",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/AboutView.vue"),
  },
  {
    path: "/private",
    name: "private",
    component: () =>
      import(/* webpackChunkName: "private" */ "../views/PrivateView.vue"),
  },
  {
    path: "/callback",
    name: "callback",
    component: () =>
      import(
        /* webpackChunkName: "callback" */ "../views/Account/CallbackView.vue"
      ),
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
