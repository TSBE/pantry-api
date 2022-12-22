using System.Security.Cryptography.X509Certificates;

namespace Pantry.Common.Authentication;

internal static class CertificatesHelper
{
    public static IEnumerable<X509Certificate2> GetCertificates(JwtTokenSettings settings)
    {
        foreach (var certificate in GetCertificatesCollection(settings))
        {
            yield return certificate;
        }
    }

    private static X509Certificate2Collection GetCertificatesCollection(JwtTokenSettings settings)
    {
        if (string.IsNullOrEmpty(settings.Base64EncodedPfx))
        {
            throw new ArgumentException("The certificate cannot be null or an empty string.");
        }

        return GetCertificatesCollection(settings.Base64EncodedPfx, settings.PasswordPfx);
    }

    private static X509Certificate2Collection GetCertificatesCollection(string certificate, string? password)
    {
        var collection = new X509Certificate2Collection();
        byte[] certificateRawContent = Convert.FromBase64String(certificate);

        if (!string.IsNullOrEmpty(password))
        {
            collection.Import(
                certificateRawContent,
                password,
                X509KeyStorageFlags.PersistKeySet);
        }
        else
        {
            collection.Import(certificateRawContent);
        }

        return collection;
    }
}
