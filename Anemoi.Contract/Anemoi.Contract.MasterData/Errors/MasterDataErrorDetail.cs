using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.Contract.MasterData.Errors;

public static class MasterDataErrorDetail
{
    public static class ProvinceError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = ["Error while creating a new province!"], Code = "PEE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = ["Error while updating an exist province!"], Code = "PEE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = ["Province was not found!"], Code = "PEE_03"
        };

        public static ErrorDetail AlreadyExist() => new()
        {
            Messages = ["Province is already exist!"], Code = "PEE_04"
        };
    }

    public static class DistrictError
    {
        public static ErrorDetail CreateFailed() => new()
        {
            Messages = ["Error while creating a new district!"], Code = "DIE_01"
        };

        public static ErrorDetail UpdateFailed() => new()
        {
            Messages = ["Error while updating an exist district!"], Code = "DIE_02"
        };

        public static ErrorDetail NotFound() => new()
        {
            Messages = ["District was not found!"], Code = "DIE_03"
        };
    }
}