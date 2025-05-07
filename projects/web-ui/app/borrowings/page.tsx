import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import Link from "next/link"

export default function BorrowingsPage() {
  return (
    <>
      <h1 className="text-2xl font-bold mb-6">My Borrowings</h1>

      <Card>
        <CardHeader>
          <CardTitle>Borrowed Books</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <p className="text-muted-foreground mb-4">You haven't borrowed any books yet.</p>
          <Link href="/user/borrow">
            <Button>Browse Books to Borrow</Button>
          </Link>
        </CardContent>
      </Card>
    </>
  )
}
