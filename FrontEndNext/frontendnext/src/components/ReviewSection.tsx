import React, { useState } from "react";
import { useAppSelector } from "@/lib/store/hooks";
import {
  useGetReviewsByProductQuery,
  useCreateReviewMutation,
} from "@/lib/services/reviewService";
import { ReviewCreate } from "@/lib/types/review";
import { Button } from "@/components/ui/button";
import { Star } from "lucide-react";

interface ReviewSectionProps {
  productId: number;
}

export default function ReviewSection({ productId }: ReviewSectionProps) {
  const { data: reviews = [], isLoading } =
    useGetReviewsByProductQuery(productId);
  const [showForm, setShowForm] = useState(false);
  const [rating, setRating] = useState(0);
  const [comment, setComment] = useState("");
  const [createReview, { isLoading: isSubmitting }] = useCreateReviewMutation();
  const user = useAppSelector((state) => state.auth.user);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    if (!user) {
      setError("You must be logged in to write a review.");
      return;
    }
    if (rating < 1 || rating > 5) {
      setError("Please select a rating.");
      return;
    }
    if (!comment.trim()) {
      setError("Please enter your comment.");
      return;
    }
    const payload: ReviewCreate = {
      productId,
      accountId: user.id,
      rating,
      comment,
    };
    try {
      await createReview(payload).unwrap();
      setShowForm(false);
      setRating(0);
      setComment("");
    } catch (err: any) {
      setError(err?.data?.error || "Failed to submit review");
    }
  };

  const ratingStats = [5, 4, 3, 2, 1].map((star) => ({
    star,
    count: reviews.filter((r) => r.rating === star).length,
  }));
  const total = reviews.length;

  return (
    <section className="mt-12">
      <h2 className="text-2xl font-bold text-center mb-6">
        WHAT OUR CUSTOMERS ARE SAYING
      </h2>
      <div className="flex flex-col md:flex-row md:justify-center md:items-center md:gap-8 mb-8">
        <div className="flex-1 flex flex-col items-center mb-4 md:mb-0 min-w-[180px]">
          <div className="flex items-center gap-2 text-xl font-semibold">
            <span>
              {total > 0
                ? (
                    reviews.reduce((sum, r) => sum + r.rating, 0) / total
                  ).toFixed(2)
                : "0.00"}
            </span>
            <Star className="h-5 w-5 text-yellow-400 fill-yellow-400" />
            <span className="text-base font-normal">out of 5</span>
          </div>
          <span className="text-muted-foreground text-sm">
            Based on {total} review{total !== 1 ? "s" : ""}
          </span>
        </div>
        <div className="flex-1 flex flex-col gap-1 min-w-[200px]">
          {ratingStats.map((stat) => (
            <div key={stat.star} className="flex items-center gap-2">
              <span className="w-8 text-right">
                {stat.star}{" "}
                <Star className="inline h-4 w-4 text-yellow-400 fill-yellow-400" />
              </span>
              <div
                className="flex-1 h-2 bg-gray-200 rounded mx-2 relative"
                style={{ minWidth: 120 }}
              >
                <div
                  className="h-2 bg-yellow-400 rounded"
                  style={{
                    width: total > 0 ? `${(stat.count / total) * 100}%` : "0%",
                    transition: "width 0.3s",
                  }}
                />
              </div>
              <span className="w-4 text-right">{stat.count}</span>
            </div>
          ))}
        </div>
        <div className="flex-1 flex flex-col items-center justify-center min-w-[180px] mt-4 md:mt-0">
          <Button onClick={() => setShowForm((v) => !v)} className="w-48">
            {showForm ? "CANCEL REVIEW" : "WRITE A REVIEW"}
          </Button>
        </div>
      </div>
      {showForm && (
        <form
          onSubmit={handleSubmit}
          className="border rounded-lg p-6 mb-8 max-w-xl mx-auto bg-white dark:bg-zinc-900 shadow"
        >
          <div className="flex flex-col items-center mb-4">
            <div className="flex gap-1 mb-2">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  type="button"
                  key={star}
                  onClick={() => setRating(star)}
                  className={
                    star <= rating
                      ? "text-yellow-400"
                      : "text-gray-300 dark:text-zinc-700"
                  }
                  aria-label={`Rate ${star} star${star > 1 ? "s" : ""}`}
                >
                  <Star className="h-7 w-7 fill-current" />
                </button>
              ))}
            </div>
            <input
              type="text"
              maxLength={100}
              placeholder="Give your review a title (optional)"
              className="input input-bordered w-full mb-2 hidden" // Ẩn vì chỉ cần comment
              disabled
            />
            <textarea
              className="textarea textarea-bordered w-full min-h-[80px] mb-2"
              placeholder="Start writing here..."
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              maxLength={1000}
              required
            />
            {error && <div className="text-red-500 text-sm mb-2">{error}</div>}
            <Button type="submit" disabled={isSubmitting} className="w-40 mt-2">
              {isSubmitting ? "Submitting..." : "Submit Review"}
            </Button>
          </div>
        </form>
      )}
      <div className="divide-y">
        {isLoading ? (
          <div className="text-center py-8">Loading reviews...</div>
        ) : reviews.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">
            No reviews yet.
          </div>
        ) : (
          reviews.map((review) => (
            <div
              key={review.id}
              className="flex flex-col md:flex-row items-start gap-4 py-6"
            >
              <div className="flex items-center gap-2 min-w-[120px]">
                <div className="rounded-full bg-gray-200 dark:bg-zinc-800 w-10 h-10 flex items-center justify-center">
                  <span className="font-bold text-lg text-gray-700 dark:text-zinc-200">
                    {review.accountName?.[0] || "?"}
                  </span>
                </div>
                <span className="font-medium">{review.accountName}</span>
              </div>
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  {Array.from({ length: 5 }).map((_, i) => (
                    <Star
                      key={i}
                      className={
                        i < review.rating
                          ? "h-4 w-4 text-yellow-400 fill-yellow-400"
                          : "h-4 w-4 text-gray-300 dark:text-zinc-700"
                      }
                    />
                  ))}
                </div>
                <div className="font-semibold mb-1">{review.comment}</div>
                <div className="text-xs text-muted-foreground">
                  {new Date(review.createdAt).toLocaleDateString()}
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </section>
  );
}
