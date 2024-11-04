using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anemoi.BuildingBlock.Application.ApplicationModels;
using OneOf;

namespace Anemoi.BuildingBlock.Application.Abstractions;

public interface IFileService
{
    Task<OneOf<List<string>, Exception>> SaveFilesAsync(IEnumerable<FileData> files,
        string folderName, CancellationToken token = default);

    Task<OneOf<string, Exception>> SaveStreamFileAsync(Stream stream,
        string folderName, string fileName, Dictionary<string, string> metadata, CancellationToken token = default);

    Task<OneOf<string, Exception>> CopyFileAsync(string sourceKey, string destinationKey,
        CancellationToken token = default);

    Task<OneOf<FileResponseData, Exception>> GetFileAsync(string key, CancellationToken token = default);

    Task<OneOf<string, Exception>> GetFileMetadataAsync(string key, string headerKey,
        CancellationToken token = default);

    Task<long> GetFileSizeAsync(string key, CancellationToken token = default);
    Task RemoveFolderAsync(string folder, CancellationToken token = default);

    Task<OneOf<string, Exception>> SaveFileBase64Async(string base64String, string folderName,
        CancellationToken token = default);

    Task<OneOf<string, Exception>> GeneratePreSignedUrlAsync(string key, string preferBucket,
        CancellationToken token = default);
}