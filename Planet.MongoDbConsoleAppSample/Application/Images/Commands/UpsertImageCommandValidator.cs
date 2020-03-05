using FluentValidation;

namespace Planet.MongoDbConsoleAppSample.Application.Images.Commands {
    public class UpsertImageCommandValidator : AbstractValidator<UpsertImageCommand> {
        public UpsertImageCommandValidator () {
            RuleFor (x => x.Id).Must (x => x == null || x.Length == 24);
            RuleFor (x => x.Title).NotEmpty ().MaximumLength (80);
            RuleFor (x => x.Source).NotEmpty ().MaximumLength (2040);
        }
    }
}