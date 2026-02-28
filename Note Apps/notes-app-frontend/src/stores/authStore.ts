import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api/authApi'
import type { LoginRequest, RegisterRequest, AuthUser } from '@/types/auth.types'

export const useAuthStore = defineStore('auth', () => {
  // ── State ────────────────────────────────────────────────────────────────────
  // Initialize from localStorage so the user stays logged in on page refresh
  const token = ref<string | null>(localStorage.getItem('token'))
  const user = ref<AuthUser | null>(JSON.parse(localStorage.getItem('user') || 'null'))

  // ── Getters ──────────────────────────────────────────────────────────────────
  // isAuthenticated is used by the route guard to protect pages
  const isAuthenticated = computed(() => !!token.value)

  // ── Actions ──────────────────────────────────────────────────────────────────
  async function login(credentials: LoginRequest) {
    const response = await authApi.login(credentials)
    const { token: newToken, username, email } = response.data

    // Persist to localStorage so login survives page refresh
    token.value = newToken
    user.value = { username, email }
    localStorage.setItem('token', newToken)
    localStorage.setItem('user', JSON.stringify({ username, email }))
  }

  async function register(data: RegisterRequest) {
    // Register doesn't log the user in automatically —
    // we redirect to login after registration
    await authApi.register(data)
  }

  function logout() {
    // Clear everything — store state and localStorage
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  return { token, user, isAuthenticated, login, register, logout }
})
