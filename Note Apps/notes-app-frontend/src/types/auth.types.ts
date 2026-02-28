// Shape of the login/register request bodies
export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
}

// Shape of what the API returns after login
export interface AuthResponse {
  token: string
  username: string
  email: string
}

// What we store in Pinia after login
export interface AuthUser {
  username: string
  email: string
}
