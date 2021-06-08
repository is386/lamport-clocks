dotnet publish -c Release
eval $(minikube docker-env)
docker build -t hw1 .
kubectl apply -f cluster.yaml 
kubectl get all