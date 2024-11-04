package installers

import (
	"context"
	"github.com/labstack/echo/v4"
	"github.com/labstack/echo/v4/middleware"
	"go.uber.org/fx"
	"go.uber.org/zap"
)

func NewApiService(lc fx.Lifecycle) *echo.Echo {
	e := echo.New()
	lc.Append(fx.Hook{
		OnStart: func(context.Context) error {
			e.Use(middleware.Recover())
			e.Use(middleware.Decompress())
			e.Use(middleware.Gzip())
			e.Use(middleware.CORSWithConfig(middleware.CORSConfig{
				AllowOrigins:     []string{"*"},
				AllowMethods:     []string{"*"},
				AllowHeaders:     []string{"*"},
				AllowCredentials: true,
				ExposeHeaders:    []string{"X-Original-File-Name"},
			}))
			logger, _ := zap.NewProduction()
			e.Use(middleware.RequestLoggerWithConfig(middleware.RequestLoggerConfig{
				LogURI:      true,
				LogMethod:   true,
				LogStatus:   true,
				LogRemoteIP: true,
				LogValuesFunc: func(c echo.Context, v middleware.RequestLoggerValues) error {
					logger.Info("request", zap.String("remoteIp", v.RemoteIP), zap.String("method", v.Method), zap.String("uri", v.URI), zap.Int("status", v.Status))
					return nil
				},
			}))
			return nil
		},
		OnStop: func(ctx context.Context) error {
			return e.Shutdown(ctx)
		},
	})
	return e
}
