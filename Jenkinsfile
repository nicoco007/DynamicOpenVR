pipeline {
  agent {
    node {
      label 'windows && vs-15'
    }

  }
  stages {
    stage('Build Debug') {
      steps {
        bat 'msbuild /p:Configuration=Debug /p:Platform="Any CPU"'
        bat 'mkdir Packaging\\Plugins'
        bat 'mkdir Packaging\\Libs'
        bat 'copy DynamicOpenVR\\bin\\Debug\\DynamicOpenVR.dll Packaging\\Libs'
        bat 'copy DynamicOpenVR.BeatSaber\\bin\\Debug\\DynamicOpenVR.BeatSaber.dll Packaging\\Plugins'
        bat '7z a DynamicOpenVR.BeatSaber.DEBUG.zip -r "./Packaging/*"'
        archiveArtifacts 'DynamicOpenVR.BeatSaber.DEBUG.zip'
      }
    }
    stage('Build Release') {
      steps {
        bat 'msbuild /p:Configuration=Release /p:Platform="Any CPU"'
        bat 'mkdir Packaging\\Plugins'
        bat 'mkdir Packaging\\Libs'
        bat 'copy DynamicOpenVR\\bin\\Release\\DynamicOpenVR.dll Packaging\\Libs'
        bat 'copy DynamicOpenVR.BeatSaber\\bin\\Release\\DynamicOpenVR.BeatSaber.dll Packaging\\Plugins'
        bat '7z a DynamicOpenVR.BeatSaber.RELEASE.zip -r "./Packaging/*"'
        archiveArtifacts 'DynamicOpenVR.BeatSaber.RELEASE.zip'
      }
    }
    stage('Cleanup') {
      steps {
        cleanWs(cleanWhenAborted: true, cleanWhenFailure: true, cleanWhenNotBuilt: true, cleanWhenSuccess: true, cleanWhenUnstable: true, deleteDirs: true)
      }
    }
  }
}