package services

import (
	"StorageService/application/applicationModels"
	"StorageService/application/configurations"
	"context"
	"fmt"
	"github.com/aws/aws-sdk-go-v2/aws"
	"github.com/aws/aws-sdk-go-v2/config"
	"github.com/aws/aws-sdk-go-v2/credentials"
	"github.com/aws/aws-sdk-go-v2/service/s3"
	"github.com/bwmarrin/snowflake"
	"io"
	"path/filepath"
	"time"
)

type S3FileService struct {
	s3Client  *s3.Client
	s3Setting *configurations.S3Setting
}

func NewS3FileService(configuration *configurations.Configuration) *S3FileService {
	s3Setting := configuration.S3Setting
	r2Resolver := aws.EndpointResolverWithOptionsFunc(func(service, region string, options ...interface{}) (aws.Endpoint, error) {
		return aws.Endpoint{
			URL: s3Setting.ServiceUrl,
		}, nil
	})

	cfg, err := config.LoadDefaultConfig(context.TODO(),
		config.WithEndpointResolverWithOptions(r2Resolver),
		config.WithCredentialsProvider(credentials.NewStaticCredentialsProvider(s3Setting.AccessKey, s3Setting.SecretKey, "")),
		config.WithRegion("auto"),
	)
	if err != nil {
		panic(err)
	}

	client := s3.NewFromConfig(cfg)
	return &S3FileService{s3Client: client, s3Setting: &s3Setting}
}

func (fileService *S3FileService) SaveFiles(folder string, fileName string, body io.Reader) (file string, err error) {
	node, err := snowflake.NewNode(1)
	if err != nil {
		return "", err
	}
	id := node.Generate()
	nextFileName := fmt.Sprintf("%s%s", id, filepath.Ext(fileName))
	metadata := map[string]string{}
	metadata["original-file-name"] = fileName
	key := fmt.Sprintf("%s/%s", folder, nextFileName)
	_, e := fileService.s3Client.PutObject(context.TODO(), &s3.PutObjectInput{
		Bucket:   aws.String(fileService.s3Setting.BucketName),
		Key:      aws.String(file),
		Body:     body,
		Metadata: metadata,
	})
	if e != nil {
		return "", err
	}
	return key, nil
}

func (fileService *S3FileService) GetFile(key string) (fileData applicationModels.FileData, err error) {
	file, err := fileService.s3Client.GetObject(context.TODO(), &s3.GetObjectInput{
		Key:    aws.String(key),
		Bucket: aws.String(fileService.s3Setting.BucketName),
	})
	if err != nil {
		return applicationModels.FileData{}, err
	}
	return applicationModels.FileData{Data: file.Body, ContentType: file.ContentType, Metadata: file.Metadata}, nil
}

func (fileService *S3FileService) GeneratePreSignedUrl(key string) (preSignLink string, err error) {
	preSignClient := s3.NewPresignClient(fileService.s3Client)
	file, err := preSignClient.PresignPutObject(context.TODO(), &s3.PutObjectInput{
		Key:    aws.String(key),
		Bucket: aws.String(fileService.s3Setting.BucketName),
	}, func(preSignOptions *s3.PresignOptions) {
		preSignOptions.Expires = 5 * time.Minute
	})
	if err != nil {
		return "", err
	}
	return file.URL, nil
}
