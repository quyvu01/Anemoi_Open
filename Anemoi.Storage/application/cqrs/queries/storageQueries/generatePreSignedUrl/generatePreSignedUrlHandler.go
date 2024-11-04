package generatePreSignedUrl

import (
	"StorageService/application/abstractions"
	"StorageService/application/requests/queries/storageQueries/generatePreSignedUrl"
	"StorageService/application/responses"
	"context"
)

type GeneratePreSignedUrlHandler struct {
	FileService abstractions.IFileService
}

func NewGeneratePreSignedUrlHandler(fileService abstractions.IFileService) *GeneratePreSignedUrlHandler {
	return &GeneratePreSignedUrlHandler{FileService: fileService}
}

func (handler *GeneratePreSignedUrlHandler) Handle(_ context.Context, request *generatePreSignedUrl.PreSignedUrlQuery) (*responses.PreSignedUrlResponse, error) {
	url, err := handler.FileService.GeneratePreSignedUrl(request.Key)
	if err != nil {
		return nil, err
	}
	return &responses.PreSignedUrlResponse{Url: url}, nil
}
