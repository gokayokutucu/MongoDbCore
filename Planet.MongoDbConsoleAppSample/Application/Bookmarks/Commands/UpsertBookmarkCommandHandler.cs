using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbConsoleAppSample.Repositories;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Commands {
    public class UpsertBookmarkCommandHandler : IRequestHandler<UpsertBookmarkCommand, string> {
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;

        public UpsertBookmarkCommandHandler (IBookmarkRepository bookmarkRepository, IMapper mapper, IMediator mediator) {
            _bookmarkRepository = bookmarkRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<string> Handle (UpsertBookmarkCommand request, CancellationToken cancellationToken) {
            Bookmark entity;
            if (string.IsNullOrEmpty (request.Id)) {
                entity = new Bookmark (request.Url, request.Title, "createdBy_bbe4k56sslf56jll43ls0",
                    BookmarkContentType.Article, DetailType.Detailed, Identifier.New);
            } else {
                entity = await _bookmarkRepository.GetAsync (request.Id);
                entity.Title = request.Title;
            }
            request.Images?.ForEach (o => entity.AddImage (o.FileName, o.Url, o.BookmarkId, "createdBy_bbe4k56sslf56jll43ls0"));
            await _bookmarkRepository.UnitOfWork.SaveAsync (entity, cancellationToken);

            await _mediator.Publish (new BookmarkCreated { BookmarkId = entity.Id }, cancellationToken);

            return entity.Id;
        }
    }
}