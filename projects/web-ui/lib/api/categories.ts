import { apiRequest } from "./api-client"
import type { Category as CategoryType, PaginatedCategories } from "@/types/api"

export type Category = CategoryType

/**
 * Get all categories with pagination
 */
export async function getCategories(page = 1, pageSize = 10): Promise<Category[]> {
  const response = await apiRequest<PaginatedCategories>("/category", { method: "GET" }, true, {
    pageIndex: page - 1,
    pageSize,
  })

  return response.data
}

/**
 * Get a category by ID
 */
export async function getCategoryById(id: string): Promise<Category> {
  return await apiRequest<Category>(`/category/${id}`)
}

/**
 * Create a new category
 */
export async function createCategory(category: Omit<Category, "categoryId" | "slug">): Promise<Category> {
  return await apiRequest<Category>("/category", { method: "POST" }, true, {
    Name: category.name,
  })
}

/**
 * Update a category
 */
export async function updateCategory(id: string, category: Partial<Category>): Promise<Category> {
  return await apiRequest<Category>(`/category/${id}`, { method: "PUT" }, true, {
    Name: category.name,
  })
}

/**
 * Delete a category
 */
export async function deleteCategory(id: string): Promise<void> {
  await apiRequest(`/category/${id}`, { method: "DELETE" })
}
