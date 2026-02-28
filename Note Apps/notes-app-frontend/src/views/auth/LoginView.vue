<template>
  <div class="min-h-screen bg-gray-50 flex items-center justify-center px-4">
    <div class="w-full max-w-md">

      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900">Notes App</h1>
        <p class="text-gray-500 mt-2">Sign in to your account</p>
      </div>

      <!-- Card -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">

        <!-- Error Alert -->
        <div v-if="error" class="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
          {{ error }}
        </div>

        <!-- Form -->
        <form @submit.prevent="handleLogin" class="space-y-5">

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input v-model="form.email" type="email" placeholder="you@example.com" required class="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                     focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
                     transition" />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Password
            </label>
            <input v-model="form.password" type="password" placeholder="••••••••" required class="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                     focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
                     transition" />
          </div>

          <!-- Submit -->
          <button type="submit" :disabled="isLoading" class="w-full py-2.5 px-4 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400
                   text-white font-medium rounded-lg text-sm transition cursor-pointer
                   disabled:cursor-not-allowed">
            <span v-if="isLoading">Signing in...</span>
            <span v-else>Sign In</span>
          </button>

        </form>

        <!-- Footer -->
        <p class="text-center text-sm text-gray-500 mt-6">
          Don't have an account?
          <RouterLink to="/register" class="text-blue-600 hover:underline font-medium">
            Register
          </RouterLink>
        </p>

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

// Form state — two-way bound to the inputs via v-model
const form = ref({
  email: '',
  password: ''
})

// UI state
const isLoading = ref(false)
const error = ref<string | null>(null)

async function handleLogin() {
  // Clear previous error before each attempt
  error.value = null
  isLoading.value = true

  try {
    // authStore.login handles the API call, token storage, and store update
    await authStore.login(form.value)

    // On success, navigate to notes
    router.push({ name: 'notes' })

  } catch (err: any) {
    // Show the error message returned by the backend middleware
    // e.g. "Invalid email or password."
    error.value = err.response?.data?.message || 'Login failed. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>
