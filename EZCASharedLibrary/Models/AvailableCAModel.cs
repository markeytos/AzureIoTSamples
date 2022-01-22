using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EZCASharedLibrary.Models
{
    public class AvailableCAModel
    {
        [JsonPropertyName("CAID")]
        public string? CAID { get; set; } 
        [JsonPropertyName("CAFriendlyName")]
        public string? CAFriendlyName { get; set; }
        [JsonPropertyName("CATemplateType")]
        public string? CATemplateType { get; set; } 
        [JsonPropertyName("TemplateID")]
        public string? TemplateID { get; set; } 

        [JsonPropertyName("KeyUsage")]
        public string? KeyUsage { get; set; }
        [JsonPropertyName("MaxCertLifeDays")]
        public int MaxCertLifeDays { get; set; }
        [JsonPropertyName("DomainRestrictions")]
        public string? DomainRestrictions { get; set; } //this is if all domains are allowed or not 

        [JsonPropertyName("CAKeyType")]
        public string? CAKeyType { get; set; }
        [JsonPropertyName("CAHashing")]
        public string? CAHashing { get; set; }
    }
}
