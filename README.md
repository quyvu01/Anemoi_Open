# Install dotnet EF
dotnet tool install --global dotnet-ef

# Change your Database setting on appsettings.json

# To migrate the Database:
dotnet ef migrations add [your_migration_name] --project ../[...].Infrastructure

# To update the database: 
dotnet ef database update

#To Generate RSA Key

This is usually done on the client side.

First, we generate an RSA-2048(PKCS#1) private key.
openssl genrsa -out private.key 2048

Then we generate the public key from the private key.
openssl rsa -in private.key -out public.pem -RSAPublicKey_out

To verify the keys, use any text editor to open the private key and you should see something like this:

-----BEGIN RSA PRIVATE KEY-----
...
-----END RSA PRIVATE KEY-----

The public key would look like this:

-----BEGIN RSA PUBLIC KEY-----
...
-----END RSA PUBLIC KEY-----
