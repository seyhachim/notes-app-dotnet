import api from './axiosInstance'
import type { NoteRequest, NoteQueryParams, Note, PagedResponse } from '@/types/note.types'

// All notes-related API calls in one place.

export const notesApi = {
  getAll: (params?: NoteQueryParams) => api.get<PagedResponse<Note>>('/notes', { params }),

  getById: (id: number) => api.get<Note>(`/notes/${id}`),

  create: (data: NoteRequest) => api.post<Note>('/notes', data),

  update: (id: number, data: NoteRequest) => api.put<Note>(`/notes/${id}`, data),

  delete: (id: number) => api.delete(`/notes/${id}`),
}
