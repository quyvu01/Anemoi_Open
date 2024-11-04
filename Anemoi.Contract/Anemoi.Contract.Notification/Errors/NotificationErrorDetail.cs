using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.Contract.Notification.Errors;

public sealed class NotificationErrorDetail
{
    public static class EmailConfigurationError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new Email Configuration!" }, Code = "PEE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist Email Configuration!" }, Code = "PEE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Email Configuration was not found!" }, Code = "PEE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Email Configuration is already exist!" }, Code = "PEE_04"
        };

        public static ErrorDetail AuthenticateFailed() => new()
        {
            Messages = new[] { "Email Configuration authenticate failed!" }, Code = "PEE_05"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = new[] { "Email Configuration remove failed!" }, Code = "PEE_05"
        };
    }

    public static class EmailTemplateError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new Email Template!" }, Code = "PEE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist Email Template!" }, Code = "PEE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Email Template was not found!" }, Code = "PEE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Email Template is already exist!" }, Code = "PEE_04"
        };
    }

    public static class WorkspaceEmailTemplateError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new Workspace Email Template!" }, Code = "PLE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist Workspace Email Template!" }, Code = "PLE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Workspace Email Template was not found!" }, Code = "PLE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Workspace Email Template is already exist!" }, Code = "PLE_04"
        };

        public static ErrorDetail Applied() => new()
        {
            Messages = new[] { "Workspace Email Template is already applied!" }, Code = "PLE_05"
        };
    }

    public static class WorkspaceEmailTemplateOfferError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating!" }, Code = "PME_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating!" }, Code = "PME_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Not found!" }, Code = "PME_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Already exist!" }, Code = "PME_04"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = new[] { "Remove failed!" }, Code = "PME_05"
        };
    }

    public static class WorkspaceEmailTemplateSendConfigurableError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new Workspace Email Template SendConfigurable!" },
            Code = "PME_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist Workspace Email Template SendConfigurable!" },
            Code = "PME_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Workspace Email Template SendConfigurable was not found!" }, Code = "PME_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Workspace Email Template SendConfigurable is already exist!" }, Code = "PME_04"
        };
    }

    public static class EmailSendingStorageError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new Workspace Email Sending Storage!" },
            Code = "ESE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = new[] { "Error while updating an exist Email Sending Storage!" },
            Code = "ESE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Email Sending Storage was not found!" }, Code = "ESE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = new[] { "Email Sending Storage is already exist!" }, Code = "ESE_04"
        };
    }

    public static class NotificationError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = new[] { "Error while creating a new Notification!" },
            Code = "NTE_01"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = new[] { "Notification was not found!" }, Code = "NTE_02"
        };

        public static ErrorDetail RemoveFailed() => new()
        {
            Messages = new[] { "Error while removing Notification!" }, Code = "NTE_03"
        };
    }
}