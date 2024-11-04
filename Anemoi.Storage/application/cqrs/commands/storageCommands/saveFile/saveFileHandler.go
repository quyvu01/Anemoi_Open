package saveFile

import (
	"StorageService/application/abstractions"
	"StorageService/application/requests/commands/storageCommands/saveFile"
	"StorageService/application/responses"
	"context"
)

type SaveFileHandler struct {
	FileService abstractions.IFileService
}

func NewSaveFileHandler(fileService abstractions.IFileService) *SaveFileHandler {
	return &SaveFileHandler{FileService: fileService}
}

func (saveFileHandler *SaveFileHandler) Handle(_ context.Context, command *saveFile.SaveFileCommand) (*responses.FileSavedResponse, error) {
	key, err := saveFileHandler.FileService.SaveFiles(command.Folder, command.OriginalFileName, command.Body)
	if err != nil {
		return nil, nil
	}
	return &responses.FileSavedResponse{Key: key}, nil
}
