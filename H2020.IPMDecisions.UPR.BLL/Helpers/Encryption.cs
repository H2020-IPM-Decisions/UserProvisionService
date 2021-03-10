using Microsoft.AspNetCore.DataProtection;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public class Encryption
    {
        private readonly IDataProtector _protector;

        public Encryption(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(nameof(Encryption));
        }

        public string Encrypt(string input)
        {
            return _protector.Protect(input);
        }

        public string Decrypt(string input)
        {
            return _protector.Unprotect(input);

        }
    }
}