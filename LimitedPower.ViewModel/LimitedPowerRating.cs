namespace LimitedPower.ViewModel
{
    public class LimitedPowerRating
    {
        public double Rating { get; set; }
        public string Description { get; set; }
        public RatingType RatingType { get; set; }
        public ReviewSource ReviewSource { get; set; }

        public LimitedPowerRating(double rating, string description, ReviewSource reviewSource, RatingType ratingType = RatingType.Main)
        {
            Rating = rating;
            Description = description;
            ReviewSource = reviewSource;
            RatingType = ratingType;
        }
    }

    public enum RatingType
    {
        Main,
        SideBoard,
        Lesson
    }

    public enum ReviewSource
    {
        // ReSharper disable InconsistentNaming
        DraftaholicsAnonymous,
        DraftSim,
        InfiniteMythicEditionJustLolaman,
        InfiniteMythicEditionM0bieus,
        InfiniteMythicEditionScottynada,
        Deathsie,
        Drifter,
        SeventeenLands,
        // ReSharper restore InconsistentNaming
    }
}