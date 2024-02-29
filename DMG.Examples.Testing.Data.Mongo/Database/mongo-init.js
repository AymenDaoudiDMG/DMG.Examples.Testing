db.log.insertOne({ "message": "Login as admin." });
db = db.getSiblingDB('admin');
db.auth(
    process.env.MONGO_INITDB_ROOT_USERNAME,
    process.env.MONGO_INITDB_ROOT_PASSWORD
)

db.log.insertOne({ "message": 'Creating and switching to ${process.env.DB_NAME} database.' });
db = db.getSiblingDB(process.env.DB_NAME);

db.log.insertOne({ "message": 'Creating user for ${process.env.DB_NAME} database.' });
db.createUser({
    user: process.env.DB_USER,
    pwd: process.env.DB_PASSWORD,
    roles: [
        { role: "readWrite", db: process.env.DB_NAME }
    ]
});

db.getSiblingDB(process.env.DB_NAME).auth(process.env.DB_USER, process.env.DB_PASSWORD);

db.createCollection("users");