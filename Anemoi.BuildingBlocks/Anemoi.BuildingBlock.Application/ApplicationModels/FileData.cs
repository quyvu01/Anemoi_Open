using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Anemoi.BuildingBlock.Application.ApplicationModels;

public sealed record FileData(IFormFile File, string FileName, Dictionary<string, string> Metadata);