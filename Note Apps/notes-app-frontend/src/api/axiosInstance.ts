import axios from 'axios'

// Single Axios instance used by all API calls.
// Base URL points to our ASP.NET backend.
// Change the port if yours is different.
const api = axios.create({
  baseURL: 'http://localhost:5104/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

// ── Request Interceptor ────────────────────────────────────────────────────────
// Runs before every request.
// Reads the JWT token from localStorage and attaches it as a Bearer token.
// This means we never manually add Authorization headers in individual API calls.
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// ── Response Interceptor ───────────────────────────────────────────────────────
// Runs after every response.
// If the API returns 401 (token expired or invalid),
// clear the stored token and redirect to login.
api.interceptors.response.use(
  (response) => response, // pass successful responses straight through
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

export default api
