package applicationModels

import "io"

type FileData struct {
	ContentType *string
	Data        io.ReadCloser
	Metadata    map[string]string
}
