import api from './axiosInstance'
import type { LoginRequest, RegisterRequest, AuthResponse } from '@/types/auth.types'

// All auth-related API calls in one place.
// Components and stores never import axios directly — always go through these.

export const authApi = {
  register: (data: RegisterRequest) => api.post<AuthResponse>('/auth/register', data),

  login: (data: LoginRequest) => api.post<AuthResponse>('/auth/login', data),
}
