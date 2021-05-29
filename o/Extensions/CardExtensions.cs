using LimitedPower.Model;

namespace LimitedPower.UI.Extensions
{
    public static class CardExtensions
    {
        public static string Gn(this ColorWheel c)
        {
            return c switch
            {
                ColorWheel.White | ColorWheel.Red => "Lorehold",
                ColorWheel.Blue | ColorWheel.Red => "Prismari",
                ColorWheel.Green | ColorWheel.Blue => "Quandrix",
                ColorWheel.White | ColorWheel.Black => "Silverquill",
                ColorWheel.Black | ColorWheel.Green => "Witherbloom",
                _ => c.ToString()
            };
        }
    }
}
