pipeline {
    agent {
        label 'dev-agent'
    }

    environment {
        ENVIRONMENT = "Development"
        NOTIFY_EMAIL = "hoangvodonghi@gmail.com"
    }

    stages {
        stage('Load Env and Generate Config') {
            steps {
                withCredentials([file(credentialsId: 'env-sh', variable: 'ENV_SH')]) {
                    sh '''
                        sed 's/\r//g' "$ENV_SH" | sed '/^[[:space:]]*$/d' > cleaned_env.sh
                        set -a
                        . ./cleaned_env.sh
                        set +a
                        envsubst < Organization.API/appsettings.template.json > Organization.API/appsettings.${ENVIRONMENT}.json
                        rm -f cleaned_env.sh
                    '''
                }
            }
        }

        stage('Generate docker-compose.yml file') {
            steps {
                withCredentials([file(credentialsId: 'docker-env-sh', variable: 'DOCKER_ENV_SH')]) {
                    sh '''
                        sed 's/\r//g' "$DOCKER_ENV_SH" | sed '/^[[:space:]]*$/d' > cleaned_docker_env.sh
                        set -a
                        . ./cleaned_docker_env.sh
                        echo "MY_SERVICE_NAME=$MY_SERVICE_NAME"
                        echo "HOST_PORT=$HOST_PORT"
                        export ASPNETCORE_ENVIRONMENT=${ENVIRONMENT}
                        set +a
                        envsubst < docker-compose.template.yml > docker-compose.yml
                        rm -f cleaned_docker_env.sh
                    '''
                }
            }
        }

        stage('Build') {
            steps {
                echo 'Building Docker image...'
                sh '''#!/bin/bash
                docker-compose build 2>&1 | tee build.log
                exit ${PIPESTATUS[0]}
                '''
            }
        }

        stage('Remove Existing Containers') {
            steps {
                echo 'Remove existing containers...' 
                sh '''
                docker-compose down --remove-orphans
                '''
            }
        }

        stage('Test') {
            steps {
                echo 'Running tests in container...'
                sh '''
                 
                '''
            }
        }

        stage('Deloy') {
            steps {
                echo 'Deloying application...'
                sh '''
                 docker-compose up -d
                '''
            }
        }

        stage('Check Health') {
            steps {
                withCredentials([file(credentialsId: 'docker-env-sh', variable: 'DOCKER_ENV_SH')]) {
                    echo 'Checking health of service...'
                    sh '''
                        sed 's/\\r//g' "$DOCKER_ENV_SH" | sed '/^[[:space:]]*$/d' > cleaned_docker_env.sh
                        set -a
                        . ./cleaned_docker_env.sh
                        set +a

                        SERVICE_NAME="$MY_SERVICE_NAME"
                        cid=$(docker-compose ps -q $SERVICE_NAME)
                        if [ -z "$cid" ]; then
                            echo "❌ Not found container for service $SERVICE_NAME" | tee health.log
                            exit 1
                        fi

                        # Lấy trạng thái "health" nếu có, nếu không thì fallback sang trạng thái "status"
                        health=$(docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}no-healthcheck{{end}}' $cid)
                        status=$(docker inspect --format='{{.State.Status}}' $cid)

                        if [ "$health" = "no-healthcheck" ]; then
                            echo "ℹ️ Container: $SERVICE_NAME has no HEALTHCHECK configured." | tee health.log
                            echo "🔍 Status: $status" | tee -a health.log
                            if [ "$status" != "running" ]; then
                                echo "❌ $SERVICE_NAME is not running!" >> health.log
                                exit 1
                            fi
                        else
                            echo "Container: $SERVICE_NAME, Health: $health, Status: $status" | tee health.log
                            if [ "$health" = "unhealthy" ]; then
                                echo "❌ $SERVICE_NAME is unhealthy!" >> health.log
                                exit 1
                            fi
                        fi

                        echo "✅ $SERVICE_NAME is running and healthy (if configured)." | tee -a health.log
                        rm -f cleaned_docker_env.sh
                    '''
                }
            }
        }
    }

    post {
        success {
            script {
                def recipients = env.NOTIFY_EMAIL
                def healthInfo = fileExists('health.log') ? readFile('health.log') : 'No health check info available.'

                emailext(
                    subject: "✅ Jenkins Pipeline Succeeded: ${env.JOB_NAME} #${env.BUILD_NUMBER}",
                    to: recipients,
                    mimeType: 'text/html',
                    body: """
                        <html>
                        <body style="font-family: Arial, sans-serif; color: #333;">
                            <h2 style="color: green;">✅ Build Successful!</h2>
                            <p>The Jenkins pipeline <b>${env.JOB_NAME}</b> build <b>#${env.BUILD_NUMBER}</b> has completed successfully.</p>
                            <p>Check the build details at: <a href="${env.BUILD_URL}">${env.BUILD_URL}</a></p>
                            <hr>
                            <h3>🔍 Container Health Check:</h3>
                            <pre style="background:#f8f8f8; border:1px solid #ccc; padding:10px;">${healthInfo}</pre>
                            <hr>
                            <p style="font-size: small; color: gray;">This is an automated notification.</p>
                        </body>
                        </html>
                    """
                )
                sh 'rm -f health.log'
            }
        }

        failure {
            script {
                def recipients = env.NOTIFY_EMAIL
                def logs = fileExists('build.log') ? readFile('build.log') : 'No build.log file found.'
                def healthInfo = fileExists('health.log') ? readFile('health.log') : 'No health check info available.'

                emailext(
                    subject: "❌ Jenkins Pipeline Failed: ${env.JOB_NAME} #${env.BUILD_NUMBER}",
                    to: recipients,
                    mimeType: 'text/html',
                    body: """
                        <html>
                        <body style="font-family: Arial, sans-serif; color: #333;">
                            <h2 style="color: red;">❌ Build Failed!</h2>
                            <p>The Jenkins pipeline <b>${env.JOB_NAME}</b> build <b>#${env.BUILD_NUMBER}</b> has failed.</p>
                            <p>Please check the build details at: <a href="${env.BUILD_URL}">${env.BUILD_URL}</a></p>

                            <hr>
                            <h3>🔍 Container Health Check:</h3>
                            <pre style="background:#f8f8f8; border:1px solid #ccc; padding:10px;">${healthInfo}</pre>

                            <hr>
                            <h3>🪵 Build Logs:</h3>
                            <pre style="background:#f8f8f8; border:1px solid #ccc; padding:10px; max-height:400px; overflow:auto;">${logs}</pre>
                            
                            <p style="font-size: small; color: gray;">This is an automated notification. Please address the failure as soon as possible.</p>
                        </body>
                        </html>
                    """
                )
                sh 'rm -f build.log || true'
                sh 'rm -f health.log || true'
            }
        }
    }

}