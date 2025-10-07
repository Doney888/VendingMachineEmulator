namespace ClassLibrary2;

public interface IVendingMachine
{
    decimal Income {get; }
    ulong SerialNumber {get; init; }
    string Name {get; init; }
    decimal GetProfit();
}