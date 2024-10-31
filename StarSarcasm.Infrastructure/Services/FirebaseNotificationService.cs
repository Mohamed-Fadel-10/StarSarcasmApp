using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class FirebaseNotificationService
    {
        private readonly IWebHostEnvironment _env;

        public FirebaseNotificationService(IWebHostEnvironment env)
        {
            _env = env;

            if (FirebaseApp.DefaultInstance == null)
            {
                string pathToJson = Path.Combine(_env.ContentRootPath, "wwwroot", "hazzy-7afb2-firebase-adminsdk-xki91-b344c342c9.json");

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(pathToJson)
                });
            }
        }

        public async Task SendNotificationAsync(string deviceToken, string title, string body)
        {
            var message = new Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Successfully sent message: " + response);
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }
    }
}
