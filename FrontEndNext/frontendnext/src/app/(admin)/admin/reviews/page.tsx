"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Search,
  Edit,
  Trash2,
  Loader2,
  Star,
  Check,
  X,
  Eye,
} from "lucide-react";
import { toast } from "sonner";
import {
  useGetAllReviewsQuery,
  useUpdateReviewMutation,
  useDeleteReviewMutation,
} from "@/lib/services/reviewService";
import { useGetProductsQuery } from "@/lib/services/productService";
import { useGetUsersQuery } from "@/lib/services/userService";
import { Review, ReviewUpdateDto } from "@/lib/types/review";
import { Product } from "@/lib/types/product";
import { User } from "@/lib/types/user";

type ReviewStatus = "Pending" | "Approved" | "Rejected";

export default function ReviewsPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<ReviewStatus | "All">("All");
  const [isViewDialogOpen, setIsViewDialogOpen] = useState(false);
  const [selectedReview, setSelectedReview] = useState<Review | null>(null);

  // RTK Query hooks
  const { data: reviews = [], isLoading, error } = useGetAllReviewsQuery();
  const { data: productsData } = useGetProductsQuery({
    pageNumber: 1,
    pageSize: 1000,
  });
  const { data: users = [] } = useGetUsersQuery();
  const products: Product[] = productsData?.items || [];

  const [updateReview, { isLoading: isUpdating }] = useUpdateReviewMutation();
  const [deleteReview, { isLoading: isDeleting }] = useDeleteReviewMutation();

  const handleApprove = async (review: Review) => {
    try {
      await updateReview({
        id: review.id,
        review: {
          productId: review.productId,
          accountId: review.accountId,
          rating: review.rating,
          comment: review.comment,
          isApproved: true,
        },
      }).unwrap();
      toast.success("Review approved successfully");
    } catch (error) {
      toast.error("Failed to approve review");
      console.error("Error approving review:", error);
    }
  };

  const handleReject = async (review: Review) => {
    try {
      await updateReview({
        id: review.id,
        review: {
          productId: review.productId,
          accountId: review.accountId,
          rating: review.rating,
          comment: review.comment,
          isApproved: false,
        },
      }).unwrap();
      toast.success("Review rejected successfully");
    } catch (error) {
      toast.error("Failed to reject review");
      console.error("Error rejecting review:", error);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteReview(id).unwrap();
      toast.success("Review deleted successfully");
    } catch (error) {
      toast.error("Failed to delete review");
      console.error("Error deleting review:", error);
    }
  };

  const openViewDialog = (review: Review) => {
    setSelectedReview(review);
    setIsViewDialogOpen(true);
  };

  const renderStars = (rating: number) => {
    return (
      <div className="flex items-center">
        {[1, 2, 3, 4, 5].map((star) => (
          <Star
            key={star}
            className={`h-4 w-4 ${
              star <= rating ? "text-yellow-400 fill-current" : "text-gray-300"
            }`}
          />
        ))}
        <span className="ml-1 text-sm text-gray-600">({rating})</span>
      </div>
    );
  };

  const getProductName = (productId: number) => {
    const product = products.find((p: Product) => p.id === productId);
    return product ? product.name : `Product ${productId}`;
  };

  const getUserName = (userId: number) => {
    const user = users.find((u: User) => u.id === userId);
    return user ? user.fullName : `User ${userId}`;
  };

  const getStatusBadge = (isApproved: boolean | null | undefined) => {
    if (isApproved === null || isApproved === undefined) {
      return <Badge variant="secondary">Pending</Badge>;
    }
    return isApproved ? (
      <Badge variant="default">Approved</Badge>
    ) : (
      <Badge variant="destructive">Rejected</Badge>
    );
  };

  const getStatusFilter = (
    isApproved: boolean | null | undefined
  ): ReviewStatus => {
    if (isApproved === null || isApproved === undefined) {
      return "Pending";
    }
    return isApproved ? "Approved" : "Rejected";
  };

  const filteredReviews = reviews.filter((review) => {
    const matchesSearch =
      getProductName(review.productId)
        .toLowerCase()
        .includes(searchTerm.toLowerCase()) ||
      getUserName(review.accountId)
        .toLowerCase()
        .includes(searchTerm.toLowerCase()) ||
      review.comment.toLowerCase().includes(searchTerm.toLowerCase());

    const matchesStatus =
      statusFilter === "All" ||
      getStatusFilter(review.isApproved) === statusFilter;

    return matchesSearch && matchesStatus;
  });

  const pendingCount = reviews.filter(
    (r) => getStatusFilter(r.isApproved) === "Pending"
  ).length;
  const approvedCount = reviews.filter(
    (r) => getStatusFilter(r.isApproved) === "Approved"
  ).length;
  const rejectedCount = reviews.filter(
    (r) => getStatusFilter(r.isApproved) === "Rejected"
  ).length;

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-red-500">Failed to load reviews</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Reviews Management</h1>
      </div>

      {/* Statistics */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div className="bg-white p-4 rounded-lg border">
          <div className="text-2xl font-bold text-blue-600">
            {reviews.length}
          </div>
          <div className="text-sm text-gray-600">Total Reviews</div>
        </div>
        <div className="bg-white p-4 rounded-lg border">
          <div className="text-2xl font-bold text-yellow-600">
            {pendingCount}
          </div>
          <div className="text-sm text-gray-600">Pending</div>
        </div>
        <div className="bg-white p-4 rounded-lg border">
          <div className="text-2xl font-bold text-green-600">
            {approvedCount}
          </div>
          <div className="text-sm text-gray-600">Approved</div>
        </div>
        <div className="bg-white p-4 rounded-lg border">
          <div className="text-2xl font-bold text-red-600">{rejectedCount}</div>
          <div className="text-sm text-gray-600">Rejected</div>
        </div>
      </div>

      {/* Filters */}
      <div className="flex items-center space-x-4">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search reviews..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
        <Select
          value={statusFilter}
          onValueChange={(value) =>
            setStatusFilter(value as ReviewStatus | "All")
          }
        >
          <SelectTrigger className="w-48">
            <SelectValue placeholder="Filter by status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="All">All Status</SelectItem>
            <SelectItem value="Pending">Pending</SelectItem>
            <SelectItem value="Approved">Approved</SelectItem>
            <SelectItem value="Rejected">Rejected</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Reviews Table */}
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Product</TableHead>
              <TableHead>User</TableHead>
              <TableHead>Rating</TableHead>
              <TableHead>Comment</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Date</TableHead>
              <TableHead>Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredReviews.map((review) => (
              <TableRow key={review.id}>
                <TableCell className="font-medium">
                  {getProductName(review.productId)}
                </TableCell>
                <TableCell>{getUserName(review.accountId)}</TableCell>
                <TableCell>{renderStars(review.rating)}</TableCell>
                <TableCell className="max-w-xs truncate">
                  {review.comment}
                </TableCell>
                <TableCell>{getStatusBadge(review.isApproved)}</TableCell>
                <TableCell>
                  {new Date(review.createdAt).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  <div className="flex items-center space-x-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => openViewDialog(review)}
                    >
                      <Eye className="h-4 w-4" />
                    </Button>
                    {getStatusFilter(review.isApproved) === "Pending" && (
                      <>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleApprove(review)}
                          disabled={isUpdating}
                          className="text-green-600 hover:text-green-700"
                        >
                          <Check className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleReject(review)}
                          disabled={isUpdating}
                          className="text-red-600 hover:text-red-700"
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </>
                    )}
                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button variant="outline" size="sm">
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </AlertDialogTrigger>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>Delete Review</AlertDialogTitle>
                          <AlertDialogDescription>
                            Are you sure you want to delete this review? This
                            action cannot be undone.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancel</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => handleDelete(review.id)}
                            className="bg-red-600 hover:bg-red-700"
                          >
                            Delete
                          </AlertDialogAction>
                        </AlertDialogFooter>
                      </AlertDialogContent>
                    </AlertDialog>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {/* View Review Dialog */}
      <Dialog open={isViewDialogOpen} onOpenChange={setIsViewDialogOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Review Details</DialogTitle>
            <DialogDescription>View and manage this review.</DialogDescription>
          </DialogHeader>
          {selectedReview && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Product</Label>
                  <p className="text-sm text-gray-600">
                    {getProductName(selectedReview.productId)}
                  </p>
                </div>
                <div>
                  <Label>User</Label>
                  <p className="text-sm text-gray-600">
                    {getUserName(selectedReview.accountId)}
                  </p>
                </div>
                <div>
                  <Label>Rating</Label>
                  <div className="mt-1">
                    {renderStars(selectedReview.rating)}
                  </div>
                </div>
                <div>
                  <Label>Status</Label>
                  <div className="mt-1">
                    {getStatusBadge(selectedReview.isApproved)}
                  </div>
                </div>
                <div>
                  <Label>Date</Label>
                  <p className="text-sm text-gray-600">
                    {new Date(selectedReview.createdAt).toLocaleString()}
                  </p>
                </div>
              </div>
              <div>
                <Label>Comment</Label>
                <div className="mt-1 p-3 bg-gray-50 rounded-md">
                  {selectedReview.comment}
                </div>
              </div>
            </div>
          )}
          <DialogFooter>
            {selectedReview &&
              getStatusFilter(selectedReview.isApproved) === "Pending" && (
                <div className="flex space-x-2">
                  <Button
                    onClick={() => handleApprove(selectedReview)}
                    disabled={isUpdating}
                    className="bg-green-600 hover:bg-green-700"
                  >
                    <Check className="h-4 w-4 mr-2" />
                    Approve
                  </Button>
                  <Button
                    onClick={() => handleReject(selectedReview)}
                    disabled={isUpdating}
                    variant="destructive"
                  >
                    <X className="h-4 w-4 mr-2" />
                    Reject
                  </Button>
                </div>
              )}
            <Button
              variant="outline"
              onClick={() => setIsViewDialogOpen(false)}
            >
              Close
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
