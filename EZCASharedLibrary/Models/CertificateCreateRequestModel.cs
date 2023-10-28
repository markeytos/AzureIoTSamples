using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EZCASharedLibrary.Models
{
    public class CertificateCreateRequestModel
    {
        public CertificateCreateRequestModel(AvailableCAModel ca, string subjectName, 
            List<string> subjectAltNames, string csr, int lifetime)
        {
            SubjectName = subjectName;
            SubjectAltNames = subjectAltNames;
            CAID = ca.CAID;
            TemplateID = ca.TemplateID;
            CSR = csr;
            ValidityInDays = lifetime;
        }

        [JsonPropertyName("SubjectName")]
        public string SubjectName { get; set; } = string.Empty;
        [JsonPropertyName("SubjectAltNames")]
        public List<string> SubjectAltNames { get; set; } = new List<string>();
        [JsonPropertyName("CAID")]
        public string? CAID { get; set; } 
        [JsonPropertyName("TemplateID")]
        public string? TemplateID { get; set; }
        [JsonPropertyName("CSR")]
        public string CSR { get; set; } = string.Empty;
        [JsonPropertyName("ValidityInDays")]
        public int ValidityInDays { get; set; } 
        [JsonPropertyName("EKUs")]
        public string[] EKUs { get; set; } = new string[] { "1.3.6.1.5.5.7.3.1", "1.3.6.1.5.5.7.3.2" };
        [JsonPropertyName("KeyUsages")]
        public string[] KeyUsages { get; set; } = new string[] { "Key Encipherment", "Digital Signature" };
        //CertLocation
        [JsonPropertyName("SelectedLocation")]
        public string SelectedLocation { get; set; } = "IoT Device"; // local, AKV, KMS, ETC
        [JsonPropertyName("ResourceID")]
        public string ResourceID { get; set; } = string.Empty;
        [JsonPropertyName("SecretName")]
        public string SecretName { get; set; } = string.Empty;
        [JsonPropertyName("AKVName")]
        public string AKVName { get; set; } = string.Empty;
        [JsonPropertyName("AutoRenew")]
        public bool AutoRenew { get; set; } = false;
        [JsonPropertyName("AutoRenewPercentage")]
        public int AutoRenewPercentage { get; set; } = 80;
    }
}
