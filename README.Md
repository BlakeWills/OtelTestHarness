# Open Telemetry Test harness

This is a test harness for https://github.com/open-telemetry/opentelemetry-dotnet/issues/2652

## Scenarios

### Azure Function

- Project: OtelTestHarness

#### Issue:

The Azure isolated function SDK makes a gRPC request on startup, which prevents the HTTP filter from running.

### Console Application:

- Project: OtelTestHarnessApp

#### Issue:

This breaks if the call to MakeGrpcRequest on line 41, within Program.cs is not commented out, as it makes a gRPC request before any http requests.
Note: Run GrpcServer to ensure the grpc request has something to talk too.