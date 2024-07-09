# Demo .NET Health Checks

In orchestration platforms like Kubernetes and Azure Container Apps, health checks are used to monitor the health of containers and ensure they are operational.
Usually, health checks are divided into three main types: liveness probes, readiness probes, and startup probes. This demo shows how to implement health checks in a .NET Core application.



## Kind of Health Checks


### Startup Probes

- **Purpose**: Check if the application inside the container has initialized correctly. It is used for applications with long startup times.
- **Functioning**: Ensures the application has initialized correctly. This prevents false positives in liveness probes during startup.
- **Beavior**: The startup probe is the first probe to run, and the container will not receive traffic until the startup probe passes.
  - If the startup probe fails, the container is restarted, and the startup probe is retried.
  - If the startup probe passes, the liveness and readiness probes are started.
- **Example**: Endpoint `/healthz/startup` that returns a status indicating if the initialization is complete.
    - Status `200 OK` means the initialization is complete.
    - Status `503 Service Unavailable` means the initialization is not complete.


### Liveness Probes

- **Purpose**: Check if the container is alive and operational.
- **Functioning**: Monitors the application to ensure it hasn't entered an irreversibly failed state, such as being stuck or in an infinite loop.
- **Behavior**: The liveness probe is run periodically, and is started after the startup probe passes.
  - If the liveness probe fails multiple times, the container is considered unhealthy, and the container is restarted.
  - If the liveness probe passes, the container continues to receive traffic.
- **Example**: Endpoint `/healthz/live` that returns a status indicating if the container is alive.
  - Status `200 OK` means the container is alive.
  - Status `503 Service Unavailable` means the container is not alive.


### Readiness Probes

- **Purpose**: Check if the container is ready to receive traffic.
- **Functioning**: Ensures that the application and all its dependencies are ready to process requests, including external services and databases.
- **Behavior**: The readiness probe is run periodically, and is started after the startup probe passes.
  - If the readiness probe fails, the container is removed from service and will not receive traffic until the probe passes.
  - If the readiness probe passes, the container continues to receive traffic.
- **Example**: Endpoint `/healthz/ready` that returns a status indicating if the container is ready.
    - Status `200 OK` means the container is ready to receive traffic.
    - Status `503 Service Unavailable` means the container is not ready to receive traffic.


### Summary of Differences:
- **Startup Probe**: Checks if the application has initialized correctly. If it fails, the container is restarted. Used for applications with long startup times.
- **Readiness Probe**: Checks if the application is ready to receive traffic. If it fails, the container is taken out of service.
- **Liveness Probe**: Checks if the application is alive and functioning. If it fails, the container is restarted.



### References

- [Health checks in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks)
