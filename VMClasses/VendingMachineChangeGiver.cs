namespace ClassLibrary2;

public static class VendingMachineChangeGiver
{
    public static Dictionary<int, int> CalculateChange(int amount)
    {
        int[] denominations = CurrencyValue.RevAll;
        var change = new Dictionary<int, int>();
        int remaining = amount;
    
        foreach (int denom in denominations.OrderByDescending(d => d))
        {
            if (remaining <= 0) break;
        
            int count = remaining / denom;
            if (count > 0)
            {
                change[denom] = count;
                remaining -= count * denom;
            }
        }
    
        return change;
    }
}