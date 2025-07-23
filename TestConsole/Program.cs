using Tsikhanau.Packages.ValueObjects.String;
using Tsikhanau.Validation;

var invalidRequest = new UserRegistrationRequest
{
    Email = "invalid-email",                    // нет @
    Password = "123",                           // короткий
    ConfirmPassword = "321",                    // не совпадает
    FullName = "John",                          // одно слово
    Address = new Address
    {
        Street = "",                            // пусто
        City = " ",                             // только пробел
        ZipCode = null                          // null
    },
    PhoneNumbers = new List<PhoneNumber>
    {
        new PhoneNumber { Type = "pager", Number = "123" }, // неправильный тип
        new PhoneNumber { Type = "mobile", Number = "" }    // пустой номер
    }
};


var validationTemplateBuilder = ValidationTemplateBuilder<UserRegistrationRequest>.New()
    .ForObject(invalidRequest)
    .ForField(r => r.Email)
        .WithFieldName("Email")
        .AddRule(email => email == "john.doe@example.com", "Incorrect email address")
        .Build()
    .ForField(x => x.Password)
        .WithFieldName("Password")
        .AddRule(p => p.Length >= 8, "Must be at least 8 characters")
        .AddRule(p => p.Any(char.IsDigit), "Must contain at least one digit")
        .AddRule(p => p.Any(c => !char.IsLetterOrDigit(c)), "Must contain a special character")
        .Build()
    .ForField(x => x.ConfirmPassword)
        .WithFieldName("ConfirmPassword")
        .AddRule(cp => cp == invalidRequest.Password, "Passwords do not match")
        .Build()
    .ForField(x => x.FullName)
        .WithFieldName("Full Name")
        .AddRule(fn => fn.Split(' ').Length >= 2, "Full name must contain at least two words")
        .Build()
    .ForField(x => x.Address)
        .WithFieldName("Address")
        .AddRule(a => a != null, "Address is required")
        .Build()
    .AddObjectRule(req =>
        req.PhoneNumbers != null && req.PhoneNumbers.Any(), "At least one phone number is required")
    .AddObjectRule(req =>
        req.PhoneNumbers.All(p => !string.IsNullOrWhiteSpace(p.Number)), "Phone number cannot be empty")
    .AddObjectRule(req =>
        req.PhoneNumbers.All(p => new[] { "mobile", "home", "work" }.Contains(p.Type)), "Invalid phone number type");



public class UserRegistrationRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string FullName { get; set; }
    public Address Address { get; set; }
    public List<PhoneNumber> PhoneNumbers { get; set; }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}

public class PhoneNumber
{
    public string Type { get; set; } // "mobile", "home", "work"
    public string Number { get; set; }
}
