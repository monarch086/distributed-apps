# Protobuf Example

## Run Server

`dotnet run --project Server/Protobuf.WebApp/Protobuf.Server/Protobuf.Server.csproj`

Go to 'https://localhost:7047/employees' (or 'http://localhost:5098/employees') in browser to verify server is running (list of employees should appear)

## Run Client

- `npm install`
- `node index.js`

## Compile JS schema

`protoc --js_out=import_style=commonjs,binary:. employees.proto`

## Compile C# schema

`protoc --csharp_out=Protobuf --csharp_opt=file_extension=.pb.cs Protobuf/employees.proto`
