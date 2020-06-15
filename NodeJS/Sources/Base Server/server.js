const express = require('express');  // npm install --save express
const app = express();
const cors = require('cors');  // npm install --save cors
const routes = require('./routes');
const config = require('./config.json');

// set views folder
app.set('views', __dirname + '/views');

// set cors
app.use(cors());

// static content middleware
app.use(express.static(__dirname + '/static')); 

// add jsx engine
app.engine('jsx', require('express-react-views').createEngine());  // npm install express-react-views react react-dom
app.set('view engine', 'jsx');

// add body parser middleware
const bodyParser = require('body-parser');
app.use(bodyParser.json());  // npm install --save body-parser 

// custom middlewares
app.use(
    (request, response, next) => {
        console.log('app-middleware-A');
        next();
    }
);
// more custom middlewares here ...

// register routes
app.use('/', routes);

// start
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`)); 