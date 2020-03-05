using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Planet.MongoDbConsoleAppSample.Notifications;
using Planet.MongoDbConsoleAppSample.Notifications.Models;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Commands {
    public class BookmarkCreated : INotification {
        public string BookmarkId { get; set; }

        public class CreateImageAfterBookmarkCreatedHandler : INotificationHandler<BookmarkCreated> {
            private readonly INotificationService _notification;

            public CreateImageAfterBookmarkCreatedHandler (INotificationService notification) {
                _notification = notification;
            }

            public async Task Handle (BookmarkCreated notification, CancellationToken cancellationToken) {
                await _notification.SendAsync (new Message ());
            }
        }

        public class CreateVideoAfterBookmarkCreatedHandler : INotificationHandler<BookmarkCreated> {
            private readonly INotificationService _notification;

            public CreateVideoAfterBookmarkCreatedHandler (INotificationService notification) {
                _notification = notification;
            }

            public async Task Handle (BookmarkCreated notification, CancellationToken cancellationToken) {
                await _notification.SendAsync (new Message ());
            }
        }
    }
}