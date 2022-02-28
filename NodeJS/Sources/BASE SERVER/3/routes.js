const express = require('express');
const config = require('./config.json');
const code1 = require('./code/code1');

const app = express();
const router = express.Router();

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

    request.token = token;  // save the extracted token on the request object in order it to be available on other handlers.

    next();
};

router.get('/', (request, response) => {
    console.log('/');
    response.send('TEST SERVER');
});

// more routes here ...

module.exports = router;