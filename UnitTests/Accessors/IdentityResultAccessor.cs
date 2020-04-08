using Microsoft.AspNetCore.Identity;

namespace UnitTests.Accessors
{
    public class IdentityResultAccessor : IdentityResult
    {
        
        public IdentityResult Value { get; }
        
        public IdentityResultAccessor(bool succeeded)
        {
            if (succeeded)
            {
                Value = new IdentityResult();
                {
                    Succeeded = true;
                }
            }
        }
    }
}