namespace InnoShop.UserManagement.Domain.UserAggregate;

public record RatingAverage(double Sum, int Count)
{
    public double Value => Count > 0 ? Sum / Count : 0;
    public int ReviewCount => Count;

    public RatingAverage Add(int newRating) =>
        new(Sum + newRating, Count + 1);
    public RatingAverage Delete(int oldRating) =>
        Count > 0
        ? new(Sum - oldRating, Count - 1) : this;
}

