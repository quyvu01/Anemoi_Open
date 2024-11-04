package installers

import (
	"StorageService/application/abstractions"
	saveFileHandler "StorageService/application/cqrs/commands/storageCommands/saveFile"
	generatePreSignedUrlHandler "StorageService/application/cqrs/queries/storageQueries/generatePreSignedUrl"
	getFileHandler "StorageService/application/cqrs/queries/storageQueries/getFile"
	"StorageService/application/requests/commands/storageCommands/saveFile"
	"StorageService/application/requests/queries/storageQueries/generatePreSignedUrl"
	"StorageService/application/requests/queries/storageQueries/getFile"
	"StorageService/application/responses"
	"github.com/labstack/gommon/log"
	"github.com/mehdihadeli/go-mediatr"
)

func NewMediatorInstaller(fileService abstractions.IFileService) *mediatr.Unit {
	var err error
	err = mediatr.RegisterRequestHandler[*getFile.GetFileQuery, *responses.FileResponse](getFileHandler.NewGetFileHandler(fileService))
	if err != nil {
		log.Fatal(err.Error())
	}
	err = mediatr.RegisterRequestHandler[*generatePreSignedUrl.PreSignedUrlQuery, *responses.PreSignedUrlResponse](generatePreSignedUrlHandler.NewGeneratePreSignedUrlHandler(fileService))
	if err != nil {
		log.Fatal(err.Error())
	}
	err = mediatr.RegisterRequestHandler[*saveFile.SaveFileCommand, *responses.FileSavedResponse](saveFileHandler.NewSaveFileHandler(fileService))
	if err != nil {
		log.Fatal(err.Error())
	}
	return &mediatr.Unit{}
}
