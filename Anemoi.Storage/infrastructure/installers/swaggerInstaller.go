package installers

import (
	"github.com/labstack/echo/v4"
	echoSwagger "github.com/swaggo/echo-swagger"
)

func NewSwaggerInstaller(echo *echo.Echo) *echo.HandlerFunc {
	wrapHandler := echoSwagger.WrapHandler
	echo.GET("/swagger/*", wrapHandler)
	return &wrapHandler
}
