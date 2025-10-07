namespace ClassLibrary2;

public class VendingMachine(ulong serialnumber, string name) : IVendingMachine
{
    public decimal Income { get; private set; } = 0;
    private decimal Outcome { get; set; } = 0;
    public ulong SerialNumber { get; init; } = serialnumber;
    public string Name { get; init; } = name;
    public decimal CurrentBalance { get; private set; } = 0;
    
    private const int _maxNumber = 20;
    private Product[] _products = new Product[_maxNumber];
    private int _lastNumber = -1;
    private Dictionary<Product, int> _productsCnt = new Dictionary<Product, int>();

    public int GetAvailableSlots() => _maxNumber - _lastNumber - 1;

    public decimal GetProfit()
    {
        decimal res = Income - Outcome;
        Income = 0;
        Outcome = 0;
        return res;
    }

    public int NumberOfProducts() => _lastNumber + 1;
    public Product GetProduct(int num) => _products[num];
    public Product[] GetAllProducts() => _products;
    public int GetCountOfProduct(int num) => _productsCnt[_products[num]];
    public int GetCountOfProducts(Product product) => _productsCnt[product];

    public void RemoveProduct(int num)
    {
        Product product = _products[num];
        if (_productsCnt[product] == 1)
        {
            _productsCnt.Remove(product);
            _lastNumber--;
        }
        else
        {
            _productsCnt[product]--;
        }
    }
    
    public void addMoney(decimal money) => Income += money;
    
    public Dictionary<int, int> giveChange(int amount)
    {
        Outcome += amount;
        return VendingMachineChangeGiver.CalculateChange(amount);
    }
    
    public decimal InsertCoin(decimal coin)
    {
        CurrentBalance += coin;
        addMoney(coin);
        return CurrentBalance;
    }
    
    public (bool success, Product product, string error) PurchaseProduct(int productId)
    {
        if (productId < 0 || productId > _lastNumber)
            return (false, null, "Неверный ID товара");
            
        var product = _products[productId];
        if (product == null)
            return (false, null, "Товар не найден");
            
        if (!_productsCnt.ContainsKey(product) || _productsCnt[product] <= 0)
            return (false, null, "Товар закончился");
            
        if (CurrentBalance < product.Price)
            return (false, null, "Недостаточно средств");
        
        CurrentBalance -= product.Price;
        RemoveProduct(productId);
        
        return (true, product, null);
    }
    
    public Dictionary<decimal, int> ReturnChange()
    {
        var change = giveChange((int)CurrentBalance);
        CurrentBalance = 0;
        return change.ToDictionary(kvp => (decimal)kvp.Key, kvp => kvp.Value);
    }
    
    public decimal CollectRevenue() => GetProfit();
    
    public (decimal totalRevenue, int totalSales) GetStatistics()
    {
        return (Income, _productsCnt.Values.Sum());
    }
    
    public bool AddProduct(string name, int price, int initialQuantity = 1)
    {
        if (_lastNumber >= _maxNumber - 1)
            return false;

        var newProduct = new Product(name, price);
        _products[++_lastNumber] = newProduct;
        _productsCnt[newProduct] = initialQuantity;
        return true;
    }
    
    public bool RestockProduct(int productId, int quantity)
    {
        if (productId < 0 || productId > _lastNumber)
            return false;
            
        var product = _products[productId];
        if (product == null)
            return false;
            
        _productsCnt[product] += quantity;
        return true;
    }
}