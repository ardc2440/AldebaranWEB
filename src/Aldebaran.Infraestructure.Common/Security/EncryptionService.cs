using Aldebaran.Infraestructure.Common.Security;
using Microsoft.AspNetCore.DataProtection;

namespace Aldebaran.Infrastructure.Common.Security
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IDataProtector _protector;

        public EncryptionService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("InventoryMinimumAlert");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return string.Empty;

            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return string.Empty;

            return _protector.Unprotect(cipherText);
        }
    }
}
