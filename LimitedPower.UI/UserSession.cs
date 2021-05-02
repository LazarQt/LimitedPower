namespace LimitedPower.UI
{
    public static class Sets
    {
        public static string Stx = "stx";
        public static string Khm = "khm";
    }

    public class UserSession
    {
        public string SelectedSet { get; set; } = Sets.Stx;
        public bool LiveData { get; set; } = true;
    }
}
