######################################################################
# Dockerfile
# Author: Maksim Karpychev
# Date:   07 July 2017
#
# Alfresco server docker image.
#
# Repository, Solr and Share all-in-one image prototype.
# This server expects a Postgres DB to be available.
#
#
######################################################################

FROM ubuntu:14.04

MAINTAINER Maksim Karpychev <kodermax@gmail.com>

ENV ALF_BIN alfresco-community-installer-201704-linux-x64.bin
ENV ALF_URL http://eu.dl.alfresco.com.s3.amazonaws.com/release/community/201704-build-00019/$ALF_BIN
ENV ALF_INSTALL_OPTIONS_FILE install_options.txt
ENV ALF_INSTALLER_DIR /alfresco/installer
ENV ALFRESCO_HOME /opt/alfresco-community
ENV ALF_PROPS_FILE $ALFRESCO_HOME/tomcat/shared/classes/alfresco-global.properties


# Install OpenOffice requirements and other utils
RUN apt-get update && apt install -y \
    libc6 \
    libcups2 \
    libdbus-glib-1-2 \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    libxau6 \
    libxdmcp6 \
    libxext6 \
    libxinerama1 \
    unzip \
    vim \
    curl \
    mc

RUN mkdir -p $ALF_INSTALLER_DIR

# This really adds bloat, the installer could be deleted after it's used, but
# doing it with ADD or COPY results in a layer being added to the image. Only
# way I know of avoiding this at present is to use curl to download the installer from somewhere,
# run it and delete it, but all in a *single* RUN directive.
RUN curl -o $ALF_INSTALLER_DIR/$ALF_BIN $ALF_URL
COPY $ALF_INSTALL_OPTIONS_FILE $ALF_INSTALLER_DIR/

WORKDIR $ALF_INSTALLER_DIR


# Blimey, chmod results in a 829.4MB addition if left to its own devices.
# Removing the ALF_INSTALLER_BIN at the end of this command doesn't stop the COPY above from creating its 829.4MB
# layer, but it does stop the chmod from resulting in a further 829.4MB addition (since the file isn't there
# to commit the change against)
RUN chmod +x $ALF_BIN && ./$ALF_BIN --optionfile $ALF_INSTALL_OPTIONS_FILE && rm $ALF_BIN

EXPOSE 8080 7070 5701


WORKDIR $ALFRESCO_HOME


RUN mkdir /alfresco/data
RUN sed -i 's:^[ \t]*dir.root[ \t]*=\([ \t]*.*\)$:dir.root='${ALFRESCO_HOME}/alf_data':' $ALF_PROPS_FILE

# Change dir.keystore to point to installed keystore - would be nicer to ADD the keystore
# to the data volume
RUN sed -i 's:^[ \t]*dir.keystore[ \t]*=\([ \t]*.*\)$:dir.keystore='${ALFRESCO_HOME}/alf_data/keystore':' $ALF_PROPS_FILE

RUN echo "\n" >> $ALF_PROPS_FILE
RUN echo "authentication.chain=external1:external,alfrescoNtlm1:alfrescoNtlm" >> $ALF_PROPS_FILE
RUN echo "ntlm.authentication.sso.enabled=true" >> $ALF_PROPS_FILE
RUN echo "external.authentication.defaultAdministratorUserNames=admin" >> $ALF_PROPS_FILE
RUN echo "external.authentication.enabled=true" >> $ALF_PROPS_FILE
RUN echo "external.authentication.proxyHeader=X-Alfresco-Remote-User" >> $ALF_PROPS_FILE
RUN echo "external.authentication.proxyUserName=" >> $ALF_PROPS_FILE
RUN echo "aws.accessKey=" >> $AWS_ACCESS_KEY_ID 
RUN echo "aws.secretKey=" >> $AWS_SECRET_ACCESS_KEY
RUN echo "aws.regionName="  >> $AWS_REGION_NAME
RUN echo "aws.s3.bucketName=" >> $AWS_S3_BUCKET_NAME
# Ensure a log file exists ready for tail
RUN touch $ALFRESCO_HOME/alfresco.log

# Logging
ADD dev-log4j.properties $ALFRESCO_HOME/tomcat/shared/classes/alfresco/extension/

# Deploy custom war files
#ADD *.war $ALFRESCO_HOME/tomcat/webapps/

COPY assets/amps $ALFRESCO_HOME/amps

RUN set -x \
	&& bash $ALFRESCO_HOME/bin/apply_amps.sh -force
# Use instead of alfresco.sh -- but it will invoke it.
# This is only when starting the container, it is fine to run alfresco.sh to start and stop
# Alfresco once the container has been initialised.
ADD docker_start $ALFRESCO_HOME/
RUN chmod +x $ALFRESCO_HOME/docker_start

# Run the container's process!
CMD /bin/bash -c $ALFRESCO_HOME/docker_start