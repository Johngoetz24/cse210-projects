



public class Order
{
    private List<Product> _products = new List<Product>();
    private Customer _customer;

    public Order(Customer customer)
    {
        _customer = customer;
    }

    public void AddProduct(Product product) => _products.Add(product);

    public List<Product> GetProducts() => _products;
    public Customer GetCustomer() => _customer;

    public double CalculateTotalCost()
    {
        double total = 0;
        foreach (Product p in _products)
            total += p.GetTotalCost();

        total += _customer.LivesInUSA() ? 5 : 35;
        return total;
    }

    public string GetPackingLabel()
    {
        string label = "Packing Label:\n";
        foreach (Product p in _products)
            label += $"{p.GetName()} (ID: {p.GetProductId()})\n";
        return label;
    }

    public string GetShippingLabel()
    {
        return $"Shipping Label:\n{_customer.GetName()}\n{_customer.GetAddress().GetFullAddress()}";
    }
}