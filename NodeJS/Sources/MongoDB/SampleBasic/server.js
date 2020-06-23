const express = require('express');
const app = express();

// Important! MUST run the MongoDB server
const mongo = require('mongodb').MongoClient;  // npm install --save mongodb

const config = {
    port: 3336,
    connStr: 'mongodb://localhost:27017'
}

// set views folder
app.set('views', __dirname + '/views');

// add jsx engine
app.engine('jsx', require('express-react-views').createEngine());  // npm install express-react-views react react-dom
app.set('view engine', 'jsx');

// add body parser middleware
app.use(require('body-parser').json());  // npm install --save body-parser 

// map a route to static contents
app.use(express.static(__dirname + '/static'));

// GET http://127.0.0.1:3336/
app.get('/', async (request, response) => {
    console.log('root');        

    try {
        let client = await mongo.connect(config.connStr, { useNewUrlParser: true });

        let db = client.db('testDB');
        try {
            let items = await db.collection('items').find().toArray() || [];                        
            client.close();
            response.send(items.map(x => x.id));  // array of ids
        }
        catch (error) {
            console.error(error);
        };
    }
    catch (error) {
        console.log(`[ERROR] db.connect > ${error}`);
    }
});

// GET http://127.0.0.1:3336/get/100
app.get('/get/:id', async (request, response) => {
    console.log('get');

    try {
        let client = await mongo.connect(config.connStr, { useNewUrlParser: true });

        let db = client.db('testDB');
        try {
            let id = parseInt(request.params.id);
            let item = await db.collection('items').findOne({ id: id });                 
            client.close();

            response.json(item);
        }
        catch (error) {
            console.error(error);
        };
    }
    catch (error) {
        console.log(`[ERROR] db.connect > ${error}`);
    }
});

// GET http://127.0.0.1:3336/delete/100
app.get('/delete/:id', async (request, response) => {
    console.log('delete');

    try {
        let client = await mongo.connect(config.connStr, { useNewUrlParser: true });

        let db = client.db('testDB');
        try {
            let id = parseInt(request.params.id);
            let result = await db.collection('items').deleteOne({ id: id });
            console.log(`item ${request.params.id} was deleted`);
            client.close();            
        }
        catch (error) {
            console.error(error);
        };
    }
    catch (error) {
        console.log(`[ERROR] db.connect > ${error}`);
    }

    response.send('DONE');
});

// POST http://127.0.0.1:3336/set/100
// H: content-type: application/json
// B: {"name":"item1","price":40.99}
app.post('/set/:id', async (request, response) => {
    console.log('set');

    try {
        let client = await mongo.connect(config.connStr, { useNewUrlParser: true });

        let db = client.db('testDB');
        try {
            let id = parseInt(request.params.id);
            let result = await db.collection('items').updateOne({ id: id }, { '$set': request.body });            
            console.log(`item ${request.params.id} was updated`);
            client.close();
        }
        catch (error) {
            console.error(error);
        };
    }
    catch (error) {
        console.log(`[ERROR] db.connect > ${error}`);
    }
   
    response.send('DONE');
});

// POST http://127.0.0.1:3336/add
// H: content-type: application/json
// B: {"id": 100, "name":"item1","price":40.99}
app.post('/add', async (request, response) => {
    console.log('set');

    try {
        let client = await mongo.connect(config.connStr, { useNewUrlParser: true });

        let db = client.db('testDB');
        try {
            let result = await db.collection('items').insert(request.body);            
            console.log(`item ${request.body.id} was added`);
            client.close();
        }
        catch (error) {
            console.error(error);
        };
    }
    catch (error) {
        console.log(`[ERROR] db.connect > ${error}`);
    }

    response.send('DONE');
});

// start
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`)); 