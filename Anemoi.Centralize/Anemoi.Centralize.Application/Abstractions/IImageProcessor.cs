namespace Anemoi.Centralize.Application.Abstractions;

public interface IImageProcessor
{
    Task<Stream> ReduceImageQualityAsync(Stream inputImageStream, int quality, CancellationToken cancellationToken = default);
}