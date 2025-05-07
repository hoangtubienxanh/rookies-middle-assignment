import { createRootRoute, createRoute, createRouter } from "@tanstack/react-router"

// Root layout
import { RootLayout } from "@/components/layouts/root-layout"

// Pages
import { HomePage } from "@/pages/home"
import { BorrowingsPage } from "@/pages/borrowings"
import { UserBorrowPage } from "@/pages/user/borrow"
import { UserRequestsPage } from "@/pages/user/requests"
import { AdminCategoriesPage } from "@/pages/admin/categories"
import { AdminBooksPage } from "@/pages/admin/books"

// Define routes
export const rootRoute = createRootRoute({
  component: RootLayout,
})

export const indexRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/",
  component: HomePage,
})

export const borrowingsRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/borrowings",
  component: BorrowingsPage,
})

// User routes
export const userBorrowRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/user/borrow",
  component: UserBorrowPage,
})

export const userRequestsRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/user/requests",
  component: UserRequestsPage,
})

// Admin routes
export const adminCategoriesRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/admin/categories",
  component: AdminCategoriesPage,
})

export const adminBooksRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/admin/books",
  component: AdminBooksPage,
})

// Create the router
export const routeTree = rootRoute.addChildren([
  indexRoute,
  borrowingsRoute,
  userBorrowRoute,
  userRequestsRoute,
  adminCategoriesRoute,
  adminBooksRoute,
])

export const router = createRouter({ routeTree })

// Types
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router
  }
}
