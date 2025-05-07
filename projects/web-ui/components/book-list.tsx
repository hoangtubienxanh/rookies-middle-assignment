"use client"

import { useState, useEffect } from "react"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Checkbox } from "@/components/ui/checkbox"
import { Button } from "@/components/ui/button"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Card, CardContent, CardFooter } from "@/components/ui/card"
import { Plus } from "lucide-react"
import { type BookData, fetchBooks } from "@/lib/api/books"
import { CategoryBadge } from "@/components/category-badge"
import { AvailabilityIndicator } from "@/components/availability-indicator"
import { Pagination } from "@/components/pagination"
import { useToast } from "@/hooks/use-toast"

export function BookList() {
  const [books, setBooks] = useState<BookData[]>([])
  const [selectedBooks, setSelectedBooks] = useState<string[]>([])
  const [currentPage, setCurrentPage] = useState(1)
  const [totalPages, setTotalPages] = useState(5)
  const [recordsPerPage, setRecordsPerPage] = useState(20)
  const [totalRecords, setTotalRecords] = useState(0)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const { toast } = useToast()

  useEffect(() => {
    const loadBooks = async () => {
      setLoading(true)
      setError(null)
      try {
        const data = await fetchBooks(currentPage, recordsPerPage)
        setBooks(data.books)
        setTotalRecords(data.totalRecords)
        setTotalPages(Math.ceil(data.totalRecords / recordsPerPage))
      } catch (error) {
        console.error("Failed to fetch books:", error)
        setError("Failed to load books. Please try again.")
        toast({
          title: "Error",
          description: "Failed to load books. Please try again.",
          variant: "destructive",
        })
      } finally {
        setLoading(false)
      }
    }

    loadBooks()
  }, [currentPage, recordsPerPage, toast])

  const handleSelectBook = (bookId: string) => {
    setSelectedBooks((prev) => (prev.includes(bookId) ? prev.filter((id) => id !== bookId) : [...prev, bookId]))
  }

  const handleSelectAll = () => {
    if (selectedBooks.length === books.length) {
      setSelectedBooks([])
    } else {
      setSelectedBooks(books.map((book) => book.id))
    }
  }

  const handleBorrowSelected = () => {
    // In a real app, this would call an API to borrow the selected books
    alert(`Borrowing books: ${selectedBooks.join(", ")}`)
    setSelectedBooks([])
  }

  const handlePageChange = (page: number) => {
    setCurrentPage(page)
  }

  const handleRecordsPerPageChange = (value: string) => {
    setRecordsPerPage(Number(value))
    setCurrentPage(1)
  }

  if (error) {
    return (
      <Card>
        <CardContent className="p-6 text-center">
          <p className="text-red-500 mb-4">{error}</p>
          <Button onClick={() => window.location.reload()}>Retry</Button>
        </CardContent>
      </Card>
    )
  }

  return (
    <Card>
      <CardContent className="p-0">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-12">
                <Checkbox
                  checked={selectedBooks.length === books.length && books.length > 0}
                  onCheckedChange={handleSelectAll}
                  aria-label="Select all books"
                />
              </TableHead>
              <TableHead>Title</TableHead>
              <TableHead>Author</TableHead>
              <TableHead>Category</TableHead>
              <TableHead>Availability</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell colSpan={5} className="text-center py-6">
                  Loading books...
                </TableCell>
              </TableRow>
            ) : books.length === 0 ? (
              <TableRow>
                <TableCell colSpan={5} className="text-center py-6">
                  No books found
                </TableCell>
              </TableRow>
            ) : (
              books.map((book) => (
                <TableRow key={book.id}>
                  <TableCell>
                    <Checkbox
                      checked={selectedBooks.includes(book.id)}
                      onCheckedChange={() => handleSelectBook(book.id)}
                      aria-label={`Select ${book.title}`}
                    />
                  </TableCell>
                  <TableCell>{book.title}</TableCell>
                  <TableCell>{book.author}</TableCell>
                  <TableCell>
                    <CategoryBadge category={book.category || "Uncategorized"} />
                  </TableCell>
                  <TableCell>
                    <AvailabilityIndicator count={book.available || 0} />
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </CardContent>

      <CardFooter className="flex flex-col sm:flex-row justify-between items-center gap-4 p-4 border-t">
        <div>
          {selectedBooks.length > 0 ? (
            <div className="flex items-center gap-4">
              <span className="text-sm text-muted-foreground">{selectedBooks.length} books selected</span>
              <Button onClick={handleBorrowSelected} size="sm">
                <Plus className="h-4 w-4 mr-2" />
                Borrow Selected Books ({selectedBooks.length}/5)
              </Button>
            </div>
          ) : (
            <span className="text-sm text-muted-foreground">No books selected</span>
          )}
        </div>

        <div className="flex flex-col sm:flex-row items-center gap-4">
          <div className="flex items-center gap-2">
            <span className="text-sm text-muted-foreground">Show</span>
            <Select value={recordsPerPage.toString()} onValueChange={handleRecordsPerPageChange}>
              <SelectTrigger className="w-[70px]">
                <SelectValue placeholder={recordsPerPage.toString()} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="10">10</SelectItem>
                <SelectItem value="20">20</SelectItem>
                <SelectItem value="50">50</SelectItem>
                <SelectItem value="100">100</SelectItem>
              </SelectContent>
            </Select>
            <span className="text-sm text-muted-foreground">records per page</span>
          </div>

          <div className="text-sm text-muted-foreground">
            Showing {totalRecords === 0 ? 0 : (currentPage - 1) * recordsPerPage + 1} to{" "}
            {Math.min(currentPage * recordsPerPage, totalRecords)} of {totalRecords} records
          </div>
        </div>
      </CardFooter>

      <div className="p-4 border-t">
        <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={handlePageChange} />
      </div>
    </Card>
  )
}
