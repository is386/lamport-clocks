dotnet publish -c Release
eval $(minikube docker-env)
docker build -t hw4 .
kubectl apply -f cluster.yaml 
kubectl get all