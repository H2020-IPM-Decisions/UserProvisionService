using Microsoft.AspNetCore.DataProtection;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public class EncryptionHelper
    {
        private readonly IDataProtector _protector;

        public EncryptionHelper(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(nameof(EncryptionHelper));
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