pipeline {
	agent any

	environment {
		// 🌟 定义镜像名字和 GitHub 用户名
		IMAGE_NAME = "ghcr.io/strive9930/riverli-blog-api"
		// 🌟 利用 Jenkins 内置的 BUILD_NUMBER 作为唯一的镜像标签 (如 v1, v2, v3)
		IMAGE_TAG = "v${BUILD_NUMBER}"
		// 统一配置仓库的 Git 地址
		MANIFEST_REPO = "github.com/strive9930/riverli-k8s-manifests.git"
	}

	stages {
		stage('📥 1. 检出业务代码') {
			steps {
				// Jenkins 会自动处理当前仓库的代码拉取
				echo "开始构建微服务: ${IMAGE_NAME}:${IMAGE_TAG}"
			}
		}

		stage('📦 2. 构建并推送公共镜像') {
			steps {
				script {
					// 使用标准的 Docker 命令打包 (不需要输入密码，因为构建公共镜像直接推)
					sh "docker build -t ${IMAGE_NAME}:${IMAGE_TAG} ."
					sh "docker build -t ${IMAGE_NAME}:latest ."

					// 登录 GitHub Packages 或 Docker Hub (密码在 Jenkins 凭据里配置)
					withCredentials([usernamePassword(credentialsId: 'github-registry-credentials', usernameVariable: 'REG_USER', passwordVariable: 'REG_PASS')]) {
						sh "echo ${REG_PASS} | docker login -u ${REG_USER} --password-stdin"
						sh "docker push ${IMAGE_NAME}:${IMAGE_TAG}"
						sh "docker push ${IMAGE_NAME}:latest"
					}
				}
			}
		}

		stage('🔄 3. 跨仓库改写 K8s 配置库 (GitOps 核心)') {
			steps {
				script {
					// 1. 使用带有 GitHub Token 的凭证克隆我们的【配置仓库】
					withCredentials([string(credentialsId: 'github-token', variable: 'GITHUB_TOKEN')]) {
						sh "git clone https://${GITHUB_TOKEN}@${MANIFEST_REPO} manifest-folder"
					}

					// 2. 进入配置仓库，使用 Linux 的 sed 命令，精准替换 blog.yaml 里的旧镜像标签
					dir('manifest-folder') {
						// 假设我们在配置库里写的是 image: your-github-username/riverli-blog-api:.*
						// 下面这行命令会自动把冒号后面的标签替换为最新的 v${BUILD_NUMBER}
						sh "sed -i 's|image: ${IMAGE_NAME}:.*|image: ${IMAGE_NAME}:${IMAGE_TAG}|g' apps/blog.yaml"

						// 3. 配置 Git 机器人身份，提交并推回 GitHub
						sh "git config user.name 'Jenkins CI'"
						sh "git config user.email 'jenkins@riverli.com'"
						sh "git add apps/blog.yaml"
						sh "git commit -m '🤖 Jenkins 自动触发发版: ${IMAGE_NAME}:${IMAGE_TAG}'"

						withCredentials([string(credentialsId: 'github-token', variable: 'GITHUB_TOKEN')]) {
							sh "git push https://${GITHUB_TOKEN}@${MANIFEST_REPO} main"
						}
					}
				}
			}
		}
	}
}