using Anemoi.Centralize.Application.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Anemoi.Centralize.Infrastructure.Services;

public class ImageProcessor : IImageProcessor
{
    public async Task<Stream> ReduceImageQualityAsync(Stream inputImageStream, int quality,
        CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(inputImageStream, cancellationToken);
        var encoder = new JpegEncoder { Quality = quality };
        using var memoryStream = new MemoryStream();
        await image.SaveAsync(memoryStream, encoder, cancellationToken: cancellationToken);
        return memoryStream;
    }
}