package controllers

import (
	"StorageService/application/requests/commands/storageCommands/saveFile"
	"StorageService/application/requests/queries/storageQueries/generatePreSignedUrl"
	"StorageService/application/requests/queries/storageQueries/getFile"
	"StorageService/application/responses"
	"github.com/labstack/echo/v4"
	"github.com/mehdihadeli/go-mediatr"
	"io"
	"net/http"
)

type StorageController struct {
}

func NewStorageController(group *echo.Group, echo *echo.Echo) *StorageController {
	controller := &StorageController{}
	echo.GET("/getFile", controller.GetFile)
	group.GET("/generatePreSignUrl", controller.GetPreSignUrl)
	group.POST("/uploadFile", controller.UploadFile)
	return controller
}

// GetFile @description This is the GetFile API
// // @Param   string      key
func (controller *StorageController) GetFile(c echo.Context) error {
	key := c.QueryParam("key")
	file, err := mediatr.Send[*getFile.GetFileQuery, *responses.FileResponse](c.Request().Context(), &getFile.GetFileQuery{Key: key})

	if err != nil {
		return c.JSON(http.StatusBadRequest, err.Error())
	}
	defer file.FileData.Data.Close()

	if file.FileData.ContentType != nil {
		c.Response().Header().Set("Content-Type", *file.FileData.ContentType)
	}

	if val, ok := file.FileData.Metadata["original-file-name"]; ok {
		c.Response().Header().Set("X-Original-File-Name", val)
	}
	_, e := io.Copy(c.Response().Writer, file.FileData.Data)
	if e != nil {
		return c.String(http.StatusBadRequest, "Failed to read the file data!")
	}
	return nil
}

func (controller *StorageController) GetPreSignUrl(c echo.Context) error {
	key := c.QueryParam("key")

	urlWrapper, err := mediatr.Send[*generatePreSignedUrl.PreSignedUrlQuery, *responses.PreSignedUrlResponse](c.Request().Context(), &generatePreSignedUrl.PreSignedUrlQuery{Key: key})

	if err != nil {
		return c.JSON(http.StatusBadRequest, err.Error())
	}
	return c.String(http.StatusOK, urlWrapper.Url)
}

func (controller *StorageController) UploadFile(c echo.Context) error {
	file, err := c.FormFile("file")
	if err != nil {
		return c.JSON(http.StatusBadRequest, err.Error())
	}
	fileOpened, err := file.Open()
	if err != nil {
		return c.JSON(http.StatusBadRequest, err.Error())
	}
	defer fileOpened.Close()
	fileUploaded, err := mediatr.Send[*saveFile.SaveFileCommand, *responses.FileSavedResponse](c.Request().Context(), &saveFile.SaveFileCommand{Folder: "ws", OriginalFileName: file.Filename, Body: io.Reader(fileOpened)})
	if err != nil {
		return c.JSON(http.StatusBadRequest, err.Error())
	}
	return c.String(http.StatusOK, fileUploaded.Key)
}
