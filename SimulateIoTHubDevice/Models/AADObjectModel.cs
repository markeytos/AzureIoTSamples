using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimulateIoTHubDevice.Models
{
    public class AADObjectModel
    {
        public AADObjectModel()
        {

        }

        public AADObjectModel(string objectID, string name)
        {
            ObjectId = objectID;
            FriendlyName = name;
        }
        [JsonPropertyName("ObjectId")]
        public string ObjectId { get; set; } = string.Empty;
        [JsonPropertyName("FriendlyName")]
        public string FriendlyName { get; set; } = string.Empty;
        [JsonPropertyName("ObjectType")]
        public string ObjectType { get; set; } = "User";
        [JsonPropertyName("isValid")]
        public bool isValid { get; set; } = true;
    }
}
