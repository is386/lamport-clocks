# Homework 4 CS647

## Indervir Singh (is386)

I decided to implement homework 1 with clusters. I could not get it to work for more than 2 actors, so it only runs with 2 replicas. However, it works well overall on kubernetes, and the two actors communicate just fine.

I've included `start.sh` which builds the docker image, and starts the program with `minikube`. `delete.sh` will do a shutdown of the service and pods.
