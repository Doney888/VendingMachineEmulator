public static class CurrencyValue
{
    public const int One = 1;
    public const int Two = 2;
    public const int Five = 5;
    public const int Ten = 10;
    public const int Fifty = 50;
    public const int OneHundred = 100;
    public const int TwoHundred = 200;
    public const int FiveHundred = 500;
    public const int OneThousand = 1000;
    
    public static readonly int[] All = 
    {
        OneThousand, FiveHundred, TwoHundred, OneHundred, 
        Fifty, Ten, Five, Two, One
    };

    public static readonly string AllToString = "1, 2, 5, 10, 50, 100, 200, 500, 1000";
    
    public static readonly int[] RevAll = All.Reverse().ToArray();
    
    public static bool IsValid(int denomination)
    {
        return All.Contains(denomination);
    }
}