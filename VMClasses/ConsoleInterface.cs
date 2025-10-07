namespace ClassLibrary2;

public class ConsoleInterface(VendingMachine vendingMachine) : IUserInterface
{
    
    private readonly VendingMachine _vendingMachine = vendingMachine;
    
    public void Run()
    {
        while (true)
        {
            try
            {
                ShowWelcomeMessage();
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RunCustomerMode();
                        break;
                    case "2":
                        RunAdminMode();
                        break;
                    case "0":
                        DisplayMessage("До свидания!");
                        return;
                    default:
                        DisplayError("Неверный выбор");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                DisplayError($"Произошла ошибка: {ex.Message}");
            }
        }
    } 
    
    private void RunCustomerMode()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== РЕЖИМ ПОКУПАТЕЛЯ ===");
            Console.WriteLine($"Текущий баланс: {_vendingMachine.CurrentBalance:C}");
            Console.WriteLine("\n1. Посмотреть товары");
            Console.WriteLine("2. Вставить монету");
            Console.WriteLine("3. Выбрать товар");
            Console.WriteLine("4. Вернуть деньги");
            Console.WriteLine("0. Назад");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayProducts(_vendingMachine.GetAllProducts());
                    break;
                case "2":
                    var coin = GetCoinInput();
                    if (coin > 0)
                    {
                        var newBalance = _vendingMachine.InsertCoin(coin);
                        DisplayMessage($"Баланс пополнен. Текущий баланс: {newBalance:C}");
                    }
                    break;
                case "3":
                    DisplayProducts(_vendingMachine.GetAllProducts());
                    var productId = GetProductSelection();
                    if (int.TryParse(productId, out int id) && id > 0)
                    {
                        var (success, product, error) = _vendingMachine.PurchaseProduct(id - 1);
                        if (success)
                        {
                            DisplayMessage($"Товар '{product.Name}' выдан!");
                            if (_vendingMachine.CurrentBalance > 0)
                            {
                                var change = _vendingMachine.ReturnChange();
                                DisplayChange(change);
                            }
                        }
                        else
                        {
                            DisplayError(error);
                        }
                    }
                    break;
                case "4":
                    if (_vendingMachine.CurrentBalance > 0)
                    {
                        var change = _vendingMachine.ReturnChange();
                        DisplayChange(change);
                        DisplayMessage("Деньги возвращены");
                    }
                    else
                    {
                        DisplayMessage("Нет денег для возврата");
                    }
                    break;
                case "0":
                    if (_vendingMachine.CurrentBalance > 0)
                    {
                        var change = _vendingMachine.ReturnChange();
                        DisplayChange(change);
                    }
                    return;
                default:
                    DisplayError("Неверный выбор");
                    break;
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    private void RunAdminMode()
    {
        var role = AuthenticateUser();
        
        if (role != UserRole.Administrator)
        {
            DisplayError("Неверный пароль администратора");
            return;
        }

        while (true)
        {
            Console.Clear();
            ShowAdminMenu();
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Просмотр товаров
                    DisplayProducts(_vendingMachine.GetAllProducts());
                    break;
                case "2":
                    // Пополнение существующих товаров
                    RestockExistingProducts();
                    break;
                case "3":
                    // Добавление нового товара
                    AddNewProduct();
                    break;
                case "4":
                    // Сбор выручки
                    var revenue = _vendingMachine.CollectRevenue();
                    DisplayMessage($"Выручка собрана: {revenue:C}");
                    break;
                case "5":
                    // Статистика
                    var (totalRevenue, totalSales) = _vendingMachine.GetStatistics();
                    DisplayMessage($"Общая выручка: {totalRevenue:C}");
                    DisplayMessage($"Продано товаров: {totalSales}");
                    DisplayMessage($"Свободных слотов: {_vendingMachine.GetAvailableSlots()}");
                    break;
                case "0":
                    return;
                default:
                    DisplayError("Неверный выбор");
                    break;
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    private void RestockExistingProducts()
    {
        DisplayProducts(_vendingMachine.GetAllProducts());
        var productId = GetProductSelection();
        if (int.TryParse(productId, out int id) && id > 0)
        {
            Console.Write("Введите количество для пополнения: ");
            if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
            {
                var success = _vendingMachine.RestockProduct(id - 1, quantity);
                if (success)
                    DisplayMessage($"Товар пополнен на {quantity} единиц");
                else
                    DisplayError("Ошибка при пополнении товара");
            }
            else
            {
                DisplayError("Неверное количество");
            }
        }
    }

    private void AddNewProduct()
    {
        Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО ТОВАРА ===");
    
        Console.Write("Введите название товара: ");
        var name = Console.ReadLine();
    
        if (string.IsNullOrWhiteSpace(name))
        {
            DisplayError("Название товара не может быть пустым");
            return;
        }

        Console.Write("Введите цену товара: ");
        if (!int.TryParse(Console.ReadLine(), out int price) || price <= 0)
        {
            DisplayError("Неверный формат цены");
            return;
        }

        Console.Write("Введите начальное количество (по умолчанию 1): ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            quantity = 1; 
        }

        var success = _vendingMachine.AddProduct(name, price, quantity);
        if (success)
            DisplayMessage($"Товар '{name}' успешно добавлен!");
        else
            DisplayError("Не удалось добавить товар. Возможно, достигнут лимит товаров.");
    }
    
    public void ShowWelcomeMessage()
    {
        Console.Clear();
        Console.WriteLine("=== ВЕНДИНГОВЫЙ АВТОМАТ ===");
        Console.WriteLine("1. Покупка товаров");
        Console.WriteLine("2. Режим администратора");
        Console.WriteLine("0. Выход");
    }

    public void DisplayProducts(Product[] products)
    {
        if (_vendingMachine.NumberOfProducts() == 0)
        {
            Console.WriteLine("Сейчас товаров нет:(");
            return;
        }
        Console.WriteLine("\n=== ДОСТУПНЫЕ ТОВАРЫ ===");
        for (int Id = 0; Id < _vendingMachine.NumberOfProducts(); Id++)
        {
            Product product = _vendingMachine.GetProduct(Id);
            Console.WriteLine($"{Id + 1}. {product.Name} - {product.Price:C} (осталось: {_vendingMachine.GetCountOfProducts(product)})");
        }
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine($"💡 {message}");
    }

    public void DisplayError(string error)
    {
        Console.WriteLine($"❌ Ошибка: {error}");
    }

    public int GetCoinInput()
    {
        Console.WriteLine($"Внимание! Автомат принимает только следующие номиналы: {CurrencyValue.AllToString}");
        Console.Write("Введите номинал монеты (0 для отмены): ");
        if (int.TryParse(Console.ReadLine(), out var coin) && CurrencyValue.IsValid(coin))
            return coin;
        
        DisplayError("Неверный формат монеты");
        return 0;
    }

    public string GetProductSelection()
    {
        Console.Write("Выберите товар (ID): ");
        return Console.ReadLine();
    }

    public UserRole AuthenticateUser()
    {
        Console.Write("Введите пароль (Enter для покупателя): ");
        var password = Console.ReadLine();
        
        return password switch
        {
            "admin" => UserRole.Administrator,
            _ => UserRole.Customer
        };
    }

    public void DisplayChange(Dictionary<decimal, int> change)
    {
        if (change == null || !change.Any())
        {
            Console.WriteLine("Сдача: нет");
            return;
        }

        Console.WriteLine("Ваша сдача:");
        foreach (var coin in change.OrderByDescending(x => x.Key))
        {
            Console.WriteLine($"  {coin.Key:C} × {coin.Value}");
        }
    }

    public void ShowAdminMenu()
    {
        Console.WriteLine("\n=== АДМИНИСТРАТОР ===");
        Console.WriteLine("1. Просмотреть товары");
        Console.WriteLine("2. Пополнить существующие товары");
        Console.WriteLine("3. Добавить новый товар");
        Console.WriteLine("4. Собрать выручку");
        Console.WriteLine("5. Просмотреть статистику");
        Console.WriteLine("0. Назад");
    }
    
}