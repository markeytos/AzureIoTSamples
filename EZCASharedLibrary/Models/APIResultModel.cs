﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EZCASharedLibrary.Models
{
    public class APIResultModel
    {
        public APIResultModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
        public APIResultModel()
        {

        }
        public APIResultModel(bool success)
        {
            Success = success;
        }

        [JsonPropertyName("Success")]
        public bool Success { get; set; } = false;
        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;
    }
}
