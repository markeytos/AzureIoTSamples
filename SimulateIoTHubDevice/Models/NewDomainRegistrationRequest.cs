using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimulateIoTHubDevice.Models
{
    public class NewDomainRegistrationRequest
    {
        public NewDomainRegistrationRequest()
        {

        }

        public NewDomainRegistrationRequest(AvailableCAModel ca, 
            string domain, List<AADObjectModel> owners, List<AADObjectModel> requesters)
        {
            CAID = ca.CAID;
            TemplateID = ca.TemplateID;
            Domain = domain;
            Owners = owners;
            Requesters = requesters;
        }

        [JsonPropertyName("CAID")]
        public string? CAID { get; set; } 

        [JsonPropertyName("TemplateID")]
        public string? TemplateID { get; set; } 
        [JsonPropertyName("Domain")]
        public string? Domain { get; set; } 
        [JsonPropertyName("Owners")]
        public List<AADObjectModel> Owners { get; set; } = new List<AADObjectModel>();
        [JsonPropertyName("Requesters")]
        public List<AADObjectModel> Requesters { get; set; } = new List<AADObjectModel>();
    }
}
