"use client"

import Link from "next/link"
import { usePathname } from "next/navigation"
import { Book, LayoutDashboard, User, Settings, BookOpen, Clock, CheckSquare } from "lucide-react"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { ThemeToggle } from "@/components/theme-toggle"
import { useAuth } from "@/contexts/auth-context"

export function Sidebar() {
  const pathname = usePathname()
  const { isLoggedIn } = useAuth()

  // Don't render sidebar if not logged in
  if (!isLoggedIn) {
    return null
  }

  const navItems = [
    {
      name: "Dashboard",
      href: "/",
      icon: LayoutDashboard,
    },
    {
      name: "My Borrowings",
      href: "/borrowings",
      icon: Book,
    },
    {
      name: "Borrow Books",
      href: "/user/borrow",
      icon: BookOpen,
    },
    {
      name: "My Requests",
      href: "/user/requests",
      icon: Clock,
    },
    {
      name: "Admin: Categories",
      href: "/admin/categories",
      icon: Settings,
    },
    {
      name: "Admin: Books",
      href: "/admin/books",
      icon: Settings,
    },
    {
      name: "Admin: Requests",
      href: "/admin/requests",
      icon: CheckSquare,
    },
  ]

  return (
    <div className="w-[240px] border-r bg-background h-screen flex flex-col">
      <div className="p-6">
        <h2 className="text-lg font-semibold">Library System</h2>
      </div>
      <nav className="space-y-1 px-2">
        {navItems.map((item) => (
          <Link key={item.name} href={item.href}>
            <Button
              variant={pathname === item.href ? "secondary" : "ghost"}
              className={cn("w-full justify-start", pathname === item.href ? "bg-secondary" : "hover:bg-secondary/50")}
            >
              <item.icon className="h-5 w-5 mr-3" />
              {item.name}
            </Button>
          </Link>
        ))}
      </nav>
      <div className="mt-auto">
        <div className="px-4 py-2 border-t">
          <ThemeToggle />
        </div>
        <div className="p-4 border-t">
          <div className="flex items-center gap-3">
            <div className="h-9 w-9 rounded-full bg-secondary flex items-center justify-center">
              <User className="h-4 w-4" />
            </div>
            <div>
              <div className="text-sm font-medium">User</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
