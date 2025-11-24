namespace InnoShop.UserManagement.Domain.UserAggregate;

public record RatingSummary(double TotalScore, int NumberOfReviews)
{
    public static RatingSummary Empty => new(0, 0);
    public double Average => NumberOfReviews > 0
        ? Math.Round(TotalScore / NumberOfReviews, 2)
        : 0;

    public RatingSummary AddRating(int rating) =>
        new(TotalScore + rating, NumberOfReviews + 1);
        
    public RatingSummary RemoveRating(int rating) =>
        NumberOfReviews > 0
            ? new(TotalScore - rating, NumberOfReviews - 1)
            : this;
}

