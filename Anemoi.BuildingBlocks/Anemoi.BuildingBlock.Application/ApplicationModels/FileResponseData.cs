using System.Collections.Generic;
using System.IO;

namespace Anemoi.BuildingBlock.Application.ApplicationModels;

public record FileResponseData(Stream FileContent, string ContentType, Dictionary<string, string> Metadata);