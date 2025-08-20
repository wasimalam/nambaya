pipeline { 
    environment { 
        dockerImage = '' 
        docker_registry="scr.syseleven.de"
        dockerUsername="robot\$dev"
        dockerPassword="eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpYXQiOjE2MDE5NjQwNTksImlzcyI6ImhhcmJvci10b2tlbi1kZWZhdWx0SXNzdWVyIiwiaWQiOjIxMSwicGlkIjo0MDEsImFjY2VzcyI6W3siUmVzb3VyY2UiOiIvcHJvamVjdC80MDEvcmVwb3NpdG9yeSIsIkFjdGlvbiI6InB1c2giLCJFZmZlY3QiOiIifV19.SHadIPN_AGRfhq6R_JB7rbkiPg0lpbPL84nnxm1csnzkxIBCCkp0h4rvRaVqvMAUUB4xb1jSmGr3FS1xR8Gka707V2p3ZQbAIWFzYNIdu0Zd7QXVRtJ64rRHy6pBgrvTJVKaXmvvlZWUED7E0YbrX_DOaIM5AVIK81rO42jOBAfYHWDCIzSWv0Lz4G5tRK58GCFgGXI3Osa7jm7sNocjTzMVUo6QXcfaruzmGukE2ISKekJyZ-VHeJr2_2fnAEeVMzXINJkol14Jx7D0wTJcQb8H6h77IPqcaXodhJ4WIo7bVFlCV6lCxd8tcnty0YXtGLti3eiXG3kEWEIgXrqaykdWURzMzbQhls5J-Rcw5yYdNZosXVtL_iCyGNOEdbzfOHbVxVTyb25sM3WE6Z9a0--On8oYhfYPLWUWOfQhCXzyTdsgpP7wrAbUrtUXrPE-f5G9qSrv3txrIswD81KAK020rNGcEIEefkbPRIR4L3_nHL6O6FC-Qtm0jnqH5kgr7UuZczFPmMntmsxXe4vfQYJyGuTwZtnNTuoNMDdRuZG5dBMT_-92L5OJvckwgfU7IET6In_HNnO33coc8SGzGnmEfY4D_UfZogKh17osVk9UMlHZ2LTB04_uXE3H0yhMeFfcdSsFVPNVmXKJnwumqe8WTicMFpeFPZGKrJ27bGo"
        DOCKER_CONTENT_TRUST=1;
        DOCKER_CONTENT_TRUST_SERVER="https://scr.syseleven.de"
        DOCKER_CONTENT_TRUST_ROOT_PASSPHRASE="nambaya123"
        DOCKER_CONTENT_TRUST_REPOSITORY_PASSPHRASE="nambaya123"

    }
    agent {
        dockerfile {
            filename 'Dockerfile'
            dir 'Services/Patient/API'
        }
    }
    stages {
        stage('Building our image') { 
            steps { 
                script { 
                    dockerImage = docker.build "$docker_registry/ssi-stack-nambaya-dev/jenkins_patient"+ ":$BUILD_NUMBER" 
                }
            } 
        }
        stage('Push our image') { 
            steps { 
                script { 
                    docker.withRegistry( DOCKER_CONTENT_TRUST_SERVER, dockerUsername ) { 
                        dockerImage.push() 
                    }
                } 
            }
        } 
        stage('Cleaning up') { 
            steps { 
                sh "docker rmi $registry:$BUILD_NUMBER" 
            }
        } 
    }
}


