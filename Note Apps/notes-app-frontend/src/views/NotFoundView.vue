<template>
    <div class="min-h-screen bg-gray-50 flex items-center justify-center px-4">
        <div class="text-center">

            <p class="text-8xl font-bold text-gray-200 mb-4">404</p>

            <h1 class="text-2xl font-bold text-gray-900 mb-2">
                Page not found
            </h1>

            <p class="text-gray-500 mb-8">
                The page you're looking for doesn't exist.
            </p>

            <RouterLink :to="isAuthenticated ? '/notes' : '/login'" class="px-6 py-2.5 bg-emerald-600 hover:bg-emerald-700 text-white
               text-sm font-medium rounded-lg transition">
                {{ isAuthenticated ? 'Back to Notes' : 'Back to Login' }}
            </RouterLink>

        </div>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuthStore } from '@/stores/authStore'

const authStore = useAuthStore()

// Send logged-in users back to notes, guests back to login
const isAuthenticated = computed(() => authStore.isAuthenticated)
</script>
