using Microsoft.AspNetCore.Identity;

public class VietnameseIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Mật khẩu phải chứa ít nhất một chữ số ('0'-'9')."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Mật khẩu phải chứa ít nhất một chữ thường ('a'-'z')."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Mật khẩu phải chứa ít nhất một chữ hoa ('A'-'Z')."
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Mật khẩu phải chứa ít nhất một ký tự đặc biệt."
        };
    }
}
