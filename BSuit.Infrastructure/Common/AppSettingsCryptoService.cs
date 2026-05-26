using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.Common
{
    /// <summary>
    /// using var scope = builder.Services.BuildServiceProvider().CreateScope();
    /// var crypto = scope.ServiceProvider.GetRequiredService<IAppSettingsCryptoService>();
    /// var conn = crypto.Decrypt(encrypted);
    /// </summary>
    public class AppSettingsCryptoService : IAppSettingsCryptoService
    {
        private readonly IDataProtector _protector;

        public AppSettingsCryptoService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("BSuit.AppSettings");
        }

        public string Encrypt(string plainText)
        {
            return _protector.Protect(plainText);
        }

        public string Decrypt(string cipherText)
        {
            return _protector.Unprotect(cipherText);
        }
    }
}
