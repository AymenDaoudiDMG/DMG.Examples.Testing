FROM mongo:latest
LABEL maintainer="adaoudi@divisionsinc.com" 

# Compy script for database initialization
COPY mongo-init.js /docker-entrypoint-initdb.d/

# Copy mongod configuration file
COPY mongod.conf /etc/mongod.conf