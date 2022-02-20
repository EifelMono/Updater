# Updater (Proof of concept)

## Overview

![Alt Text](./images/Updater.Flow.png)


## Publish (create artifacts)

run 
```console
dotnet cake
```
in this folder to create the artifacts

## Test

### One TestApp
```console
dotnet cake --target="TestOneTestApp"
```

### Two TestApps
```console
dotnet cake --target="TestTwoTestApps"
```

### Three TestApps
```console
dotnet cake --target="TestThreeTestApps"
```

### Kill running Apps
```console
dotnet cake --target="KillApps"
```
