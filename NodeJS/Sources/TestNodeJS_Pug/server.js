/* 
    Run:
    > node server.js
 
    Install:
    > npm install express --save
    > npm install cors --save
    > npm install pug --save
*/

const express = require('express');
const app = express();
var cors = require('cors');

const config = {
    port: 8088,
    viewsFolder: './views'
};

let headers = {
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Methods': 'OPTIONS, POST, GET'
};

// view engine setup
app.set('views', config.viewsFolder);
app.set('view engine', 'pug');
app.use(cors());

app.get('/', (request, response) => {
    console.log('/');
    response.render('page1', { title: 'page1' });
});
app.get('/page1', (request, response) => {
    console.log('page1');
    response.render('page1', { title: 'page1' });
});
app.get('/page2', (request, response) => {
    console.log('page2');
    response.render('page2', { title: 'page2' });
});
app.get('/page3', (request, response) => {
    console.log('page3');
    response.render('page3', { title: 'page3' });
});
app.get('/page4', (request, response) => {
    console.log('page4');
    response.render('page4', { title: 'page4' });
});
app.get('/page5', (request, response) => {
    console.log('page5');
    response.render('page5', { title: 'page5' });
});

app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`));