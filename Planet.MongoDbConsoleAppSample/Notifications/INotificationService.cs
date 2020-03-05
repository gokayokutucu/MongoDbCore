using System.Threading.Tasks;
using Planet.MongoDbConsoleAppSample.Notifications.Models;

namespace Planet.MongoDbConsoleAppSample.Notifications {
    public interface INotificationService {
        Task SendAsync (Message message);
    }
}