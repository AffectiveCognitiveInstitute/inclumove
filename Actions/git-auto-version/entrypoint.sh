#!/bin/sh -l

# Install git
apt-get update
apt-get install -y git

git remote rm origin
git remote add origin https://x-access-token:${GITHUB_TOKEN}@github.com/${GITHUB_REPOSITORY}.git

git branch

# fetch master
git fetch origin master
# get most recent head in master for version
latest=$(git tag  -l --merged master --sort='-*authordate' | head -n1)

# get semantic version
semver_parts=(${latest//./})
major=${semver_parts[0]}
minor=${semver_parts[1]}
patch=${semver_parts[2]}

# get current branch name
branch=$(git rev-parse --abbrev-ref HEAD)
# get number of changes
count=$(git rev-list HEAD ^${latest} --ancestry-path ${latest} --count)
version=""
message=""
 
# check if nightly branch
case $branch in
   "master")
    	version=${major}.$((minor+1)).0
    	message="Release build version ${version}"
    	;;
   "${NIGHTLY_BRANCH}")
    	version=${major}.${minor}.${patch}-nightly-${count}
    	message="Nightly build version ${version}"
    	;;
   *)
		>&2 echo "unsupported branch type"
    	exit 1
    	;;
esac

# create tag
git tag -a ${version} -m ${message}

# push tag
git push origin ${version}