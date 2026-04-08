public class Address
{
    private string _street;
    private string _city;
    private string _state;
    private string _country;

    public Address(string street, string city, string state, string country)
    {
        _street = street;
        _city = city;
        _state = state;
        _country = country;
    }

    public string GetStreet() => _street;
    public string GetCity() => _city;
    public string GetState() => _state;
    public string GetCountry() => _country;

    public bool IsInUSA() => _country.ToLower().Contains("usa") || _country.ToLower().Contains("united states");

    public string GetFullAddress() => $"{_street}\n{_city}, {_state}\n{_country}";
}