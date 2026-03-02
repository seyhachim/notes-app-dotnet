<template>
  <div class="min-h-screen bg-gray-50">

    <!-- Navbar -->
    <nav class="bg-white border-b border-gray-200 px-6 py-4">
      <div class="max-w-4xl mx-auto flex items-center justify-between">
        <h1 class="text-xl font-bold text-gray-900">Note Application - Techbodia</h1>
        <div class="flex items-center gap-4">
          <span class="text-sm text-gray-500">{{ authStore.user?.username }}</span>
          <button @click="handleLogout" class="text-sm text-red-500 hover:text-red-700 font-medium transition">
            Logout
          </button>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="max-w-4xl mx-auto px-6 py-8">

      <!-- Top Bar: Search + Create -->
      <div class="flex flex-col sm:flex-row gap-3 mb-6">

        <!-- Search -->
        <input v-model="searchQuery" type="text" placeholder="Search notes..." class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent" />

        <!-- Sort -->
        <select v-model="sortBy" class="px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                 focus:outline-none focus:ring-2 focus:ring-emerald-500
                 bg-white cursor-pointer">
          <option value="createdAt">Newest First</option>
          <option value="updatedAt">Recently Updated</option>
          <option value="title">Title A-Z</option>
        </select>

        <!-- Create Button -->
        <button @click="openCreateModal" class="px-5 py-2.5 bg-emerald-600 hover:bg-emerald-700 text-white
                 text-sm font-medium rounded-lg transition whitespace-nowrap">
          + New Note
        </button>

      </div>

      <!-- Loading -->
      <div v-if="notesStore.isLoading" class="flex justify-center py-16">
        <div class="w-8 h-8 border-4 border-emerald-200 border-t-emerald-600 rounded-full animate-spin" />
      </div>

      <!-- Error -->
      <div v-else-if="notesStore.error" class="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
        {{ notesStore.error }}
      </div>

      <!-- Empty State -->
      <div v-else-if="notesStore.notes.length === 0" class="text-center py-16">
        <template v-if="searchQuery">
          <p class="text-4xl mb-4">🔍</p>
          <p class="text-gray-500 text-lg">No notes match "{{ searchQuery }}"</p>
          <p class="text-gray-400 text-sm mt-1">Try a different search term</p>
        </template>
        <template v-else>
          <p class="text-4xl mb-4">📭</p>
          <p class="text-gray-500 text-lg">No notes yet</p>
          <p class="text-gray-400 text-sm mt-1">Click "+ New Note" to create your first one</p>
        </template>
      </div>

      <!-- Notes Grid -->
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <div v-for="note in notesStore.notes" :key="note.id" class="bg-white border border-gray-200 rounded-xl p-5 hover:shadow-md
                 transition cursor-pointer group" @click="openEditModal(note)">

          <!-- Note Title -->
          <h3 class="font-semibold text-gray-900 mb-2 truncate">
            {{ note.title }}
          </h3>

          <!-- Note Content Preview -->
          <p class="text-gray-500 text-sm line-clamp-3 mb-4">
            {{ note.content }}
          </p>

          <!-- Footer: date + delete -->
          <div class="flex items-center justify-between">
            <span class="text-xs text-gray-400">
              {{ formatDate(note.updatedAt) }}
            </span>
            <button @click.stop="handleDelete(note.id)" class="text-xs text-red-400 hover:text-red-600 opacity-0 group-hover:opacity-100
                     transition font-medium">
              Delete
            </button>
          </div>

        </div>
      </div>

      <!-- Pagination -->
      <div v-if="notesStore.pagination.totalPages > 1" class="flex items-center justify-center gap-2 mt-8">
        <button @click="changePage(currentPage - 1)" :disabled="currentPage === 1" class="px-4 py-2 text-sm border border-gray-300 rounded-lg
                 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition">
          Previous
        </button>

        <span class="text-sm text-gray-600 px-2">
          Page {{ currentPage }} of {{ notesStore.pagination.totalPages }}
        </span>

        <button @click="changePage(currentPage + 1)" :disabled="currentPage === notesStore.pagination.totalPages" class="px-4 py-2 text-sm border border-gray-300 rounded-lg
                 hover:bg-gray-50 disabled:opacity-40 disabled:cursor-not-allowed transition">
          Next
        </button>
      </div>

    </main>

    <!-- ── Delete Confirm Modal ────────────────────────────────────────────── -->
    <div v-if="showDeleteConfirm" class="fixed inset-0 bg-black/40 flex items-center justify-center z-50 px-4">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6">

        <div class="text-center">
          <p class="text-4xl mb-4">🗑️</p>
          <h3 class="text-lg font-semibold text-gray-900 mb-2">Delete Note</h3>
          <p class="text-gray-500 text-sm mb-6">
            Are you sure you want to delete this note? This cannot be undone.
          </p>
        </div>

        <div class="flex gap-3">
          <button @click="cancelDelete" class="flex-1 py-2.5 border border-gray-300 text-gray-700 text-sm
                   font-medium rounded-lg hover:bg-gray-50 transition">
            Cancel
          </button>
          <button @click="confirmDelete" :disabled="isDeleting" class="flex-1 py-2.5 bg-red-600 hover:bg-red-700 disabled:bg-red-400
                   text-white text-sm font-medium rounded-lg transition
                   disabled:cursor-not-allowed">
            <span v-if="isDeleting">Deleting...</span>
            <span v-else>Delete</span>
          </button>
        </div>

      </div>
    </div>

    <!-- ── Create / Edit Modal ────────────────────────────────────────────── -->
    <div v-if="showModal" class="fixed inset-0 bg-black/40 flex items-center justify-center z-50 px-4"
      @click.self="closeModal">
      <div class="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6">

        <!-- Modal Header -->
        <div class="flex items-center justify-between mb-5">
          <h2 class="text-lg font-semibold text-gray-900">
            {{ isEditing ? 'Edit Note' : 'New Note' }}
          </h2>
          <button @click="closeModal" class="text-gray-400 hover:text-gray-600 text-xl leading-none">
            ✕
          </button>
        </div>

        <!-- Modal Error -->
        <div v-if="modalError" class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
          {{ modalError }}
        </div>

        <!-- Modal Form -->
        <form @submit.prevent="handleSave" class="space-y-4">

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
            <input v-model="modalForm.title" type="text" placeholder="Note title" required maxlength="200" class="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                     focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent" />
          </div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Content</label>
            <textarea v-model="modalForm.content" placeholder="Write your note here..." rows="6" class="w-full px-4 py-2.5 border border-gray-300 rounded-lg text-sm
                     focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent
                     resize-none" />
          </div>

          <!-- Modal Actions -->
          <div class="flex gap-3 pt-2">
            <button type="button" @click="closeModal" class="flex-1 py-2.5 border border-gray-300 text-gray-700 text-sm
                     font-medium rounded-lg hover:bg-gray-50 transition">
              Cancel
            </button>
            <button type="submit" :disabled="isSaving" class="flex-1 py-2.5 bg-emerald-600 hover:bg-emerald-700 disabled:bg-emerald-400
                     text-white text-sm font-medium rounded-lg transition
                     disabled:cursor-not-allowed">
              <span v-if="isSaving">Saving...</span>
              <span v-else>{{ isEditing ? 'Save Changes' : 'Create Note' }}</span>
            </button>
          </div>

        </form>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import { useNotesStore } from '@/stores/notesStore'
import type { Note } from '@/types/note.types'

const router = useRouter()
const authStore = useAuthStore()
const notesStore = useNotesStore()

// ── Search + Sort + Pagination state ─────────────────────────────────────────
const searchQuery = ref('')
const sortBy = ref('createdAt')
const currentPage = ref(1)

// ── Create/Edit Modal state ───────────────────────────────────────────────────
const showModal = ref(false)
const isEditing = ref(false)
const editingId = ref<number | null>(null)
const modalForm = ref({ title: '', content: '' })
const modalError = ref<string | null>(null)
const isSaving = ref(false)

// ── Delete Confirm Modal state ────────────────────────────────────────────────
const showDeleteConfirm = ref(false)
const deletingId = ref<number | null>(null)
const isDeleting = ref(false)

// ── Load notes on mount ───────────────────────────────────────────────────────
onMounted(() => loadNotes())

function loadNotes() {
  notesStore.fetchNotes({
    search: searchQuery.value || undefined,
    sortBy: sortBy.value,
    sortDir: sortBy.value === 'title' ? 'asc' : 'desc',
    page: currentPage.value,
    pageSize: 9
  })
}

// ── Watch for filter/sort/search changes ─────────────────────────────────────
let searchTimeout: ReturnType<typeof setTimeout>
watch(searchQuery, () => {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    currentPage.value = 1
    loadNotes()
  }, 400)
})

watch(sortBy, () => {
  currentPage.value = 1
  loadNotes()
})

// ── Pagination ────────────────────────────────────────────────────────────────
function changePage(page: number) {
  currentPage.value = page
  loadNotes()
}

// ── Create/Edit Modal helpers ─────────────────────────────────────────────────
function openCreateModal() {
  isEditing.value = false
  editingId.value = null
  modalForm.value = { title: '', content: '' }
  modalError.value = null
  showModal.value = true
}

function openEditModal(note: Note) {
  isEditing.value = true
  editingId.value = note.id
  modalForm.value = { title: note.title, content: note.content }
  modalError.value = null
  showModal.value = true
}

function closeModal() {
  showModal.value = false
}

// ── CRUD actions ──────────────────────────────────────────────────────────────
async function handleSave() {
  modalError.value = null
  isSaving.value = true

  try {
    if (isEditing.value && editingId.value !== null) {
      await notesStore.updateNote(editingId.value, modalForm.value)
    } else {
      await notesStore.createNote(modalForm.value)
    }
    closeModal()
  } catch (err: any) {
    modalError.value = err.response?.data?.message || 'Failed to save note.'
  } finally {
    isSaving.value = false
  }
}

// ── Delete actions ────────────────────────────────────────────────────────────
function handleDelete(id: number) {
  // Store which note the user wants to delete and show the confirm modal
  deletingId.value = id
  showDeleteConfirm.value = true
}

function cancelDelete() {
  // User changed their mind — close modal and clear state
  showDeleteConfirm.value = false
  deletingId.value = null
}

async function confirmDelete() {
  if (deletingId.value === null) return
  isDeleting.value = true

  try {
    await notesStore.deleteNote(deletingId.value)
    showDeleteConfirm.value = false
    deletingId.value = null
  } catch {
    // Close the modal even on error — error handling can be improved later
    showDeleteConfirm.value = false
  } finally {
    isDeleting.value = false
  }
}

function handleLogout() {
  authStore.logout()
  router.push({ name: 'login' })
}

// ── Helpers ───────────────────────────────────────────────────────────────────
function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}
</script>
