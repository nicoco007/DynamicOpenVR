pipeline {
  agent {
    node {
      label 'windows && vs-15'
    }
  }
  stages {
    stage('Prepare Debug') {
      steps {
        bat 'robocopy Packaging Packaging-Debug /E & if %ERRORLEVEL% LEQ 3 (exit /b 0)'
        bat 'mkdir Packaging-Debug\\Plugins'
        bat 'mkdir Packaging-Debug\\Libs'
      }
    }
    stage('Build Debug') {
      steps {
        bat 'msbuild /p:Configuration=Debug /p:Platform="Any CPU" /p:AutomatedBuild=true'
        bat 'copy DynamicOpenVR\\bin\\Debug\\DynamicOpenVR.dll Packaging-Debug\\Libs'
        bat 'copy DynamicOpenVR\\bin\\Debug\\DynamicOpenVR.pdb Packaging-Debug\\Libs'
        bat 'copy DynamicOpenVR.BeatSaber\\bin\\Debug\\DynamicOpenVR.BeatSaber.dll Packaging-Debug\\Plugins'
        bat 'copy DynamicOpenVR.BeatSaber\\bin\\Debug\\DynamicOpenVR.BeatSaber.pdb Packaging-Debug\\Plugins'
        bat '7z a DynamicOpenVR.BeatSaber.DEBUG.zip -r "./Packaging-Debug/*"'
        archiveArtifacts 'DynamicOpenVR.BeatSaber.DEBUG.zip'
      }
    }
    stage('Prepare Release') {
      steps {
        bat 'robocopy Packaging Packaging-Release /E & if %ERRORLEVEL% LEQ 3 (exit /b 0)'
        bat 'mkdir Packaging-Release\\Plugins'
        bat 'mkdir Packaging-Release\\Libs'
      }
    }
    stage('Build Release') {
      steps {
        bat 'msbuild /p:Configuration=Release /p:Platform="Any CPU" /p:AutomatedBuild=true'
        bat 'copy DynamicOpenVR\\bin\\Release\\DynamicOpenVR.dll Packaging-Release\\Libs'
        bat 'copy DynamicOpenVR.BeatSaber\\bin\\Release\\DynamicOpenVR.BeatSaber.dll Packaging-Release\\Plugins'
        bat '7z a DynamicOpenVR.BeatSaber.RELEASE.zip -r "./Packaging-Release/*"'
        archiveArtifacts 'DynamicOpenVR.BeatSaber.RELEASE.zip'
      }
    }
    stage('Clean up') {
      steps {
        cleanWs(cleanWhenAborted: true, cleanWhenFailure: true, cleanWhenNotBuilt: true, cleanWhenSuccess: true, cleanWhenUnstable: true, deleteDirs: true)
      }
    }
  }
}
