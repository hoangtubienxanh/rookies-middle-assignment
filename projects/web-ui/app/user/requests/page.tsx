"use client"

import { useState, useEffect } from "react"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { getUserRequests, type BookRequest } from "@/lib/api/book-requests"
import { useToast } from "@/hooks/use-toast"

export default function UserRequestsPage() {
  const [requests, setRequests] = useState<BookRequest[]>([])
  const [loading, setLoading] = useState(true)
  const { toast } = useToast()

  useEffect(() => {
    const loadRequests = async () => {
      setLoading(true)
      try {
        const data = await getUserRequests("user1")
        setRequests(data)
      } catch (error) {
        console.error("Failed to fetch requests:", error)
        toast({
          title: "Error",
          description: "Failed to load your requests. Please try again.",
          variant: "destructive",
        })
      } finally {
        setLoading(false)
      }
    }

    loadRequests()
  }, [toast])

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    })
  }

  const getStatusBadge = (status: string) => {
    switch (status) {
      case "approved":
        return <Badge variant="success">Approved</Badge>
      case "rejected":
        return <Badge variant="destructive">Rejected</Badge>
      default:
        return <Badge variant="outline">Pending</Badge>
    }
  }

  const getStatusMessage = (status: string) => {
    switch (status) {
      case "approved":
        return "All books in this request have been approved and are ready for pickup."
      case "rejected":
        return "This request has been rejected. None of the books are available for borrowing."
      default:
        return "This request is pending approval from the librarian."
    }
  }

  return (
    <>
      <h1 className="text-2xl font-bold mb-6">My Borrow Requests</h1>

      <Card>
        <CardHeader>
          <CardTitle>Request History</CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="text-center py-6">Loading requests...</div>
          ) : requests.length === 0 ? (
            <div className="text-center py-6">
              <p className="text-muted-foreground">You haven't made any borrow requests yet.</p>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Request Date</TableHead>
                  <TableHead>Books</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Details</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {requests.map((request) => (
                  <TableRow key={request.id}>
                    <TableCell>{formatDate(request.requestDate)}</TableCell>
                    <TableCell>
                      <div className="space-y-1">
                        {request.books.map((book) => (
                          <div key={book.id}>{book.title}</div>
                        ))}
                        <div className="text-xs text-muted-foreground mt-1">Total: {request.books.length} book(s)</div>
                      </div>
                    </TableCell>
                    <TableCell>{getStatusBadge(request.status)}</TableCell>
                    <TableCell>
                      <p className="text-sm text-muted-foreground">{getStatusMessage(request.status)}</p>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </CardContent>
      </Card>
    </>
  )
}
