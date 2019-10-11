using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Detector.EpisodeStorage.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detector.EpisodeStorage.ScreenShotDB
{
    public class RSAProvider
    {
        private readonly ILogger<RSAProvider> _logger;
        private readonly IOptions<Config> _config;
        private readonly RSACryptoServiceProvider _csp;
        private readonly X509Certificate2 _certificate;
        private bool _enabled;

        public RSAProvider(ILogger<RSAProvider> logger, IOptions<Config> config)
        {
            _logger = logger;
            _config = config;
            _certificate = new X509Certificate2(_config.Value.KeyPath);
            _csp = (RSACryptoServiceProvider) _certificate?.PrivateKey;
            _enabled = _config.Value.SignEnabled;
        }

        public byte[] Sign(string textToSign)
        {
            _logger.Log(LogLevel.Debug, $"Sign message with key '{_certificate.Subject}' ...");
            // Hash the data
            var data = new UnicodeEncoding().GetBytes(textToSign);

            // Sign the hash
            return _csp.SignData(data, CryptoConfig.MapNameToOID("SHA1"));
        }
        public byte[] Sign(byte [] dataToSign)
        {
            _logger.Log(LogLevel.Debug, $"Sign data with key '{_certificate.Subject}' ...");
            // Sign the hash
            return _csp.SignData(dataToSign, CryptoConfig.MapNameToOID("SHA1"));
        }
        public bool Verify(string text, byte[] signature)
        {
            _logger.Log(LogLevel.Debug, $"Verify sign with key '{_certificate.Subject}' ...");
            // Hash the data
            var data = new UnicodeEncoding().GetBytes(text);
            
            // Verify the signature with the hash
            return _csp.VerifyData(data, CryptoConfig.MapNameToOID("SHA1"), signature);
        }
        public bool Verify(byte[] data, byte[] signature)
        {
            _logger.Log(LogLevel.Debug, $"Verify sign with key '{_certificate.Subject}' ...");
            // Verify the signature with the hash
            return _csp.VerifyData(data, CryptoConfig.MapNameToOID("SHA1"), signature);
        }
    }
}
