# Unity-Build-Release for GitHub Actions
This action builds a unity release.
```
action "Build Release" {
    uses = "actions/unity-build-release",
    DOWNLOAD_URL = download url for unity editor linux release,
    BUILD_PATH = target folder in docker container,
    PROJECT_PATH = target folder in GITHUB_WORKSPACE
}
```
UNITY\_USER and UNITY\_PW should be specified via secret