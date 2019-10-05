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
        bat 'copy BeatSaber.OpenVR\\bin\\Release\\BeatSaber.OpenVR.dll Packaging\\Plugins'
        bat '7z a BeatSaber.OpenVR.zip -r "./Packaging/*"'
      }
    }
  }
}
