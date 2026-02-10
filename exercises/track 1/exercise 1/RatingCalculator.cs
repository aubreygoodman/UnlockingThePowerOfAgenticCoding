using System;
using System.Collections.Generic;

public class Review
{
    public int Rating { get; set; }    // 1-5 stars
    public bool Verified { get; set; }
}

public class RatingCalculator
{
    /// <summary>
    /// Calculate the average star rating from a list of reviews.
    /// Returns the average rounded to one decimal place.
    /// Returns 0 if there are no reviews.
    /// </summary>
    public static double CalculateAverageRating(List<Review> reviews)
    {
        if (reviews == null || reviews.Count == 0)
            return 0;

        int total = 0;
        int count = 0;
        foreach (var review in reviews)
        {
            if (review == null)
                continue;
            total += review.Rating;
            count++;
        }

        if (count == 0)
            return 0;

        double average = (double)total / count;
        return Math.Round(average, 1);
    }
}

// These should all work correctly:
public class Program
{
    public static void Main()
    {
        // Expected: 0
        Console.WriteLine(RatingCalculator.CalculateAverageRating(new List<Review>()));

        // Expected: 5.0
        Console.WriteLine(RatingCalculator.CalculateAverageRating(new List<Review>
        {
            new Review { Rating = 5, Verified = true }
        }));

        // Expected: 4.0
        Console.WriteLine(RatingCalculator.CalculateAverageRating(new List<Review>
        {
            new Review { Rating = 4, Verified = true },
            new Review { Rating = 3, Verified = true },
            new Review { Rating = 5, Verified = false },
        }));
    }
}