const odbc = require('odbc');

const config = {
    connectionString: `Driver={Microsoft Access Driver (*.mdb)};Dbq=${__dirname}\\testDB.mdb;`
}

async function connectToDatabase() {        
    const connectionConfig = {        
        connectionString: config.connectionString,
        connectionTimeout: 10,
        loginTimeout: 10,
    }

    try {
        console.log(process.arch); // x32 or x64
        console.log(config.connectionString);
        const connection = await odbc.connect(connectionConfig);
    }
    catch (ex) {
        console.error(ex);
    }
}

connectToDatabase();