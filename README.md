# Lamport Clocks and Kubernetes

This is a program written in C# that implements Lamport Clocks using Akka Actors. Once I completed the implementation, I also used Akka's Cluster API to run this on Kubernetes.

## Usage

`sh start.sh` will build the project, build the Docker image, and then run it using Minikube.

`sh delete.sh` will delete the service, pods, and statefulset.

## Dependencies

- `dotnet 5.0+`

- You will need `docker` and `minikube` to run this project.
