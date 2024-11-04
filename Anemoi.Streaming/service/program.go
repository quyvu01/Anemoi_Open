package main

import (
	"StreamingService/application/configurations"
	"StreamingService/infrastructure/installers"
	"fmt"
	"github.com/labstack/echo/v4"
	"go.uber.org/fx"
)

func main() {
	app := fx.New(
		fx.Provide(installers.NewConfiguration),
		fx.Provide(installers.NewApiService),
		fx.Provide(installers.NewEchoGroup),
		fx.Invoke(func(e *echo.Echo, c *configurations.Configuration) {
			go func() {
				e.Logger.Fatal(e.Start(fmt.Sprintf(":%s", c.Port)))
			}()
		}))
	app.Run()
}
