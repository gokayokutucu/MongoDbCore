using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbConsoleAppSample.Repositories;

namespace Planet.MongoDbConsoleAppSample.Application.Images.Commands {

    public class UpsertImageCommandHandler : IRequestHandler<UpsertImageCommand, string> {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public UpsertImageCommandHandler (IImageRepository imageRepository, IMapper mapper) {
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        public Task<string> Handle (UpsertImageCommand request, CancellationToken cancellationToken) {
            // Image entity;
            // if (string.IsNullOrEmpty (request.Id)) {
            //     entity = new Image (request.Title, request.Source, "ddkkkg9997glgkg7ghg", default, Identifier.New, request.BookmarkId, request.Id);
            //     request.Owners?.ForEach (o => entity.AddOwner (o.Name, o.Url));
            // } else {
            //     entity = await _imageRepository.GetAsync (request.Id);
            //     request.Owners?.ForEach (o => entity.AddOwner (o.Name, o.Url));
            // }
            // var entityId = await _imageRepository.UnitOfWork.SaveAsync (entity, cancellationToken);

            // return entityId;
            throw new NotImplementedException ();
        }
    }
}