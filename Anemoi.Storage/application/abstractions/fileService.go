package abstractions

import (
	"StorageService/application/applicationModels"
	"io"
)

type IFileService interface {
	SaveFiles(folder string, fileName string, body io.Reader) (file string, err error)
	GetFile(key string) (fileData applicationModels.FileData, err error)
	GeneratePreSignedUrl(key string) (preSignLink string, err error)
}
