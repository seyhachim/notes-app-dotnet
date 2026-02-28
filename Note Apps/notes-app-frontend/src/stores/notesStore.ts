import { defineStore } from 'pinia'
import { ref } from 'vue'
import { notesApi } from '@/api/notesApi'
import type { Note, NoteRequest, NoteQueryParams, PagedResponse } from '@/types/note.types'

export const useNotesStore = defineStore('notes', () => {
  // ── State ────────────────────────────────────────────────────────────────────
  const notes = ref<Note[]>([])
  const pagination = ref<Omit<PagedResponse<Note>, 'data'>>({
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
  })
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // ── Actions ──────────────────────────────────────────────────────────────────
  async function fetchNotes(params?: NoteQueryParams) {
    isLoading.value = true
    error.value = null
    try {
      const response = await notesApi.getAll(params)
      notes.value = response.data.data
      pagination.value = {
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
      }
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to load notes.'
    } finally {
      isLoading.value = false
    }
  }

  async function createNote(data: NoteRequest) {
    const response = await notesApi.create(data)
    // Add to the top of the list without refetching
    notes.value.unshift(response.data)
    pagination.value.totalCount++
  }

  async function updateNote(id: number, data: NoteRequest) {
    const response = await notesApi.update(id, data)
    // Replace the old note in the list with the updated one
    const index = notes.value.findIndex((n) => n.id === id)
    if (index !== -1) notes.value[index] = response.data
  }

  async function deleteNote(id: number) {
    await notesApi.delete(id)
    // Remove from list without refetching
    notes.value = notes.value.filter((n) => n.id !== id)
    pagination.value.totalCount--
  }

  return {
    notes,
    pagination,
    isLoading,
    error,
    fetchNotes,
    createNote,
    updateNote,
    deleteNote,
  }
})
