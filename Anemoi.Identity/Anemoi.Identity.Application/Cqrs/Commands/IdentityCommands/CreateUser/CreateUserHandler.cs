using System.Linq;
using System.Threading;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Cqrs.Commands.CommandFlow.CommandOneFlow;
using Anemoi.BuildingBlock.Application.Results;
using Anemoi.BuildingBlock.Infrastructure.RequestHandlers.Commands.EntityFramework.EfCommandOne;
using Anemoi.Contract.Identity.Commands.UserCommands.CreateUser;
using Anemoi.Contract.Identity.Errors;
using Anemoi.Contract.Identity.Responses;
using Anemoi.Identity.Application.Abstractions;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using MassTransit;
using Serilog;

namespace Anemoi.Identity.Application.Cqrs.Commands.IdentityCommands.CreateUser;

public sealed class CreateUserHandler(
    ILogger logger,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IPublishEndpoint publishEndpoint,
    ISqlRepository<User> sqlRepository)
    : EfCommandOneResultHandler<User, CreateUserCommand, UserIdResponse>(sqlRepository, unitOfWork, mapper, logger)
{
    protected override ICommandOneFlowBuilderResult<User, UserIdResponse> BuildCommand(
        IStartOneCommandResult<User, UserIdResponse> fromFlow,
        CreateUserCommand command, CancellationToken cancellationToken)
        => fromFlow
            .CreateOne(Mapper.Map<User>(command))
            .WithCondition(async user =>
            {
                var isPasswordValid = await userRepository
                    .ValidatePasswordAsync(user, command.Password, cancellationToken);
                if (!isPasswordValid)
                    return IdentityErrorDetail.UserError.PasswordNotValid();
                var existedUsers = await SqlRepository
                    .GetManyByConditionAsync(x => x.Email == command.Email,
                        token: cancellationToken);
                if (existedUsers.Any(x => x.IsActivated))
                    return IdentityErrorDetail.UserError.UserExisted();
                var inactiveUsers = existedUsers.Where(a => !a.IsActivated).ToList();
                await SqlRepository.RemoveManyAsync(inactiveUsers, cancellationToken);
                return None.Value;
            })
            .WithErrorIfSaveChange(IdentityErrorDetail.UserError.CreateFailed())
            .WithResultIfSucceed(user => new UserIdResponse { Id = user.UserId.ToString() });
}