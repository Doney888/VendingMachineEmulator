namespace ClassLibrary2;

public class Program
{
    static void Main(string[] args)
    {
        var vendingMachine = new VendingMachine(1337, "Charles");
        var consoleInetface = new ConsoleInterface(vendingMachine);

        consoleInetface.Run();
    }
}