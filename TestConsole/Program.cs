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

// Beautiful new DSL syntax!
var validationTemplate = ValidationTemplate<UserRegistrationRequest>.Create()
    .Field(r => r.Email, "Email")
        .Must(email => email.Contains('@'), "Must contain @ symbol")
        .Must(email => email.Length >= 5, "Email must be at least 5 characters")
    .Field(x => x.Password, "Password")
        .Must(p => p.Length >= 8, "Must be at least 8 characters")
        .Must(p => p.Any(Char.IsDigit), "Must contain at least one digit")
        .Must(p => p.Any(c => !Char.IsLetterOrDigit(c)), "Must contain a special character")
    .Field(x => x.ConfirmPassword, "Confirm Password")
        .Must(cp => cp == invalidRequest.Password, "Passwords do not match")
    .Field(x => x.FullName, "Full Name")
        .Must(fn => !String.IsNullOrWhiteSpace(fn), "Full name is required")
        .Must(fn => fn.Split(' ').Length >= 2, "Full name must contain at least two words")
    .Object()
        .Must(req => req.PhoneNumbers.Count != 0, "At least one phone number is required")
        .Must(req => req.PhoneNumbers.All(p => !String.IsNullOrWhiteSpace(p.Number)), "Phone number cannot be empty")
        .Must(req => req.PhoneNumbers.All(p => new[] { "mobile", "home", "work" }.Contains(p.Type)), "Invalid phone number type")
    .Build();

// Test the validation
var result = validationTemplate.Validate(invalidRequest);
Console.WriteLine($"Validation result: {(result.IsSuccess ? "SUCCESS" : "FAILED")}");

if (result.IsFailure)
{
    Console.WriteLine("Validation errors:");
    Console.WriteLine(result.Error!.Message);
}



public class UserRegistrationRequest
{
    public String Email { get; set; }
    public String Password { get; set; }
    public String ConfirmPassword { get; set; }
    public String FullName { get; set; }
    public Address Address { get; set; }
    public List<PhoneNumber> PhoneNumbers { get; set; }
}

public class Address
{
    public String Street { get; set; }
    public String City { get; set; }
    public String ZipCode { get; set; }
}

public class PhoneNumber
{
    public String Type { get; set; } // "mobile", "home", "work"
    public String Number { get; set; }
}
