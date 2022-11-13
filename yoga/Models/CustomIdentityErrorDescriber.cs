using Microsoft.AspNetCore.Identity;

namespace yoga.Models
{
    public class CustomIdentityErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Password must have at least one special character(!@#$..etc)"
            };
        }
    }
}