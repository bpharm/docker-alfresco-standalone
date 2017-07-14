#!/bin/sh
: ${REMOTE_USER:="bitrix"}
: ${REMOTE_HOST:="portal.binnopharm.ru"}
: ${LOCAL_DIR:="alfresco-proxy/publish/"}
: ${REMOTE_DIR:="/srv/www/aspnetcore/coreapi/alfresco-proxy"}

echo Adding $REMOTE_HOST to known_hosts...
ssh-keyscan -H $REMOTE_HOST >> ~/.ssh/known_hosts -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null

echo Deploying via SSH...
rsync --delete -avzh -e ssh --delete "$LOCAL_DIR" "$REMOTE_USER@$REMOTE_HOST:$REMOTE_DIR"
