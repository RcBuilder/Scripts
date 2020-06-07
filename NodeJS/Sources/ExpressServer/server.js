const express = require('express');
const app = express();

const config = {
    port: 3333,
    token: '#123456'
}

// set views folder
app.set('views', __dirname + '/views');

// add html engine
app.engine('html', require('webfiller').__express);  // npm install --save webfiller
app.set('view engine', 'html');

// add jsx engine
app.engine('jsx', require('express-react-views').createEngine());  // npm install express-react-views react react-dom
app.set('view engine', 'jsx');

// add pug engine
app.engine('pug', require('pug').__express);  // npm install --save pug
app.set('view engine', 'pug');

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


// text content
app.get('/test1', (request, response) => {
    console.log('test1');        
    response.send('some content...');    
});

// json content
app.get('/test2', (request, response) => {
    console.log('test2');
    response.json({ a: 1, b: 2 });
});

// html content
app.get('/test3', (request, response) => {
    console.log('test3');
    response.send('<p>this is a paragraph</p>');
});

// http status code
app.get('/test4', (request, response) => {
    console.log('test4');
    response.sendStatus(403);
});

// http status code + custom message
app.get('/test5', (request, response) => {
    console.log('test5');
    response.status(403).send('Forbidden');
});

// redirect to local route
app.get('/test6', (request, response) => {
    console.log('test6');
    response.redirect('/test1');
});

// redirect to url
app.get('/test7', (request, response) => {
    console.log('test7');
    response.redirect('http://127.0.0.1:3333/test1');
});

// send a file (html)
app.get('/test8', (request, response) => {
    console.log('test8');
    response.sendFile(__dirname + '/views/view1.html');    
});

// send a file (image)
app.get('/test9', (request, response) => {
    console.log('test9');
    response.sendFile(__dirname + '/static/static1.jpg');
});

// download a file (image)
app.get('/test10', (request, response) => {
    console.log('test10');
    response.download(__dirname + '/static/static1.jpg');
});

// render html file
app.get('/test11', (request, response) => {
    console.log('test11');
    response.render('view1.html');
});

// render jsx file (react)
app.get('/test12', (request, response) => {
    console.log('test12');
    response.render('view1.jsx', { name: 'Roby' });
});

// render pug file
app.get('/test13', (request, response) => {
    console.log('test13');
    response.render('view1.pug', { title: 'page-title', name: 'Roby' });
});

// pipeline - inline filters
app.get('/test14',
    (request, response, next) => {
        console.log('test14-A');      
        next();
    },
    (request, response, next) => {
        console.log('test14-B');
        next();
    },
    (request, response) => {
        console.log('test14=C');
        response.send('some content...');
});

/*
    GET http://127.0.0.1:3333/test15 
    RES: 401 No Authorization Header
    -
    GET http://127.0.0.1:3333/test15
    H: Authorization Basic 1234
    RES: 401 Not a Bearer Authorization
    -
    GET http://127.0.0.1:3333/test15
    H: Authorization Bearer
    RES: 401 No Token
    -
    GET http://127.0.0.1:3333/test15
    H: Authorization Bearer #67890
    RES: 401 Invalid Token
    -
    GET http://127.0.0.1:3333/test15
    H: Authorization: Bearer #123456
    RES: 200 some content...
*/
const authorize = (request, response, next) => {
    console.log('authorization process');

    let authorization = request.headers['authorization'];
    if (!authorization)
        return response.status(401).send('No Authorization Header');

    let scheme = authorization.split(' ')[0];
    if (!scheme || scheme != 'Bearer')
        return response.status(401).send('Not a Bearer Authorization');

    let token = authorization.split(' ')[1];
    if (!token)
        return response.status(401).send('No Token');

    if (token != config.token)
        return response.status(401).send('Invalid Token');

    next();
};

// with authorization process 
app.get('/test15', authorize, (request, response) => {
    console.log('test15');
    response.send('some content...');
});

const filterA = (request, response, next) => {
    request.items = ['itemA'];
    next();
}
const filterB = (request, response, next) => {
    request.items.push('itemB');
    next();
}
const filterC = (request, response, next) => {
    request.items.push('itemC');
    next();
}

// pipeline - external filters
app.get('/test16', filterA, filterB, filterC, (request, response) => {
    console.log('test16');
    response.send(request.items);   // ["itemA","itemB","itemC"]
});

// headers
app.get('/test17', (request, response) => {
    console.log('test17');
    response.send(request.headers);
});

// params
app.post('/test18/:a/:b', (request, response) => {
    console.log('test18');
    console.log(request.body);      // { a: 1, b: 2 }    
    console.log(request.params);    // { a: 1, b: 2 }   
    console.log(request.query);     // { a: 1, b: 2 }  
    response.send('OK');
});

// routers
app.use('/', require('./router1'));       // external router for more routes on the root
app.use('/api', require('./router2'));    // external router for api routes
app.use('/users', require('./router3'));  // external router for users routes

// start
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`)); 