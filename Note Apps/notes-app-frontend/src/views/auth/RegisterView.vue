<template>
  <div class="min-h-screen bg-gray-50 flex items-center justify-center px-4">
    <div class="w-full max-w-md">

      <!-- Header -->
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900">Notes App</h1>
        <p class="text-gray-500 mt-2">Create your account</p>
      </div>

      <!-- Card -->
      <div class="bg-white rounded-2xl shadow-sm border border-gray-200 p-8">

        <!-- Success message after registration -->
        <div v-if="success" class="mb-6 p-4 bg-green-50 border border-green-200 rounded-lg text-green-700 text-sm">
          Account created! Redirecting to login...
        </div>

        <!-- Error Alert -->
        <div v-if="error" class="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
          {{ error }}
        </div>

        <!-- Form -->
        <form @submit.prevent="handleRegister" class="space-y-5">

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">
              Username
            </label>
            <input v-model="form.username" type="text" placeholder="johndoe" required class="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                     focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
                     transition" />
          </div>

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
            <input v-model="form.password" type="password" placeholder="••••••••" required minlength="6" class="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                     focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent
                     transition" />
            <p class="text-xs text-gray-400 mt-1">Minimum 6 characters</p>
          </div>

          <!-- Submit -->
          <button type="submit" :disabled="isLoading" class="w-full py-2.5 px-4 bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400
                   text-white font-medium rounded-lg text-sm transition cursor-pointer
                   disabled:cursor-not-allowed">
            <span v-if="isLoading">Creating account...</span>
            <span v-else>Create Account</span>
          </button>

        </form>

        <!-- Footer -->
        <p class="text-center text-sm text-gray-500 mt-6">
          Already have an account?
          <RouterLink to="/login" class="text-blue-600 hover:underline font-medium">
            Sign in
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

const form = ref({
  username: '',
  email: '',
  password: ''
})

const isLoading = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

async function handleRegister() {
  error.value = null
  isLoading.value = true

  try {
    await authStore.register(form.value)

    // Show success message briefly then redirect to login
    success.value = true
    setTimeout(() => router.push({ name: 'login' }), 1500)

  } catch (err: any) {
    // Backend returns "Email is already in use." or "Username is already taken."
    error.value = err.response?.data?.message || 'Registration failed. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>
