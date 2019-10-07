pipeline {
  agent {
    node {
      label 'windows && vs-15'
    }

  }
  stages {
    stage('Build') {
      steps {
        bat 'msbuild /p:Configuration=Release /p:Platform="Any CPU"'
        bat 'mkdir Packaging\\Plugins'
        bat 'mkdir Packaging\\Libs'
        bat 'copy DynamicOpenVR\\bin\\Release\\DynamicOpenVR.dll Packaging\\Libs'
        bat 'copy DynamicOpenVR.BeatSaber\\bin\\Release\\DynamicOpenVR.BeatSaber.dll Packaging\\Plugins'
        bat '7z a DynamicOpenVR.BeatSaber.zip -r "./Packaging/*"'
        archiveArtifacts 'DynamicOpenVR.BeatSaber.zip'
      }
    }
    stage('Cleanup') {
      steps {
        cleanWs(cleanWhenAborted: true, cleanWhenFailure: true, cleanWhenNotBuilt: true, cleanWhenSuccess: true, cleanWhenUnstable: true, deleteDirs: true)
      }
    }
  }
}