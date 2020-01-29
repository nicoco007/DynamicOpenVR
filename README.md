# DynamicOpenVR
[![Build Status](https://img.shields.io/jenkins/build/https/ci.gnyra.com/job/DynamicOpenVR/job/master?style=flat-square)](https://ci.gnyra.com/blue/organizations/jenkins/DynamicOpenVR/)
[![Latest Version](https://img.shields.io/github/v/release/nicoco007/DynamicOpenVR?include_prereleases&style=flat-square)](https://github.com/nicoco007/DynamicOpenVR/releases)
[![License](https://img.shields.io/github/license/nicoco007/DynamicOpenVR?style=flat-square)](https://github.com/nicoco007/DynamicOpenVR/blob/master/LICENSE)

Unity scripts to allow dynamic creation of OpenVR actions at runtime. Intended to allow 3rd parties to add actions and bindings to an existing game to extend functionality.

## Installing
Download the latest release from [the releases page](https://github.com/nicoco007/DynamicOpenVR/releases) and extract **the entire contents** of the ZIP file into your Beat Saber folder (for Steam, this is usually `C:\Program Files (x86)\Steam\steamapps\common\Beat Saber`), **overwriting any existing files**.

## Troubleshooting
See [the troubleshooting guide](https://github.com/nicoco007/DynamicOpenVR/blob/master/TROUBLESHOOTING.md).

## DynamicOpenVR.BeatSaber
An implementation of DynamicOpenVR as a [Beat Saber](https://beatsaber.com/) plugin. Get the latest (unstable!) [debug](https://ci.gnyra.com/job/DynamicOpenVR/job/master/lastSuccessfulBuild/artifact/DynamicOpenVR.BeatSaber.DEBUG.zip) or [release](https://ci.gnyra.com/job/DynamicOpenVR/job/master/lastSuccessfulBuild/artifact/DynamicOpenVR.BeatSaber.RELEASE.zip) build.

## Contributing
Guidelines coming soon.

To automatically copy the compiled DLLs into Beat Saber's installation directory, create a file called DynamicOpenVR.BeatSaber.csproj.user next to DynamicOpenVR.BeatSaber\DynamicOpenVR.BeatSaber.csproj and paste in the following:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Replace this with the path to your Beat Saber installation -->
    <BeatSaberDir>C:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```
