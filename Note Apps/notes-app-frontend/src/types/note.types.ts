// Shape of a note returned from the API
export interface Note {
  id: number
  title: string
  content: string
  createdAt: string
  updatedAt: string
}

// What we send when creating or updating a note
export interface NoteRequest {
  title: string
  content: string
}

// Query params for the notes list endpoint
export interface NoteQueryParams {
  search?: string
  sortBy?: string
  sortDir?: string
  page?: number
  pageSize?: number
}

// Paged response wrapper from the API
export interface PagedResponse<T> {
  data: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}
