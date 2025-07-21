using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.Outcomes.Validation;

public class User
{
    public String? Email { get; set; }
    public Int32 Age { get; set; }
    public String? Name { get; set; }
}

public static class ValidatorExample
{
    public static Result<Unit, Error> ValidateUser(User user)
    {
        return Validator<User>.For(user)
            .Rule(x => x.Email).NotNull().Email()
            .Rule(x => x.Age).GreaterThan(18)
            .Rule(x => x.Name).NotEmpty().MaxLength(50)
            .Validate();
    }

    public static Result<Unit, Error[]> ValidateUserWithAllErrors(User user)
    {
        return Validator<User>.For(user)
            .Rule(x => x.Email).NotNull().Email()
            .Rule(x => x.Age).GreaterThan(18)
            .Rule(x => x.Name).NotEmpty().MaxLength(50)
            .ValidateAll();
    }

    public static Result<Unit, Error> ValidateUserWithCustomErrors(User user)
    {
        return Validator<User>.For(user)
            .Rule(x => x.Email)
                .NotNull("Email address is required")
                .Email("Please provide a valid email address")
            .Rule(x => x.Age)
                .GreaterThan(18, "User must be an adult")
            .Rule(x => x.Name)
                .NotEmpty("Name is required")
                .MaxLength(50, "Name is too long")
            .Validate();
    }

    public static Result<Unit, Error> ValidateUserWithCustomErrorObjects(User user)
    {
        return Validator<User>.For(user)
            .Rule(x => x.Email)
                .Must(email => email != null, Error.Validation("Email is required"))
                .Must(email => email?.Contains("@") == true, Error.Create("BUSINESS_RULE", "Email must contain @ symbol"))
            .Rule(x => x.Age)
                .Must(age => age >= 18, Error.Create("AGE_RESTRICTION", "Must be 18 or older"))
            .Validate();
    }
}