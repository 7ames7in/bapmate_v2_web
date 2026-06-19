using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BapMate.Infrastructure.Services
{
    public class SmsService
    {
        private readonly string _userId;
        private readonly string _apiKey;
        private readonly string _sender;
        private readonly string _apiUrl;

        public SmsService(string userId, string apiKey, string sender, string apiUrl)
        {
            _userId = userId;
            _apiKey = apiKey;
            _sender = sender;
            _apiUrl = apiUrl;
        }

        public SmsService(IConfiguration configuration)
        {
            _userId = configuration["SmsSettings:UserId"] ?? "";
            _apiKey = configuration["SmsSettings:ApiKey"] ?? "";
            _sender = configuration["SmsSettings:Sender"] ?? "";
            _apiUrl = configuration["SmsSettings:ApiUrl"] ?? "https://apis.aligo.in/send/";
        }

        public async Task<string> SendSmsAsync(string receiver, string message, string title = "")
        {
            using (var client = new HttpClient())
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(_userId), "user_id");
                formData.Add(new StringContent(_apiKey), "key");
                formData.Add(new StringContent(message), "msg");
                formData.Add(new StringContent(receiver), "receiver");
                formData.Add(new StringContent(_sender), "sender");
                formData.Add(new StringContent(title), "title");
                formData.Add(new StringContent("SMS"), "msg_type");

                client.DefaultRequestHeaders.Add("Accept", "*/*");

                var response = await client.PostAsync(_apiUrl, formData);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}
