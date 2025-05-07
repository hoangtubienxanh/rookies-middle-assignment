"use client"

import { useState, useEffect } from "react"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { CheckCircle, XCircle } from "lucide-react"
import { useToast } from "@/hooks/use-toast"
import { getAllRequests, approveRequest, rejectRequest, type BookRequest } from "@/lib/api/book-requests"

export default function AdminRequestsPage() {
  const [requests, setRequests] = useState<BookRequest[]>([])
  const [loading, setLoading] = useState(true)
  const [processing, setProcessing] = useState<string | null>(null)
  const { toast } = useToast()

  useEffect(() => {
    loadRequests()
  }, [])

  const loadRequests = async () => {
    setLoading(true)
    try {
      const data = await getAllRequests()
      setRequests(data)
    } catch (error) {
      console.error("Failed to fetch requests:", error)
      toast({
        title: "Error",
        description: "Failed to load requests. Please try again.",
        variant: "destructive",
      })
    } finally {
      setLoading(false)
    }
  }

  const handleApprove = async (requestId: string) => {
    setProcessing(requestId)
    try {
      await approveRequest(requestId)
      toast({
        title: "Success",
        description: "Request approved successfully. All books are now borrowed.",
      })
      loadRequests()
    } catch (error: any) {
      toast({
        title: "Error",
        description: error.message || "Failed to approve request. Please try again.",
        variant: "destructive",
      })
    } finally {
      setProcessing(null)
    }
  }

  const handleReject = async (requestId: string) => {
    setProcessing(requestId)
    try {
      await rejectRequest(requestId)
      toast({
        title: "Success",
        description: "Request rejected successfully.",
      })
      loadRequests()
    } catch (error: any) {
      toast({
        title: "Error",
        description: error.message || "Failed to reject request. Please try again.",
        variant: "destructive",
      })
    } finally {
      setProcessing(null)
    }
  }

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

  return (
    <>
      <h1 className="text-2xl font-bold mb-6">Manage Borrow Requests</h1>

      <Card>
        <CardHeader>
          <CardTitle>All Requests</CardTitle>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="text-center py-6">Loading requests...</div>
          ) : requests.length === 0 ? (
            <div className="text-center py-6">
              <p className="text-muted-foreground">No borrow requests found.</p>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>User</TableHead>
                  <TableHead>Request Date</TableHead>
                  <TableHead>Books</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {requests.map((request) => (
                  <TableRow key={request.id}>
                    <TableCell>User #{request.userId}</TableCell>
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
                      {request.status === "pending" ? (
                        <div className="flex space-x-2">
                          <Button
                            variant="outline"
                            size="sm"
                            className="flex items-center"
                            onClick={() => handleApprove(request.id)}
                            disabled={processing === request.id}
                          >
                            <CheckCircle className="h-4 w-4 mr-1" />
                            Approve All
                          </Button>
                          <Button
                            variant="outline"
                            size="sm"
                            className="flex items-center"
                            onClick={() => handleReject(request.id)}
                            disabled={processing === request.id}
                          >
                            <XCircle className="h-4 w-4 mr-1" />
                            Reject All
                          </Button>
                        </div>
                      ) : (
                        <span className="text-sm text-muted-foreground">
                          {request.status === "approved" ? "Approved on " : "Rejected on "}
                          {formatDate(request.updatedDate || request.requestDate)}
                        </span>
                      )}
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
