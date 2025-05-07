import { Badge } from "@/components/ui/badge"

type CategoryType = "Science" | "Technology" | "History" | "Fiction" | string

interface CategoryBadgeProps {
  category: CategoryType
}

export function CategoryBadge({ category }: CategoryBadgeProps) {
  const getVariantByCategory = (category: CategoryType) => {
    switch (category) {
      case "Science":
        return "secondary"
      case "Technology":
        return "outline"
      case "History":
        return "default"
      case "Fiction":
        return "destructive"
      default:
        return "secondary"
    }
  }

  return <Badge variant={getVariantByCategory(category) as any}>{category}</Badge>
}
