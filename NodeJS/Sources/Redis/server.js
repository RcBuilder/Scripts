const express = require('express');
const app = express();

const config = {
    port: 3335,
    redisPort: 6379
}

// Important! MUST run the Redis server (redis-server.exe)
const redis = require('redis');   // // npm install --save redis
const client = redis.createClient(config.redisPort);  // default: 6379 

client.on('error', (error) => {
    console.error(error);
});
client.on('connect', (data) => {
    console.log('connected to redis');
});

// set views folder
app.set('views', __dirname + '/views');

// add jsx engine
app.engine('jsx', require('express-react-views').createEngine());  // npm install express-react-views react react-dom
app.set('view engine', 'jsx');

// add body parser middleware
app.use(require('body-parser').json());  // npm install --save body-parser 

// map a route to static contents
app.use(express.static(__dirname + '/static'));

// GET http://127.0.0.1:3335/
app.get('/', (request, response) => {
    console.log('root');        

    client.keys('*', (error, data) => {
        if (error) response.status(400);
        else response.send(data);  // array of keys
    });        
});

// GET http://127.0.0.1:3335/get/roby
app.get('/get/:name', (request, response) => {
    console.log('get');

    let key = request.params.name;
    client.get(key, (error, data) => {
        if (error) response.status(400);
        else response.json(JSON.parse(data));
    });
});

// GET http://127.0.0.1:3335/delete/roby
app.get('/delete/:name', (request, response) => {
    console.log('delete');

    let key = request.params.name;
    client.del(key, (error, data) => {
        console.log(`${key} was deleted from redis`);
    });

    response.send('DONE');
});

// POST http://127.0.0.1:3335/set/roby
// H: content-type: application/json
// B: {"name":"roby cohen","age":40}
app.post('/set/:name', (request, response) => {
    console.log('set');

    let key = request.params.name;
    client.set(key, JSON.stringify(request.body), () => {        
        console.log(`${key} was added to redis`);
    });
   
    response.send('DONE');
});

// POST http://127.0.0.1:3335/setExp/roby/10
// H: content-type: application/json
// B: {"name":"roby cohen","age":40}
app.post('/setExp/:name/:exp', (request, response) => {
    console.log('setExp');

    let key = request.params.name;
    let expiration = parseInt(request.params.exp);
    client.setex(key, expiration,JSON.stringify(request.body), () => {
        console.log(`${key} was added to redis for ${expiration} seconds`);
    });

    response.send('DONE');
});

// start
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`)); 