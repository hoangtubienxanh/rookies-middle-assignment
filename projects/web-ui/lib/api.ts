export interface BookData {
  id: string
  title: string
  author: string
  category: string
  available: number
}

// Mock data based on the image
const mockBooks: BookData[] = [
  {
    id: "1",
    title: "A Brief History of Time",
    author: "Stephen Hawking",
    category: "Science",
    available: 2,
  },
  {
    id: "2",
    title: "The Selfish Gene",
    author: "Richard Dawkins",
    category: "Science",
    available: 3,
  },
  {
    id: "3",
    title: "Cosmos",
    author: "Carl Sagan",
    category: "Science",
    available: 2,
  },
  {
    id: "4",
    title: "Clean Code",
    author: "Robert C. Martin",
    category: "Technology",
    available: 4,
  },
  {
    id: "5",
    title: "The Pragmatic Programmer",
    author: "Andrew Hunt and David Thomas",
    category: "Technology",
    available: 3,
  },
  {
    id: "6",
    title: "Design Patterns",
    author: "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
    category: "Technology",
    available: 2,
  },
  {
    id: "7",
    title: "Artificial Intelligence: A Modern Approach",
    author: "Stuart Russell and Peter Norvig",
    category: "Technology",
    available: 3,
  },
  {
    id: "8",
    title: "Sapiens: A Brief History of Humankind",
    author: "Yuval Noah Harari",
    category: "History",
    available: 3,
  },
  {
    id: "9",
    title: "Guns, Germs, and Steel",
    author: "Jared Diamond",
    category: "History",
    available: 2,
  },
  {
    id: "10",
    title: "The Silk Roads",
    author: "Peter Frankopan",
    category: "History",
    available: 3,
  },
  {
    id: "11",
    title: "The Catcher in the Rye",
    author: "J.D. Salinger",
    category: "Fiction",
    available: 4,
  },
  {
    id: "12",
    title: "One Hundred Years of Solitude",
    author: "Gabriel García Márquez",
    category: "Fiction",
    available: 3,
  },
]

// Generate more mock data
for (let i = 13; i <= 94; i++) {
  const categories = ["Science", "Technology", "History", "Fiction"]
  const category = categories[Math.floor(Math.random() * categories.length)]

  mockBooks.push({
    id: i.toString(),
    title: `Book Title ${i}`,
    author: `Author ${i}`,
    category,
    available: Math.floor(Math.random() * 5) + 1,
  })
}

// Simulate API call with pagination
export async function fetchBooks(page: number, limit: number) {
  // Simulate network delay
  await new Promise((resolve) => setTimeout(resolve, 500))

  const startIndex = (page - 1) * limit
  const endIndex = startIndex + limit

  return {
    books: mockBooks.slice(startIndex, endIndex),
    totalRecords: mockBooks.length,
  }
}

// Function to borrow books (would be an API call in a real app)
export async function borrowBooks(bookIds: string[]) {
  // Simulate network delay
  await new Promise((resolve) => setTimeout(resolve, 1000))

  // In a real app, this would make an API call to update the database
  return {
    success: true,
    message: `Successfully borrowed ${bookIds.length} books`,
  }
}
