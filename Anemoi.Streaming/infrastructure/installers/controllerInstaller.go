package installers

import (
	"StreamingService/application/configurations"
	"github.com/golang-jwt/jwt"
	echojwt "github.com/labstack/echo-jwt/v4"
	"github.com/labstack/echo/v4"
	"os"
)

func NewEchoGroup(e *echo.Echo, configuration *configurations.Configuration) *echo.Group {
	publicFilePath := configuration.PublicKey
	data, err := os.ReadFile(publicFilePath)
	if err != nil {
		panic(err)
	}
	publicKey, err := jwt.ParseRSAPublicKeyFromPEM(data)
	if err != nil {
		panic(err)
	}
	jwtMiddleware := echojwt.WithConfig(echojwt.Config{
		SigningMethod: "RS256",
		SigningKey:    publicKey,
	})

	r := e.Group("/api")
	r.Use(jwtMiddleware)
	return r
}
