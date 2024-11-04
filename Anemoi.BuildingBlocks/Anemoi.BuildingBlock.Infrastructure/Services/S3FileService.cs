using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.StaticFiles;
using OneOf;
using Serilog;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.ApplicationModels;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Application.Extensions;

namespace Anemoi.BuildingBlock.Infrastructure.Services;

public sealed class S3FileService(S3Setting s3Setting, ILogger logger, IAmazonS3 client) : IFileService
{
    public async Task<OneOf<List<string>, Exception>> SaveFilesAsync(IEnumerable<FileData> files, string folderName,
        CancellationToken token = default)
    {
        try
        {
            var isBucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(client, s3Setting.BucketName);
            if (!isBucketExist) await client.PutBucketAsync(s3Setting.BucketName, token);
            var uploadTasks = files
                .Select(async f =>
                {
                    var fileName = Path.Combine(folderName, f.FileName);
                    var uploadRequest = new PutObjectRequest
                    {
                        InputStream = f.File.OpenReadStream(),
                        Key = fileName,
                        BucketName = s3Setting.BucketName,
                        CannedACL = S3CannedACL.NoACL,
                        DisablePayloadSigning = true
                    };
                    uploadRequest.Metadata.Add("original-file-name", HttpUtility.UrlEncode(f.File.FileName));
                    f.Metadata?.ForEach(h => uploadRequest.Metadata.Add(h.Key, h.Value));
                    await client.PutObjectAsync(uploadRequest, token);
                    return fileName;
                });
            var fileNames = await Task.WhenAll(uploadTasks);
            return fileNames.ToList();
        }
        catch (Exception e)
        {
            logger.Error("Error while uploading file to S3: {@Error}", e.Message);
            return e;
        }
    }

    public async Task<OneOf<string, Exception>> SaveStreamFileAsync(Stream stream, string folderName, string fileName,
        Dictionary<string, string> metadata, CancellationToken token = default)
    {
        try
        {
            var isBucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(client, s3Setting.BucketName);
            if (!isBucketExist) await client.PutBucketAsync(s3Setting.BucketName, token);
            var finalFileName = Path.Combine(folderName, fileName);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = finalFileName,
                BucketName = s3Setting.BucketName,
                CannedACL = S3CannedACL.NoACL
            };
            uploadRequest.Metadata.Add("original-file-name", HttpUtility.UrlEncode(fileName));
            metadata?.ForEach(h => uploadRequest.Metadata.Add(h.Key, h.Value));
            var transferUtility = new TransferUtility(client);
            await transferUtility.UploadAsync(uploadRequest, token);
            return finalFileName;
        }
        catch (Exception e)
        {
            logger.Error("Error while uploading file to S3: {@Error}", e.Message);
            return e;
        }
    }

    public async Task<OneOf<string, Exception>> CopyFileAsync(string sourceKey, string destinationKey,
        CancellationToken token = default)
    {
        try
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = s3Setting.BucketName,
                SourceKey = sourceKey,
                DestinationBucket = s3Setting.BucketName,
                DestinationKey = destinationKey
            };
            await client.CopyObjectAsync(request, token);
            return destinationKey;
        }
        catch (Exception e)
        {
            logger.Error("Error while copying file: {@Error}", e.Message);
            return e;
        }
    }

    public async Task<OneOf<FileResponseData, Exception>> GetFileAsync(string key, CancellationToken token = default)
    {
        try
        {
            var obj = await client.GetObjectAsync(s3Setting.BucketName, key, token);
            var contentType = obj.Headers["Content-Type"];
            // var originalFileName = obj.Metadata["original-file-name"];
            var metadata = obj.Metadata.Keys.ToDictionary(k => k, v => obj.Metadata[v]);
            return new FileResponseData(obj.ResponseStream, contentType, metadata);
        }
        catch (Exception e)
        {
            logger.Error("Error while getting file on S3: {@Error}", e.Message);
            return e;
        }
    }

    public async Task<OneOf<string, Exception>> GetFileMetadataAsync(string key, string headerKey,
        CancellationToken token = default)
    {
        try
        {
            var obj = await client.GetObjectMetadataAsync(s3Setting.BucketName, key, token);
            return obj.Metadata[headerKey];
        }
        catch (Exception e)
        {
            logger.Error("Error while getting file meta-data on S3: {@Error}", e.Message);
            logger.Error("Error while getting file meta-data on S3 info: {@Key}", key);
            return e;
        }
    }

    public async Task<long> GetFileSizeAsync(string key, CancellationToken token = default)
    {
        var obj = await client.GetObjectMetadataAsync(s3Setting.BucketName, key, token);
        return obj.Headers.ContentLength;
    }

    public async Task RemoveFolderAsync(string folder, CancellationToken token = default)
    {
        if (!folder.EndsWith("/")) folder += "/";
        var listObjectsRequest = new ListObjectsV2Request
        {
            BucketName = s3Setting.BucketName,
            Prefix = folder
        };

        ListObjectsV2Response listObjectsResponse;
        do
        {
            listObjectsResponse = await client.ListObjectsV2Async(listObjectsRequest, token);
            foreach (var s3Object in listObjectsResponse.S3Objects)
                await client.DeleteObjectAsync(s3Object.BucketName, s3Object.Key, token);
            listObjectsRequest.ContinuationToken = listObjectsResponse.NextContinuationToken;
        } while (listObjectsResponse.IsTruncated);
    }

    public async Task<OneOf<string, Exception>> SaveFileBase64Async(string base64String, string folderName,
        CancellationToken token = default)
    {
        await Task.Yield();
        try
        {
            logger.Information("Save file using: {FileService}", nameof(S3FileService));
            var mimeType = GetMimeTypeFromBase64Uri(base64String);
            var extension = GetExtensionFromMimeType(mimeType);
            var bytes = Convert.FromBase64String(base64String);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var stream = new MemoryStream(bytes);
            var saveResult = await SaveStreamAsync(stream, folderName, fileName, token);
            return saveResult.MapT0(f => Path.Combine(folderName, f));
        }
        catch (Exception e)
        {
            logger.Information("Save file using: {FileService} with path: {@Path}", nameof(S3FileService), e.Message);
            return e;
        }
    }

    private async Task<OneOf<string, Exception>> SaveStreamAsync(Stream stream, string folderName, string fileName,
        CancellationToken token = default)
    {
        try
        {
            var isBucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(client, s3Setting.BucketName);
            if (!isBucketExist) await client.PutBucketAsync(s3Setting.BucketName, token);
            var finalFileName = Path.Combine(folderName, fileName);
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = finalFileName,
                BucketName = s3Setting.BucketName,
                CannedACL = S3CannedACL.NoACL
            };
            uploadRequest.Metadata.Add("original-file-name", fileName);
            var transferUtility = new TransferUtility(client);
            await transferUtility.UploadAsync(uploadRequest, token);
            return fileName;
        }
        catch (Exception e)
        {
            logger.Error("Error while uploading file to S3: {@Error}", e.Message);
            return e;
        }
    }

    private static string GetExtensionFromMimeType(string mimeType)
    {
        var typeProvider = new FileExtensionContentTypeProvider();
        return typeProvider.Mappings
            .FirstOrDefault(a => a.Value == mimeType).Key;
    }

    private static string GetMimeTypeFromBase64Uri(string base64)
    {
        var match = new Regex("^data:(?<type>.+?);base64,(?<data>.+)$").Match(base64);
        if (!match.Success) return GetMimeTypeFromBase64(base64);
        var uriMimeType = match.Groups["type"].Value;
        return uriMimeType;
    }

    private static string GetMimeTypeFromBase64(string base64String) =>
        base64String[..5].ToUpper() switch
        {
            "IVBOR" => "image/png",
            "/9J/4" => "image/jpeg",
            "AAAAF" => "video/mp4",
            "JVBER" => "application/pdf",
            "AAABA" => "mage/x-icon",
            "U1PKC" => "text/plain",
            _ => null
        };

    public async Task<OneOf<string, Exception>> GeneratePreSignedUrlAsync(string key, string preferBucket,
        CancellationToken token = default)
    {
        try
        {
            AWSConfigsS3.UseSignatureVersion4 = true;
            var request = new GetPreSignedUrlRequest
            {
                BucketName = preferBucket,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };
            var url = await client.GetPreSignedURLAsync(request);
            return url;
        }
        catch (Exception e)
        {
            logger.Error("Error while get static url file on S3: {@Error}", e.Message);
            return e;
        }
    }
}