"use client"

import type React from "react"

import { useEffect, useState } from "react"
import { useRouter, usePathname } from "next/navigation"
import { useAuth } from "@/contexts/auth-context"

interface ProtectedRouteProps {
  children: React.ReactNode
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { isLoggedIn, isLoading } = useAuth()
  const router = useRouter()
  const pathname = usePathname()
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    // Skip if still loading
    if (isLoading) return

    try {
      // Define public paths that don't require authentication
      const isPublicPath = pathname === "/login" || pathname === "/register"

      if (!isLoggedIn && !isPublicPath) {
        // Redirect to login if not authenticated and not on a public path
        router.push("/login")
      } else if (isLoggedIn && isPublicPath) {
        // Redirect to home if authenticated and on a public path
        router.push("/")
      }
    } catch (err) {
      console.error("Navigation error:", err)
      setError("An error occurred during navigation. Please refresh the page.")
    }
  }, [isLoggedIn, isLoading, pathname, router])

  // Show loading state while checking authentication
  if (isLoading) {
    return <div className="flex items-center justify-center min-h-screen">Loading...</div>
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen flex-col">
        <p className="text-red-500 mb-4">{error}</p>
        <button
          onClick={() => window.location.reload()}
          className="px-4 py-2 bg-primary text-primary-foreground rounded"
        >
          Refresh Page
        </button>
      </div>
    )
  }

  // Define public paths that don't require authentication
  const isPublicPath = pathname === "/login" || pathname === "/register"

  // Render children if authenticated or on a public path
  if (isLoggedIn || isPublicPath) {
    return <>{children}</>
  }

  // This will briefly show before the redirect happens
  return <div className="flex items-center justify-center min-h-screen">Redirecting...</div>
}
