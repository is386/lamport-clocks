# Lamport Clocks and Kubernetes

This is a program written in C# that implements Lamport Clocks using Akka Actors. Once I completed the implementation, I also used Akka's Cluster API to run this on Kubernetes.

## Usage

`./start` will build the project, build the Docker image, and then run it using Minikube.

`./delete` will delete the service, pods, and statefulset.

## Dependencies

- You will need `docker` and `minikube` to run this project.
