// Authentication types
export interface LoginRequest {
  email: string
  password: string
  twoFactorCode?: string
  twoFactorRecoveryCode?: string
}

export interface RegisterRequest {
  email: string
  password: string
}

export interface AccessTokenResponse {
  tokenType?: string
  accessToken: string
  expiresIn: number
  refreshToken: string
}

export interface RefreshRequest {
  refreshToken: string
}

// Book types
export interface BookItem {
  id: string
  title: string
  author: string
  categoryId?: string,
  category?: string,
  quantity?: number
}

export interface BookCreateOptions {
  title: string
  author: string
  inputQuantity?: number
  categoryId?: string
}

// Category types
export interface Category {
  categoryId: string
  name: string
  slug?: string
}

// Loan types
export interface LoanApplicationItem {
  id: string
  status: string
  created: string
  items: BookItem[]
}

// Pagination types
export interface PaginatedItems<T> {
  pageIndex: number
  pageSize: number
  count: number
  data: T[]
}

export type PaginatedBooks = PaginatedItems<BookItem>
export type PaginatedCategories = PaginatedItems<Category>
export type PaginatedLoanApplications = PaginatedItems<LoanApplicationItem>
