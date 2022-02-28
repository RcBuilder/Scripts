const express = require('express');
const app = express();

const config = {
    port: 3335
}

// set views folder
app.set('views', __dirname + '/views');

// add jsx engine
app.engine('jsx', require('express-react-views').createEngine());  // npm install express-react-views react react-dom
app.set('view engine', 'jsx');

// add body parser middleware
const bodyParser = require('body-parser'); 
app.use(bodyParser.json());  // npm install --save body-parser 

// map a route to static contents
// in this example - the static contents defined under '/static' folder. 
// e.g: /static1.txt
const staticMiddleware = express.static(__dirname + '/static');
app.use(staticMiddleware);

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