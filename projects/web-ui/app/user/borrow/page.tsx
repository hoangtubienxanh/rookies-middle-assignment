"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Checkbox } from "@/components/ui/checkbox"
import { Button } from "@/components/ui/button"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { AlertCircle, Info } from "lucide-react"
import { useToast } from "@/hooks/use-toast"
import { fetchBooks, type BookData } from "@/lib/api/books"
import { submitBookRequest } from "@/lib/api/book-requests"
import { CategoryBadge } from "@/components/category-badge"
import { AvailabilityIndicator } from "@/components/availability-indicator"
import { Pagination } from "@/components/pagination"

export default function UserBorrowPage() {
  const [books, setBooks] = useState<BookData[]>([])
  const [selectedBooks, setSelectedBooks] = useState<string[]>([])
  const [currentPage, setCurrentPage] = useState(1)
  const [totalPages, setTotalPages] = useState(5)
  const [recordsPerPage, setRecordsPerPage] = useState(20)
  const [totalRecords, setTotalRecords] = useState(94)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const { toast } = useToast()
  const router = useRouter()

  useEffect(() => {
    const loadBooks = async () => {
      setLoading(true)
      try {
        const data = await fetchBooks(currentPage, recordsPerPage)
        setBooks(data.books)
        setTotalRecords(data.totalRecords)
        setTotalPages(Math.ceil(data.totalRecords / recordsPerPage))
      } catch (error) {
        console.error("Failed to fetch books:", error)
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
    setSelectedBooks((prev) => {
      const newSelection = prev.includes(bookId) ? prev.filter((id) => id !== bookId) : [...prev, bookId]

      return newSelection
    })
  }

  const handleSelectAll = () => {
    if (selectedBooks.length === books.length) {
      setSelectedBooks([])
    } else {
      // Only select up to 5 books
      setSelectedBooks(books.slice(0, 5).map((book) => book.id))
    }
  }

  const handleSubmitRequest = async () => {
    if (selectedBooks.length === 0) {
      toast({
        title: "No books selected",
        description: "Please select at least one book to borrow.",
        variant: "destructive",
      })
      return
    }

    if (selectedBooks.length > 5) {
      toast({
        title: "Too many books selected",
        description: "You can only borrow up to 5 books in a single request.",
        variant: "destructive",
      })
      return
    }

    setSubmitting(true)
    try {
      await submitBookRequest("user1", selectedBooks)
      toast({
        title: "Success",
        description: `Your request for ${selectedBooks.length} book(s) has been submitted. If approved, all books will be borrowed together.`,
      })
      setSelectedBooks([])
      router.push("/user/requests")
    } catch (error: any) {
      toast({
        title: "Error",
        description: error.message || "Failed to submit request. Please try again.",
        variant: "destructive",
      })
    } finally {
      setSubmitting(false)
    }
  }

  const handlePageChange = (page: number) => {
    setCurrentPage(page)
  }

  const handleRecordsPerPageChange = (value: string) => {
    setRecordsPerPage(Number(value))
    setCurrentPage(1)
  }

  return (
    <>
      <h1 className="text-2xl font-bold mb-6">Borrow Books</h1>

      <Alert variant="info" className="mb-6">
        <Info className="h-4 w-4" />
        <AlertTitle>Request Information</AlertTitle>
        <AlertDescription>
          You can request to borrow 1-5 books at a time. All books in a request are processed together - if approved,
          you'll receive all books; if denied, you'll receive none.
        </AlertDescription>
      </Alert>

      {selectedBooks.length >= 5 && (
        <Alert variant="warning" className="mb-6">
          <AlertCircle className="h-4 w-4" />
          <AlertTitle>Maximum selection reached</AlertTitle>
          <AlertDescription>
            You have selected {selectedBooks.length} books. The maximum allowed is 5 books per request.
          </AlertDescription>
        </Alert>
      )}

      <Card>
        <CardHeader>
          <CardTitle>Available Books</CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-12">
                  <Checkbox
                    checked={selectedBooks.length === books.length && books.length > 0 && books.length <= 5}
                    onCheckedChange={handleSelectAll}
                    aria-label="Select all books"
                    disabled={books.length > 5}
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
                        disabled={selectedBooks.length >= 5 && !selectedBooks.includes(book.id)}
                      />
                    </TableCell>
                    <TableCell>{book.title}</TableCell>
                    <TableCell>{book.author}</TableCell>
                    <TableCell>
                      <CategoryBadge category={book.category} />
                    </TableCell>
                    <TableCell>
                      <AvailabilityIndicator count={book.available} />
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
                <Button
                  onClick={handleSubmitRequest}
                  disabled={selectedBooks.length === 0 || selectedBooks.length > 5 || submitting}
                >
                  {submitting ? "Submitting..." : "Submit Borrow Request"}
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
              Showing {(currentPage - 1) * recordsPerPage + 1} to {Math.min(currentPage * recordsPerPage, totalRecords)}{" "}
              of {totalRecords} records
            </div>
          </div>
        </CardFooter>

        <div className="p-4 border-t">
          <Pagination currentPage={currentPage} totalPages={totalPages} onPageChange={handlePageChange} />
        </div>
      </Card>
    </>
  )
}
