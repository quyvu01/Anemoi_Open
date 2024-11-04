using System;
using Anemoi.BuildingBlock.Application.Extensions;
using Anemoi.BuildingBlock.Application.Helpers;
using Anemoi.Contract.Identity.Commands.UserCommands.CreateUser;
using Anemoi.Contract.Identity.Commands.UserCommands.UpdateUser;
using Anemoi.Contract.Identity.ModelIds;
using Anemoi.Identity.Application.ApplicationModels.Apple;
using Anemoi.Identity.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Anemoi.Identity.Application.Mappings.UserMappings;

public sealed class ToUserMapping : Profile
{
    public ToUserMapping()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(x => x.PasswordHash, opt => opt.MapFrom<UserPasswordResolve>())
            .ForMember(x => x.ChangedPasswordTime, opt =>
            {
                opt.PreCondition(x => x.Password is { });
                opt.MapFrom(_ => DateTime.UtcNow);
            })
            .ForMember(x => x.SearchHint,
                opt => opt.MapFrom(x => $"{x.FirstName} {x.LastName}".Trim().GenerateSearchHint()))
            .ForMember(x => x.UserId, opt => opt.MapFrom(_ => new UserId(IdGenerator.NextGuid())))
            .ForMember(x => x.UserName, opt => opt.MapFrom(x => x.Email))
            .ForMember(x => x.LockoutEnabled, opt => opt.MapFrom(_ => false))
            .ForMember(x => x.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(x => x.SecurityStamp, opt => opt.MapFrom(_ => IdGenerator.NextGuid().ToString()))
            .AfterMap((_, des) =>
            {
                des.NormalizedUserName = des.UserName?.ToUpper();
                des.Id = des.UserId.Value;
            });

        CreateMap<UpdateUserCommand, User>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.UserId, opt => opt.Ignore())
            .ForMember(x => x.Email, opt => opt.Ignore())
            .ForMember(x => x.SearchHint, opt =>
            {
                opt.PreCondition(x => x.FirstName is { } || x.LastName is { });
                opt.MapFrom(x => $"{x.FirstName} {x.LastName}".Trim().GenerateSearchHint());
            })
            .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember is { }));

        CreateMap<AppleUserData, CreateUserCommand>();
    }

    private sealed class UserPasswordResolve(IPasswordHasher<User> passwordHasher) :
        IValueResolver<CreateUserCommand, User, string>
    {
        public string Resolve(CreateUserCommand source, User destination, string destMember,
            ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Password)) return null;
            var hashedPassword = passwordHasher.HashPassword(destination, source.Password);
            return hashedPassword;
        }
    }
}