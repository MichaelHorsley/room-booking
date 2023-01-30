#!/bin/bash
DeploymentVersion=deployment_projections_version.txt

namespace="$1"

if test -f "$DeploymentVersion"; 
then
	value=$(<$DeploymentVersion)
    echo "$DeploymentVersion exists and count is $value"
    echo "$namespace deploying to namespace"

    kubectl create ns "$namespace"

    kubectl config set-context --current --namespace="$namespace"

    /bin/bash Deploy/creds.sh

    docker_command="michaelhorsley/room-booking-host-projections:$value"
    echo "$docker_command"

    docker build -t "$docker_command" -f api/room-booking-api/host-projections/pi.Dockerfile ./api/room-booking-api
    echo $((value+1)) > $DeploymentVersion

    push_command="michaelhorsley/room-booking-host-projections:$value"
    docker push "$push_command"

	sed -e "s/<VERSION_NUMBER>/$value/g" -e "s/<NAMESPACE>/$namespace/g" Deploy/host_projections_deployment.yaml > Deploy/host_projections_deploy.yaml

	kubectl apply -f Deploy/host_projections_deploy.yaml
else
	echo "$DeploymentVersion does not exist."
	echo '1' > $DeploymentVersion
fi