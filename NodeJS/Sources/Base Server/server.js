const express = require('express');
const app = express();
const cors = require('cors');  // npm install --save cors
const routes = require('./routes');
const config = require('./config.json');

// set views folder
app.set('views', __dirname + '/views');
app.use(cors());

// static content middleware
app.use(express.static(__dirname + '/static')); 

// add jsx engine
app.engine('jsx', require('express-react-views').createEngine());  // npm install express-react-views react react-dom
app.set('view engine', 'jsx');

// register routes
app.use('/', routes);

// start
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`)); 