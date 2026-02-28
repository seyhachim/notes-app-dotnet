import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/notes', // root always redirects to notes
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/auth/LoginView.vue'),
      meta: { requiresGuest: true }, // logged-in users can't visit login page
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/auth/RegisterView.vue'),
      meta: { requiresGuest: true },
    },
    {
      path: '/notes',
      name: 'notes',
      component: () => import('@/views/notes/NotesView.vue'),
      meta: { requiresAuth: true }, // must be logged in
    },
  ],
})

// ── Route Guard ───────────────────────────────────────────────────────────────
// Runs before every navigation.
// requiresAuth  → redirect to /login if not authenticated
// requiresGuest → redirect to /notes if already authenticated
router.beforeEach((to) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'login' }
  }

  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    return { name: 'notes' }
  }
})

export default router
