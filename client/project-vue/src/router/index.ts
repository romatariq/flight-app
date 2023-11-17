import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '@/views/HomeView.vue'
import LoginView from '@/views/LoginView.vue'
import RegisterView from '@/views/RegisterView.vue'
import ReviewsView from '@/views/ReviewsView.vue'
import ReviewView from '@/views/ReviewView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.VITE_BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView
    },
    {
      path: '/reviews',
      name: 'reviews',
      component: ReviewsView
    },
    {
      path: '/reviews/edit/:id?',
      name: 'review',
      component: ReviewView
    }
  ]
})

export default router
