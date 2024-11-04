using Anemoi.BuildingBlock.Application.Cqrs.Commands;
using Anemoi.BuildingBlock.Application.Errors;
using Anemoi.Identity.Application.IdentityResults;
using Anemoi.Identity.Domain.Models;
using OneOf;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.TokenGenerators;

public sealed record TokenGeneratorCommand(User User) : ICommand<OneOf<IdentitySuccess, ErrorDetail>>;