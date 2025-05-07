import { cn } from "@/lib/utils"

interface AvailabilityIndicatorProps {
  count: number
}

export function AvailabilityIndicator({ count }: AvailabilityIndicatorProps) {
  return (
    <div className="flex items-center gap-2">
      <span className={cn("inline-block w-2 h-2 rounded-full", count > 0 ? "bg-green-500" : "bg-red-500")}></span>
      <span className="text-sm">{count} available</span>
    </div>
  )
}
