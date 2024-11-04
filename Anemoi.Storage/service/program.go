package main

import (
	"StorageService/application/abstractions"
	"StorageService/application/configurations"
	"StorageService/infrastructure/installers"
	"StorageService/infrastructure/services"
	"StorageService/service/controllers"
	"fmt"
	"github.com/labstack/echo/v4"
	"github.com/mehdihadeli/go-mediatr"
	"go.uber.org/fx"
)

func main() {
	app := fx.New(
		fx.Provide(installers.NewConfiguration),
		fx.Provide(installers.NewApiService),
		fx.Provide(installers.NewEchoGroup),
		fx.Provide(installers.NewMediatorInstaller),
		fx.Provide(installers.NewSwaggerInstaller),
		fx.Provide(fx.Annotate(services.NewS3FileService, fx.As(new(abstractions.IFileService)))),
		fx.Provide(controllers.NewStorageController),
		fx.Invoke(func(e *echo.Echo, c *configurations.Configuration, _ *controllers.StorageController, _ *mediatr.Unit, _ *echo.HandlerFunc) {
			go func() {
				e.Logger.Fatal(e.Start(fmt.Sprintf(":%s", c.Port)))
			}()
		}))
	app.Run()
}
