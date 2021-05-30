namespace LimitedPower.Model
{
    public class LimitedPowerRating
    {
        public double Rating { get; set; }
        public string Description { get; set; }
        public ReviewContributor ReviewContributor { get; set; }

        public LimitedPowerRating(double rating, string description, ReviewContributor reviewContributor)
        {
            Rating = rating;
            Description = description;
            ReviewContributor = reviewContributor;
        }
    }

    public enum ReviewContributor
    {
        // ReSharper disable InconsistentNaming
        DraftaholicsAnonymous,
        DraftSim,
        InfiniteMythicEditionJustLolaman,
        InfiniteMythicEditionM0bieus,
        InfiniteMythicEditionScottynada,
        Deathsie,
        Drifter,
        Raszero,
        SeventeenLands,
        // ReSharper restore InconsistentNaming
    }
}