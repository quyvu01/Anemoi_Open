package installers

import (
	"StreamingService/application/configurations"
	"github.com/spf13/viper"
)

func NewConfiguration() *configurations.Configuration {
	configuration := configurations.Configuration{}
	viper.SetConfigFile("./service/appsettings.json")
	err := viper.ReadInConfig()
	if err != nil {
		panic("Can't find the file appsettings.json")
	}
	err = viper.Unmarshal(&configuration)
	if err != nil {
		panic("Environment can't be loaded!")
	}
	return &configuration
}
