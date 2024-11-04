package getFile

import (
	"StorageService/application/abstractions"
	"StorageService/application/requests/queries/storageQueries/getFile"
	"StorageService/application/responses"
	"context"
)

type GetFileHandler struct {
	FileService abstractions.IFileService
}

func NewGetFileHandler(fileService abstractions.IFileService) *GetFileHandler {
	return &GetFileHandler{FileService: fileService}
}

func (getFileHandler *GetFileHandler) Handle(_ context.Context, request *getFile.GetFileQuery) (*responses.FileResponse, error) {
	file, err := getFileHandler.FileService.GetFile(request.Key)
	if err != nil {
		return nil, err
	}
	return &responses.FileResponse{FileData: file}, nil
}
