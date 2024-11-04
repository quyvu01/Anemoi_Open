package saveFile

import "io"

type SaveFileCommand struct {
	Folder           string
	OriginalFileName string
	Body             io.Reader
}
