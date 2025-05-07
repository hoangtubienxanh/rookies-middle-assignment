import { apiRequest } from "./api-client"
import type { BookItem, BookCreateOptions, PaginatedBooks } from "@/types/api"

export type BookData = BookItem & {
  available?: number
  category?: string
}

/**
 * Fetch books with pagination
 */
export async function fetchBooks(
  page = 1,
  pageSize = 20,
  categoryId?: string,
): Promise<{ books: BookData[]; totalRecords: number }> {
  try {
    const response = await apiRequest<PaginatedBooks>("/books", { method: "GET" }, true, {
      pageIndex: page - 1,
      pageSize,
    })

    const books = response.data.map((book) => ({
      ...book,
      available: book.quantity,
      category: book.categoryId ? book.category : "Uncategorized",
    }))

    return {
      books,
      totalRecords: response.count,
    }
  } catch (error) {
    console.error("Error fetching books:", error)

    return {
      books: [],
      totalRecords: 0,
    }
  }
}

/**
 * Get a book by ID
 */
export async function getBookById(id: string): Promise<BookData> {
  const book = await apiRequest<BookItem>(`/books/${id}`)

  return {
    ...book,
    available: 1, // Default value
    category: book.categoryId ? book.category : "Uncategorized",
  }
}

/**
 * Create a new book
 */
export async function createBook(book: Omit<BookData, "id">): Promise<BookData> {
  const createOptions: BookCreateOptions = {
    title: book.title,
    author: book.author,
    categoryId: book.categoryId,
    inputQuantity: book.available || 1,
  }

  const createdBook = await apiRequest<BookItem>("/books", {
    method: "POST",
    body: JSON.stringify(createOptions),
  })

  return {
    ...createdBook,
    available: book.available || 1,
    category: book.category || "Uncategorized",
  }
}

/**
 * Update a book
 */
export async function updateBook(id: string, book: Partial<BookData>): Promise<BookData> {
  const updatedBook = await apiRequest<BookItem>(`/books/${id}`, { method: "PUT" }, true, {
    Title: book.title,
    Author: book.author,
    InputQuantity: book.available || 1,
    CategoryId: book.categoryId,
  })

  return {
    ...updatedBook,
    available: book.available || 1,
    category: book.category || "Uncategorized",
  }
}

/**
 * Delete a book
 */
export async function deleteBook(id: string): Promise<void> {
  await apiRequest(`/books/${id}`, { method: "DELETE" })
}
