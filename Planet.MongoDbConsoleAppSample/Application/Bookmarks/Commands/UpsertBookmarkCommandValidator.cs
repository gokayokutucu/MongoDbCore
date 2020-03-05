using FluentValidation;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Commands {
    public class UpsertBookmarkCommandValidator : AbstractValidator<UpsertBookmarkCommand> {
        public UpsertBookmarkCommandValidator () {
            RuleFor (x => x.Id).Must (x => x == null || x.Length == 24);
            RuleFor (x => x.Title).NotEmpty ().MaximumLength (80);
            RuleFor (x => x.Url).NotEmpty ().MaximumLength (2040);
        }
    }
}