# Use the official Golang 1.22.1 Alpine image as base
FROM golang:1.22.1-alpine3.19

# Set the working directory to root
WORKDIR /

# Copy all files from the local directory to the container's root directory
COPY . .

# Change working directory to root for go mod download
WORKDIR /

# Download dependencies using go modules
RUN go mod download

# Change working directory to /service
WORKDIR /service

# Build the Go app with static linking
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -o ./service ./

# Copy the environment file to /service directory
COPY ./.env /service

ENV PUBLIC_KEY=/service/certificates/public.crt

ENV PORT=8080

# Copy public certificate to /service/certificates directory
COPY ./service/certificates/ /service/certificates/

EXPOSE 8080
# Set the command to run the executable
CMD ["./service"]
