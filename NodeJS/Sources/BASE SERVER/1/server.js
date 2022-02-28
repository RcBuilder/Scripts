const express = require('express');
const app = express();

const config = {
    port: 80
}

// custom middlewares
app.use(
    (request, response, next) => {
        console.log('app-middleware-A');
        next();
    }
);

app.use(
    (request, response, next) => {
        console.log('app-middleware-B');
        next();
    }
);

app.get('/', (request, response) => {
    console.log('root');        
    response.send('some content...');    
});

app.get('/test1', (request, response) => {
    console.log('test1');
    response.send('some other content...');
});

// start
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`)); 