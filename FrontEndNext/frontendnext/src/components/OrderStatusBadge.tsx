import { OrderStatusType } from "@/lib/types/order";
import { getOrderStatusDisplay } from "@/lib/utils/orderUtils";
import { Badge } from "@/components/ui/badge";

interface OrderStatusBadgeProps {
  status: OrderStatusType;
  showIcon?: boolean;
  showDescription?: boolean;
  className?: string;
}

export function OrderStatusBadge({
  status,
  showIcon = true,
  showDescription = false,
  className = "",
}: OrderStatusBadgeProps) {
  const config = getOrderStatusDisplay(status);

  return (
    <div className={`flex items-center gap-2 ${className}`}>
      <Badge className={`${config.bgColor} ${config.color} border-0`}>
        {showIcon && <span className="mr-1">{config.icon}</span>}
        {config.label}
      </Badge>
      {showDescription && (
        <span className="text-sm text-gray-600">{config.description}</span>
      )}
    </div>
  );
}
