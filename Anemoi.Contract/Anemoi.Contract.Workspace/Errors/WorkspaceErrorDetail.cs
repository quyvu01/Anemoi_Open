using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.Contract.Workspace.Errors;

public static class WorkspaceErrorDetail
{
    public static class WorkspaceError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new workspace!" }, Code = "WKE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist workspace!" }, Code = "WKE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Workspace was not found!" }, Code = "WKE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Workspace is already exist!" }, Code = "WKE_04"
        };

        public static ErrorDetail DomainExist() => new()
        {
            Messages = new[] { "Workspace domain is already exist!" }, Code = "WKE_05"
        };

        public static ErrorDetail WorkspaceExceededLimit() => new()
        {
            Messages = new[] { "Workspace has been Exceeded!" }, Code = "WKE_06"
        };
    }

    public static class OrganizationError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new organization!" }, Code = "OGE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist organization!" }, Code = "OGE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Organization was not found!" }, Code = "OGE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Organization is already exist!" }, Code = "OGE_06"
        };
        public static ErrorDetail DomainExist() => new()
        {
            Messages = new[] { "Organization domain is already exist!" }, Code = "OGE_07"
        };
    }

    public static class MemberInvitationError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new Member Invitation!" }, Code = "MIE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist Member Invitation!" }, Code = "MIE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Member Invitation was not found!" }, Code = "MIE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Member Invitation is already exist!" }, Code = "MIE_04"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = new[] { "Member Invitation was remove failed!" }, Code = "MIE_05"
        };
    }

    public static class MemberError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new member!" }, Code = "MME_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist member!" }, Code = "MME_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Member was not found!" }, Code = "MME_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Member is already exist!" }, Code = "MME_04"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = new[] { "Member was remove failed!" }, Code = "MME_05"
        };
        public static ErrorDetail UserNotFound() => new()
        {
            Messages = new[] { "User was not found!" }, Code = "MME_06"
        };
    }
    public static class MemberMapRoleGroupError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Create failed!" }, Code = "MMG_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Update failed!" }, Code = "MMG_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Not found!" }, Code = "MMG_03"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = new[] { "Remove failed!" }, Code = "MMG_04"
        };
    }
}