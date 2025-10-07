namespace ClassLibrary2;


public interface IUserInterface
{
    void ShowWelcomeMessage();
    void DisplayProducts(Product[] products);
    void DisplayMessage(string message);
    void DisplayError(string error);
    int GetCoinInput();
    string GetProductSelection();
    UserRole AuthenticateUser();
    void DisplayChange(Dictionary<decimal, int> change);
    void ShowAdminMenu();
    
}