"use client"

import Link from "next/link"
import { useAuth } from "@/contexts/auth-context"
import { ThemeToggle } from "@/components/theme-toggle"
import { Button } from "@/components/ui/button"
import { LogOut, LogIn } from "lucide-react"

export function Header() {
  const { isLoggedIn, logout } = useAuth()

  return (
    <header className="border-b">
      <div className="flex h-16 items-center px-4 sm:px-6 justify-between">
        <div></div>
        <div className="flex items-center space-x-4">
          <ThemeToggle />
          {isLoggedIn ? (
            <Button variant="outline" size="sm" onClick={logout}>
              <LogOut className="h-4 w-4 mr-2" />
              Logout
            </Button>
          ) : (
            <Link href="/login">
              <Button variant="outline" size="sm">
                <LogIn className="h-4 w-4 mr-2" />
                Login
              </Button>
            </Link>
          )}
        </div>
      </div>
    </header>
  )
}
