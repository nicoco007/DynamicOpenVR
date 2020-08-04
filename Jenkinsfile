pipeline {
  agent {
    node {
      label 'windows && vs-15'
    }
  }
  environment {
    GIT_HASH = """${bat(
                  returnStdout: true,
                  script: "@git log -n 1 --pretty=%%h"
               )}""".trim()
    
    GIT_REV = """${bat(
                  returnStdout: true,
                  script: "@git tag -l --points-at HEAD"
               )}""".split("\n").last().trim()
    
    GIT_VERSION = """${GIT_REV.length() > 0 ? GIT_REV : GIT_HASH}"""
  }
  stages {
    stage('Sanity Checks') {
      steps {
        bat 'python bsipa_version_hash.py "Packaging\\DynamicOpenVR\\Plugins\\DynamicOpenVR.manifest" "Source\\DynamicOpenVR\\Properties\\AssemblyInfo.cs"'
        bat 'python bsipa_version_hash.py "Source\\DynamicOpenVR.BeatSaber\\manifest.json" "Source\\DynamicOpenVR.BeatSaber\\Properties\\AssemblyInfo.cs"'
      }
    }
    stage('Prepare Debug') {
      steps {
        bat 'robocopy Packaging Packaging-Debug /E & if %ERRORLEVEL% LEQ 3 (exit /b 0)'
        bat 'mkdir Packaging-Debug\\DynamicOpenVR\\Libs'
        bat 'mkdir Packaging-Debug\\DynamicOpenVR.BeatSaber\\Plugins'
      }
    }
    stage('Build Debug') {
      steps {
        bat 'dotnet build Source\\DynamicOpenVR.sln -c Debug -p:AutomatedBuild=true'
        bat 'copy Source\\DynamicOpenVR\\bin\\Debug\\net4.7.2\\DynamicOpenVR.dll Packaging-Debug\\DynamicOpenVR\\Libs'
        bat 'copy Source\\DynamicOpenVR\\bin\\Debug\\net4.7.2\\DynamicOpenVR.pdb Packaging-Debug\\DynamicOpenVR\\Libs'
        bat 'copy Source\\DynamicOpenVR.BeatSaber\\bin\\Debug\\net4.7.2\\DynamicOpenVR.BeatSaber.dll Packaging-Debug\\DynamicOpenVR.BeatSaber\\Plugins'
        bat 'copy Source\\DynamicOpenVR.BeatSaber\\bin\\Debug\\net4.7.2\\DynamicOpenVR.BeatSaber.pdb Packaging-Debug\\DynamicOpenVR.BeatSaber\\Plugins'
        bat "7z a DynamicOpenVR-${env.GIT_VERSION}-DEBUG.zip -r \"./Packaging-Debug/DynamicOpenVR/*\""
        bat "7z a DynamicOpenVR.BeatSaber-${env.GIT_VERSION}-DEBUG.zip -r \"./Packaging-Debug/DynamicOpenVR.BeatSaber/*\""
        archiveArtifacts "DynamicOpenVR-${env.GIT_VERSION}-DEBUG.zip"
        archiveArtifacts "DynamicOpenVR.BeatSaber-${env.GIT_VERSION}-DEBUG.zip"
      }
    }
    stage('Prepare Release') {
      steps {
        bat 'robocopy Packaging Packaging-Release /E & if %ERRORLEVEL% LEQ 3 (exit /b 0)'
        bat 'mkdir Packaging-Release\\DynamicOpenVR\\Libs'
        bat 'mkdir Packaging-Release\\DynamicOpenVR.BeatSaber\\Plugins'
      }
    }
    stage('Build Release') {
      steps {
        bat 'dotnet build Source\\DynamicOpenVR.sln -c Release -p:AutomatedBuild=true'
        bat 'copy Source\\DynamicOpenVR\\bin\\Release\\net4.7.2\\DynamicOpenVR.dll Packaging-Release\\DynamicOpenVR\\Libs'
        bat 'copy Source\\DynamicOpenVR.BeatSaber\\bin\\Release\\net4.7.2\\DynamicOpenVR.BeatSaber.dll Packaging-Release\\DynamicOpenVR.BeatSaber\\Plugins'
        bat "7z a DynamicOpenVR-${env.GIT_VERSION}-RELEASE.zip -r \"./Packaging-Release/DynamicOpenVR/*\""
        bat "7z a DynamicOpenVR.BeatSaber-${env.GIT_VERSION}-RELEASE.zip -r \"./Packaging-Release/DynamicOpenVR.BeatSaber/*\""
        archiveArtifacts "DynamicOpenVR-${env.GIT_VERSION}-RELEASE.zip"
        archiveArtifacts "DynamicOpenVR.BeatSaber-${env.GIT_VERSION}-RELEASE.zip"
      }
    }
    stage('Clean up') {
      steps {
        cleanWs(cleanWhenAborted: true, cleanWhenFailure: true, cleanWhenNotBuilt: true, cleanWhenSuccess: true, cleanWhenUnstable: true, deleteDirs: true)
      }
    }
  }
}
