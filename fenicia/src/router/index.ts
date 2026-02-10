import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () => import("../views/DashboardView.vue")
    },
    {
      path: '/order',
      name: 'order',
      component: () => import("../views/OrderView.vue")
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: () => import("../views/ForgotPassword.vue")
    },
    {
      path: '/recover-password',
      name: 'recover-password',
      component: () => import("../views/RecoverPassword.vue")
    },
    {
      path: '/basic/categories',
      name: 'basic-categories',
      component: () => import("../views/basic/Categories.vue")
    }
  ],
})

export default router
