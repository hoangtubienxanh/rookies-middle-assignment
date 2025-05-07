import type { AccessTokenResponse } from "@/types/api"

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL

// Store auth tokens
let accessToken: string | null = null
let refreshToken: string | null = null
let tokenExpiry: number | null = null

// Check if we're in a browser environment to use localStorage
const isBrowser = typeof window !== "undefined"

// Initialize tokens from localStorage if available
if (isBrowser) {
  accessToken = localStorage.getItem("accessToken")
  refreshToken = localStorage.getItem("refreshToken")
  const expiryStr = localStorage.getItem("tokenExpiry")
  tokenExpiry = expiryStr ? Number.parseInt(expiryStr, 10) : null
}

/**
 * Save authentication tokens to localStorage
 */
export const saveAuthTokens = (tokens: AccessTokenResponse) => {
  accessToken = tokens.accessToken
  refreshToken = tokens.refreshToken
  tokenExpiry = Date.now() + tokens.expiresIn * 1000

  if (isBrowser) {
    localStorage.setItem("accessToken", tokens.accessToken)
    localStorage.setItem("refreshToken", tokens.refreshToken)
    localStorage.setItem("tokenExpiry", tokenExpiry.toString())
  }
}

/**
 * Clear authentication tokens
 */
export const clearAuthTokens = () => {
  accessToken = null
  refreshToken = null
  tokenExpiry = null

  if (isBrowser) {
    localStorage.removeItem("accessToken")
    localStorage.removeItem("refreshToken")
    localStorage.removeItem("tokenExpiry")
  }
}

/**
 * Check if the user is authenticated
 */
export const isAuthenticated = (): boolean => {
  if (!accessToken || !tokenExpiry) return false
  return Date.now() < tokenExpiry
}

/**
 * Get the current access token
 */
export const getAccessToken = (): string | null => {
  return accessToken
}

/**
 * Refresh the access token if needed
 */
const refreshAccessToken = async (): Promise<boolean> => {
  if (!refreshToken) return false

  try {
    const response = await fetch(`${API_BASE_URL}/identity/refresh`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ refreshToken }),
    })

    if (!response.ok) {
      clearAuthTokens()
      return false
    }

    const tokens: AccessTokenResponse = await response.json()
    saveAuthTokens(tokens)
    return true
  } catch (error) {
    console.error("Failed to refresh token:", error)
    clearAuthTokens()
    return false
  }
}

/**
 * Build URL with query parameters
 */
export const buildUrl = (endpoint: string, params?: Record<string, any>): string => {
  if (!params) return `${API_BASE_URL}${endpoint}`

  const url = new URL(`${API_BASE_URL}${endpoint}`)
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null) {
      url.searchParams.append(key, value.toString())
    }
  })

  return url.toString()
}

/**
 * Make an authenticated API request
 */
export const apiRequest = async <T>(
  endpoint: string,
  options: RequestInit = {},
  requiresAuth: boolean = true,
  queryParams?: Record<string, any>
)
: Promise<T> =>
{
  // Check if token refresh is needed
  if (requiresAuth && tokenExpiry && Date.now() > tokenExpiry - 60000) {
    const refreshed = await refreshAccessToken()
    if (!refreshed && requiresAuth) {
      throw new Error("Authentication required")
    }
  }

  // Prepare headers
  const headers = new Headers(options.headers)

  if (!headers.has("Content-Type") && options.body && !(options.body instanceof FormData)) {
    headers.set("Content-Type", "application/json")
  }

  if (requiresAuth && accessToken) {
    headers.set("Authorization", `Bearer ${accessToken}`)
  }

  // Build URL with query parameters if provided
  const url = queryParams ? buildUrl(endpoint, queryParams) : `${API_BASE_URL}${endpoint}`

  // Make the request
  const response = await fetch(url, {
    ...options,
    headers,
  })

  // Handle HTTP errors
  if (!response.ok) {
    let errorMessage = `API request failed with status ${response.status}`
    try {
      const errorData = await response.json()
      errorMessage = errorData.detail || errorData.title || errorMessage
    } catch (e) {
      // If parsing JSON fails, use the default error message
    }
    throw new Error(errorMessage)
  }

  // Parse and return the response
  if (response.status === 204) {
    return {} as T
  }

  return await response.json()
}

/**
 * Login user and get access token
 */
export const login = async (email: string, password: string): Promise<AccessTokenResponse> => {
  const response = await apiRequest<AccessTokenResponse>(
    "/identity/login",
    {
      method: "POST",
      body: JSON.stringify({ email, password }),
    },
    false,
  )

  saveAuthTokens(response)
  return response
}

/**
 * Register a new user
 */
export const register = async (email: string, password: string): Promise<void> => {
  await apiRequest(
    "/identity/register",
    {
      method: "POST",
      body: JSON.stringify({ email, password }),
    },
    false,
  )
}

/**
 * Logout user
 */
export const logout = (): void => {
  clearAuthTokens()
}
