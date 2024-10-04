using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading.Tasks;

namespace StarSarcasm.Infrastructure.Services
{
    public class FirebaseNotificationService
    {
        public FirebaseNotificationService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                string basePath = Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
                string pathToServiceAccountKey = Path.Combine(basePath, "StarSarcasm.Infrastructure", "Notifications", "starsarcasm-1ca16-firebase-adminsdk-y3bpu-cc2b9f8976.json");

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(pathToServiceAccountKey),
                });
            }
        }


        public async Task SendNotification(string token, string title, string body)
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
