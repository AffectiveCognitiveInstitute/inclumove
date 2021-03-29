#!/bin/sh -l

#get prerequisites
apt-get update -qq; \
apt-get install -qq -y \
gconf-service \
lib32gcc1 \
lib32stdc++6 \
libasound2 \
libarchive13 \
libc6 \
libc6-i386 \
libcairo2 \
libcap2 \
libcups2 \
libdbus-1-3 \
libexpat1 \
libfontconfig1 \
libfreetype6 \
libgcc1 \
libgconf-2-4 \
libgdk-pixbuf2.0-0 \
libgl1-mesa-glx \
libglib2.0-0 \
libglu1-mesa \
libgtk2.0-0 \
libgtk3.0 \
libnspr4 \
libnss3 \
libpango1.0-0 \
libsoup2.4-1 \
libstdc++6 \
libx11-6 \
libxcomposite1 \
libxcursor1 \
libxdamage1 \
libxext6 \
libxfixes3 \
libxi6 \
libxrandr2 \
libxrender1 \
libxtst6 \
zlib1g \
debconf \
npm \
xdg-utils \
lsb-release \
libpq5 \
xvfb \
wget

# download unity
wget -nv ${DOWNLOAD_URL} -O UnitySetup && \
# make executable
chmod +x UnitySetup && \
# agree with license
echo y | \
# install unity with required components
./UnitySetup --unattended \
--install-location=/opt/Unity \
--verbose \
--download-location=/tmp/unity \
--components=Unity,Windows,Windows-Mono,Mac,Mac-Mono,WebGL && \
# remove setup
rm UnitySetup

# create output directory
mkdir -p ${GITHUB_WORKSPACE}/../${BUILD_PATH}

# build the release in output directory
/opt/Unity/Editor/Unity -projectPath ${PROJECT_PATH} -username ${UNITY_USER} -password ${UNITY_PW} -batchmode -nographics -buildWindows64Player ${GITHUB_WORKSPACE}/../${BUILD_PATH}