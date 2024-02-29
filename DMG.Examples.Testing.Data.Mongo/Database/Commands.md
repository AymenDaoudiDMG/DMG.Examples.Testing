# Create network
docker network create mongo

# Build and run MongoDb

docker build -f Dockerfile -t mongodb:1.0.0 . --no-cache
docker run -d --name mongodb --hostname mongodb --network mongo --env-file .env --env-file .env-mongo -p 27017:27017 mongodb:1.0.0

# Pull and run Mongo Express

docker pull mongo-express
docker run -d --name mongo-express --network mongo -p 8081:8081 -e ME_CONFIG_MONGODB_AUTH_DATABASE=dmg_examples_testig -e ME_CONFIG_MONGODB_AUTH_USERNAME=user -e ME_CONFIG_MONGODB_AUTH_PASSWORD=user -e ME_CONFIG_MONGODB_SERVER=mongodb mongo-express

(Or use the mongo-express docker-compose extension)