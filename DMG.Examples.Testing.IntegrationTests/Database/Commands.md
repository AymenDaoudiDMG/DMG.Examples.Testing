# Create network
docker network create mongo_test

# Build and run MongoDb

docker build -f Dockerfile -t mongodb_test:1.0.0 . --no-cache
docker run -d --name mongodb_test --hostname mongodb_test --network mongo_test --env-file .env --env-file .env-mongo -p 30000:30000 mongodb_test:1.0.0

# Pull and run Mongo Express

docker pull mongo-express
docker run -d --name mongo-express --network mongo_test -p 8081:8081 -e ME_CONFIG_MONGODB_AUTH_DATABASE=dmg_examples_testig -e ME_CONFIG_MONGODB_AUTH_USERNAME=user -e ME_CONFIG_MONGODB_AUTH_PASSWORD=user -e ME_CONFIG_MONGODB_SERVER=mongodb_test -e ME_CONFIG_MONGODB_PORT=30000 mongo-express

(Or use the mongo-express docker-compose extension)