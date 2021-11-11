import Vue from 'vue'
import VueRouter from 'vue-router'
import Home from '../views/Home.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  {
    path: '/tierlist/:setcode',
    name: 'TierList',
    component: () => import(/* webpackChunkName: "tierlist" */ '../views/TierList.vue')
  },
  {
    path: '/topcommons/:setcode',
    name: 'TopCommons',
    component: () => import(/* webpackChunkName: "topcommons" */ '../views/TopCommons.vue')
  },
  {
    path: '/colorrankings/:setcode',
    name: 'ColorRankings',
    component: () => import(/* webpackChunkName: "colorrankings" */ '../views/ColorRankings.vue')
  }
]

const router = new VueRouter({
  base: process.env.BASE_URL,
  routes
})

export default router
