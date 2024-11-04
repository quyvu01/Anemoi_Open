using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.Contract.Identity.Errors;

public static class IdentityErrorDetail
{
    public static class IdentityError
    {
        public static ErrorDetail UserLockedOut() => new()
        {
            Messages = ["User has been lockout!"], Code = "IDE_01"
        };

        public static ErrorDetail UserLogInNotAllowed() => new()
        {
            Messages = ["User is not allow to login!"], Code = "IDE_02"
        };

        public static ErrorDetail UserRequiresTwoFactor() => new()
        {
            Messages = ["Two Factor is required for user!"], Code = "IDE_03"
        };

        public static ErrorDetail UserLogOutFailed() => new()
        {
            Messages = ["User log out has been failed!"], Code = "IDE_04"
        };

        public static ErrorDetail FailedToChangePassword() => new()
        {
            Messages = ["Failed to change password!"], Code = "IDE_05"
        };

        public static ErrorDetail PasswordNotCorrect() => new()
        {
            Messages = ["Password was not correct!!"], Code = "IDE_06"
        };

        public static ErrorDetail LoginFailed() => new()
        {
            Messages = ["User logged in has been failed!"], Code = "IDE_07"
        };

        public static ErrorDetail LockUserFailed() => new()
        {
            Messages = ["Lock user failed!"], Code = "IDE_08"
        };

        public static ErrorDetail ResetUserPasswordFailed() => new()
        {
            Messages = ["Reset application user password failed!"], Code = "IDE_09"
        };

        public static ErrorDetail FirstTimePasswordWasNotChanged() => new()
        {
            Messages = ["You are required to change password at first time login!"], Code = "IDE_10"
        };

        public static ErrorDetail UserEmailWasNotVerified() => new()
        {
            Messages = ["User Email was not verified!"], Code = "IDE_12"
        };

        public static ErrorDetail PasswordSuccessRehashNeeded() => new()
        {
            Messages = ["Success Rehash Needed!"], Code = "IDE_15"
        };
    }

    public static class TokenError
    {
        public static ErrorDetail RefreshTokenUsed() => new()
        {
            Messages = ["The refresh token has been used!"], Code = "TEE_01"
        };

        public static ErrorDetail TokenIsNotExpired() => new()
        {
            Messages = ["The token is not expired!"], Code = "TEE_02"
        };

        public static ErrorDetail RefreshTokenNotFound() => new()
        {
            Messages = ["Refresh token not found!"], Code = "TEE_03"
        };

        public static ErrorDetail CreateRefreshTokenFailed() => new()
        {
            Messages = ["Error while creating new refresh token!"], Code = "TEE_04"
        };

        public static ErrorDetail InvalidToken() => new()
        {
            Messages = ["The token is invalid!"], Code = "TEE_05"
        };

        public static ErrorDetail SessionExpired() => new()
        {
            Messages = ["The session has been expired!"], Code = "TEE_07"
        };

        public static ErrorDetail InvalidRefreshToken() => new()
        {
            Messages = ["The refresh token is invalid!"], Code = "TEE_08"
        };
        public static ErrorDetail TokenExpired() => new()
        {
            Messages = ["Token is expired"], Code = "TEE_09"
        };
    }

    public static class UserError
    {
        public static ErrorDetail NotFound() => new()
        {
            Messages = ["User was not found!"], Code = "AUE_01"
        };

        public static ErrorDetail CreateFailed() => new()
        {
            Messages = ["Create User Error"], Code = "AUE_02"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = ["Failed while updating user!"], Code = "AUE_03"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = ["Remove user failed!"], Code = "AUE_04"
        };

        public static ErrorDetail EmailConfirmed() => new()
        {
            Messages = ["User has confirmed this account in the past!"], Code = "AUE_05"
        };

        public static ErrorDetail UserExisted() => new()
        {
            Messages = ["User has already existed!"], Code = "AUE_06"
        };

        public static ErrorDetail PasswordNotValid() => new()
        {
            Messages = ["Password not valid!"], Code = "AUE_11"
        };
    }

    public static class RoleError
    {
        public static ErrorDetail RolesRequestDuplicated() => new()
        {
            Messages = ["Roles request are duplicated!"], Code = "ROE_01"
        };

        public static ErrorDetail RolesNotExist() => new()
        {
            Messages = ["Some Roles are not exist!"], Code = "ROE_02"
        };

        public static ErrorDetail RemoveRolesError() => new()
        {
            Messages = ["Remove roles error"], Code = "ROE_03"
        };

        public static ErrorDetail AddRolesError() => new()
        {
            Messages = ["Add roles error"], Code = "ROE_04"
        };

        public static ErrorDetail RolesRequestMustNotBeNull() => new()
        {
            Messages = ["Roles request must not be null!"], Code = "ROE_05"
        };
    }

    public static class RoleGroupError
    {
        public static ErrorDetail NotFound() => new()
        {
            Messages = ["Role Group was not found!"], Code = "RGE_01"
        };

        public static ErrorDetail CreateFailed() => new()
        {
            Messages = ["Create Role Group Error"], Code = "RGE_02"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = ["Failed while updating Role Group!"], Code = "RGE_03"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = ["Remove Role Group failed!"], Code = "RGE_04"
        };

        public static ErrorDetail DuplicateNameFailed() => new()
        {
            Messages = ["Create Role Group Duplicate Name Error"], Code = "RGE_05"
        };

        public static ErrorDetail Applied() => new()
        {
            Messages = ["Applied Role Group Duplicate Name Error"], Code = "RGE_06"
        };

        public static ErrorDetail RoleGroupDefault() => new()
        {
            Messages = ["This is Role Group Default Error"], Code = "RGE_08"
        };
    }

    public static class ApplicationPolicyError
    {
        public static ErrorDetail NotFound() => new()
        {
            Messages = ["Application Policy was not found!"], Code = "APE_01"
        };

        public static ErrorDetail CreateFailed() => new()
        {
            Messages = ["Create Application Policy Error"], Code = "APE_02"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = ["Failed while updating Application Policy!"], Code = "APE_03"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = ["Remove Application Policy failed!"], Code = "APE_04"
        };
    }

    public static class UserMapRoleGroupError
    {
        public static ErrorDetail NotFound() => new()
        {
            Messages = ["UserMapRoleGroup Policy was not found!"], Code = "UMG_01"
        };

        public static ErrorDetail CreateFailed() => new()
        {
            Messages = ["UserMapRoleGroup Policy Error"], Code = "UMG_02"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = ["Failed while updating UserMapRoleGroup!"], Code = "UMG_03"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = ["Remove UserMapRoleGroup failed!"], Code = "UMG_04"
        };
    }
}