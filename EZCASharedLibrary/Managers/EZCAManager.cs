using EZCASharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Azure.Core;
using Azure.Identity;
using EZCASharedLibrary.Models;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace EZCASharedLibrary.Managers;

public class EZCAManager
{
    private readonly IHttpService _httpService;
    private AccessToken? _token;
    private readonly string _portalURL;
    public EZCAManager(IHttpService httpService, string url = "https://portal.ezca.io/")
    {
        _httpService = httpService;
        _token = null;
        _portalURL = url;
    }


    private async Task<string> GetTokenAsync()
    {
        if (_token == null || _token.Value.ExpiresOn < DateTime.UtcNow)
        {
            //Create Azure Credential 
            //This example includes Interactive for development purposes. 
            //For production you should remove interactive since there is no human involved
            DefaultAzureCredential credential = new(includeInteractiveCredentials: true);
            TokenRequestContext authContext = new(
                new[] { "https://management.core.windows.net/.default" });
            _token = await credential.GetTokenAsync(authContext);
        }
        return _token.Value.Token;
    }

    public async Task<AvailableCAModel[]?> GetAvailableCAsAsync()
    {
        AvailableCAModel[]? availableCAs = null;
        try
        {
            HttpResponseMessage response = await
                _httpService.GetAPIAsync($"{_portalURL}api/CA/GetAvailableSSLCAs",
                    await GetTokenAsync());
            if (response.IsSuccessStatusCode)
            {
                availableCAs = JsonSerializer.Deserialize
                    <AvailableCAModel[]>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine($"Error contacting server: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting available CAs: {ex.Message}");
        }
        return availableCAs;
    }

    public async Task<X509Certificate2?> RequestCertificateAsync(AvailableCAModel ca, 
        string domain, int certificateValidityDays = 10)
    {
        if(ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if(string.IsNullOrWhiteSpace(domain))
        {
            throw new ArgumentNullException(nameof(domain));
        }
        //create a 4096 RSA key
        RSA key = RSA.Create(4096);

        //create Certificate Signing Request 
        X500DistinguishedName x500DistinguishedName = new("CN=" + domain);
        CertificateRequest certificateRequest = new(x500DistinguishedName, key,
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        string csr = PemEncodeSigningRequest(certificateRequest);

        List<string> subjectAlternateNames = new()
        {
            domain
        };
        CertificateCreateRequestModel request = new(ca, "CN=" + domain,
            subjectAlternateNames, csr, certificateValidityDays);
        try
        {
            //Request Certificate from EZCA
            HttpResponseMessage response = await
                _httpService.PostAPIAsync($"{_portalURL}api/CA/RequestSSLCertificate",
                    JsonSerializer.Serialize(request), await GetTokenAsync());
            if (response.IsSuccessStatusCode)
            {
                APIResultModel? result = JsonSerializer.Deserialize
                    <APIResultModel>(await response.Content.ReadAsStringAsync());
                if (result != null)
                {
                    if(result.Success)
                    {
                        X509Certificate2 certificate = ImportCertFromPEMString(result.Message);
                        return certificate.CopyWithPrivateKey(key);
                    }
                    Console.WriteLine(result.Message);
                }
            }
            else
            {
                Console.WriteLine($"Error contacting server: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering device in EZCA: {ex.Message}");
        }
        return null;
    }

    public X509Certificate2 ImportCertFromPEMString(string pemCert)
    {
        pemCert = Regex.Replace(pemCert, @"-----[a-z A-Z]+-----", "").Trim();
        return new X509Certificate2(Convert.FromBase64String(pemCert));
    }

    public static string PemEncodeSigningRequest(CertificateRequest request)
    {
        byte[] pkcs10 = request.CreateSigningRequest();
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("-----BEGIN CERTIFICATE REQUEST-----");
        builder.AppendLine(Convert.ToBase64String(pkcs10, Base64FormattingOptions.InsertLineBreaks));
        builder.AppendLine("-----END CERTIFICATE REQUEST-----");
        return builder.ToString();
    }

    public async Task<bool> RegisterDomainAsync(AvailableCAModel ca, string domain)
    {
        if (ca == null)
        {
            throw new ArgumentNullException(nameof(ca));
        }
        if (string.IsNullOrWhiteSpace(domain))
        {
            throw new ArgumentNullException(nameof(domain));
        }
        await GetTokenAsync();
        AADObjectModel requester = await UserFromTokenAsync();
        //For this example we are making the requester the owner and requester of the domain.
        //In Production you should change this to meet your security requirements. 
        List<AADObjectModel> ownersAndRequesters = new ()
        {
            requester
        };
        NewDomainRegistrationRequest request = new(ca, domain, ownersAndRequesters, ownersAndRequesters);
        try
        {
            HttpResponseMessage response = await
                _httpService.PostAPIAsync($"{_portalURL}api/CA/RegisterNewDomain",
                    JsonSerializer.Serialize(request), await GetTokenAsync());
            if (response.IsSuccessStatusCode)
            {
                APIResultModel? result = JsonSerializer.Deserialize
                    <APIResultModel>(await response.Content.ReadAsStringAsync());
                if (result != null)
                {
                    Console.WriteLine(result.Message);
                    return result.Success;
                }
            }
            else
            {
                Console.WriteLine($"Error contacting server: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering device in EZCA: {ex.Message}");
        }
        return false;
    }

    private async Task<AADObjectModel> UserFromTokenAsync()
    {
        string stream = await GetTokenAsync();
        JwtSecurityTokenHandler handler = new ();
        var jsonToken = handler.ReadToken(stream);
        JwtSecurityToken tokenS = (JwtSecurityToken)jsonToken;
        string objectID = (string)tokenS.Payload.FirstOrDefault(i => i.Key == "oid").Value;
        string upn = (string)tokenS.Payload.FirstOrDefault(i => i.Key == "upn").Value;
        return new(objectID, upn);
    }

}