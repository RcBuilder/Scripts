const express = require('express');
const jwt = require('jsonwebtoken');

const config = {
    port: 2222,
    secretKey: 'my-secret-key'
}

const app = express();
app.use(express.json());

app.get('/', (request, response) => {
    response.send('home-page');
})


/* 
    GET api/users
    H: Authorization: Bearer <access_token>
*/
app.get('/api/users', async (request, response, next) => { 
        let authorization = request.headers['authorization'];
        if(!authorization)
            return response.status(401).send('No Authorization Header');

        let scheme = authorization.split(' ')[0];
        if(!scheme || scheme != 'Bearer')
            return response.status(401).send('Not a Bearer Authorization');

        let token = authorization.split(' ')[1];
        if(!token)
            return response.status(401).send('NoToken');

        try{
            let payloadData = await jwt.verify(token, config.secretKey);
            console.log(payloadData);
            next();
        }
        catch(ex){
            return response.sendStatus(403);
        }

    }, (request, response) => {
    response.json(
        [
            {id: 1, name: 'user-A'},
            {id: 1, name: 'user-B'},
            {id: 1, name: 'user-C'}
        ]
    );
})

/*
    POST api/token
    REQ: grant_type=password&username=&password=
    RES: { access_token }
*/
app.post('/api/token', (request, response) => {
    let username = request.body.username;
    let password = request.body.password;

    // validate credentials here...

    let payloadData = {
        id: '1234567890',
        name: 'John Doe',
        timestamp: 637252521370415500  // DateTime.Now.Ticks
    };
    
    response.json({
        access_token: jwt.sign(payloadData, config.secretKey)
    });
})

app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`));