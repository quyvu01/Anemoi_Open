using System;

namespace Anemoi.BuildingBlock.Domain;

public sealed class BusinessRuleValidationException(string message) : Exception(message);