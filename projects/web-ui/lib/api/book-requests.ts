import { apiRequest } from "./api-client"
import type { LoanApplicationItem, PaginatedLoanApplications } from "@/types/api"

export type BookRequest = LoanApplicationItem

/**
 * Get all requests for a user
 */
export async function getUserRequests(userId: string, page = 1, pageSize = 20): Promise<BookRequest[]> {
  const response = await apiRequest<PaginatedLoanApplications>("/loan", { method: "GET" }, true, {
    pageIndex: page - 1,
    pageSize,
  })

  return response.data
}

/**
 * Get all requests (for admin)
 */
export async function getAllRequests(page = 1, pageSize = 20): Promise<BookRequest[]> {
  const response = await apiRequest<PaginatedLoanApplications>("/loan", { method: "GET" }, true, {
    pageIndex: page - 1, 
    pageSize,
  })

  return response.data
}

/**
 * Get a request by ID
 */
export async function getRequestById(id: string): Promise<BookRequest> {
  return await apiRequest<BookRequest>(`/loan/${id}`)
}

/**
 * Submit a new book request
 */
export async function submitBookRequest(userId: string, bookIds: string[]): Promise<BookRequest> {
  // Validate book count
  if (bookIds.length === 0) {
    throw new Error("You must select at least one book to borrow.")
  }

  if (bookIds.length > 5) {
    throw new Error("You can only borrow up to 5 books in a single request.")
  }

  return await apiRequest<BookRequest>("/loan", {
    method: "POST",
    body: JSON.stringify(bookIds),
  })
}

/**
 * Approve a book request
 */
export async function approveRequest(requestId: string): Promise<BookRequest> {
  return await apiRequest<BookRequest>(`/loan/${requestId}`, { method: "PUT" }, true, {
    Status: "approved",
  })
}

/**
 * Reject a book request
 */
export async function rejectRequest(requestId: string): Promise<BookRequest> {
  return await apiRequest<BookRequest>(`/loan/${requestId}`, { method: "PUT" }, true, {
    Status: "rejected",
  })
}
