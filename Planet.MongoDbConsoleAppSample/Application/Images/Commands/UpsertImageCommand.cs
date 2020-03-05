using System.Collections.Generic;
using MediatR;

namespace Planet.MongoDbConsoleAppSample.Application.Images.Commands {
    public class UpsertImageCommand : IRequest<string> {
        public string Id { get; set; }
        public string Title { get; private set; }
        public string Source { get; private set; }
        public string BookmarkId { get; set; }

        public List<OwnerVoDTO> Owners { get; set; }

        public class OwnerVoDTO {
            public string Name { get; set; }
            public string Url { get; set; }
        }
    }
}