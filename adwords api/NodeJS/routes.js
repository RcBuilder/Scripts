const express = require('express');
const adwordsAPI = require('./code/adwords');
const config = require('./config.json');

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
    /// request.refresh_token = request.headers['refresh_token'] || '';

    next();
};

router.get('/', (request, response) => {
    console.log('/');
    response.send('Adwords API');
});

router.get('/token', (request, response) => {
    console.log('token');

    let authURL = adwordsAPI.generateAuthenticationUrl();
    response.redirect(authURL);
});

router.post('/refresh_token', async (request, response) => {
    console.log('refresh_token');
    console.log(request.body.refreshToken);

    try {
        let access_token = await adwordsAPI.refreshAccessToken(request.body.refreshToken);
        console.log(access_token);
        response.json({
            access_token
        });
    }
    catch (ex) {
        console.log(`[EX] ${ex.message}`);
        response.status(ex.statusCode).send(ex.message);
    }
});

router.get('/token_response', async (request, response) => {
    console.log('token_response');

    try {
        let { access_token, refresh_token } = await adwordsAPI.getAccessTokenFromAuthorizationCodeAsync(request.query.code);
        console.log(`access_token: ${access_token}`);
        console.log(`refresh_token: ${refresh_token}`);

        switch (config.token_response_format) {
            case 'chromeExtension':
                response.render('token_response.jsx', {
                    token: access_token,
                    refresh_token,
                    extensionId: config.extensionId
                });
                break;
            case 'json':
                response.json({ token: access_token, refresh_token });
                break;
            case 'text':
                response.send(`${access_token}\n${refresh_token}`);
                break;
            default:
                response.status(500).send('no token_response_format has provided!');
        }
    }
    catch (ex) {
        console.log(`[EX] ${ex.message}`);
        response.status(ex.statusCode).send(ex.message);
    }
});

/*
    GET /accounts
    H: Authorization: Bearer <token>
*/
router.get('/accounts', authorize, async (request, response) => {
    console.log('accounts');  

    try {
        let accounts = await adwordsAPI.getAccounts(request.token);
        console.log(accounts);
        response.send(accounts);
    }
    catch (ex) {
        console.log(`[EX] ${ex.message}`);
        response.status(ex.statusCode).send(ex.message);
    }
});

/*
    GET /accounts/8837814803
    H: Authorization: Bearer <token>
*/
router.get('/accounts/:account', authorize, async (request, response) => {
    console.log(`accounts (${request.params.account})`);

    try {
        let accounts = await adwordsAPI.getSubAccounts(request.token, request.params.account);
        console.log(accounts);
        response.send(accounts);
    }
    catch (ex) {
        console.log(`[EX] ${ex.message}`);
        response.status(ex.statusCode).send(ex.message);
    }
});

/*
    GET /campaigns/8837814803
    H: Authorization: Bearer <token>
*/
router.get('/campaigns/:account', authorize, async (request, response) => {    
    console.log(`campaigns (${request.params.account})`);    

    try {
        // { totalCampaigns, campaigns:[ { id, name, metrics } ] }
        let result = await adwordsAPI.getCampaigns(request.token, request.params.account);
        console.log(result.campaigns);
        response.json(result.campaigns);
    }
    catch (ex) {
        console.log(`[EX] ${ex.message}`);
        response.status(ex.statusCode).send(ex.message);
    }
});

/*
    POST /campaigns/stats/8837814803
    H: Authorization: Bearer <token>
    H: Content-Type: application/json
    B: { dateFrom, dateTo }
*/
router.post('/campaigns/stats/:account', authorize, async (request, response) => {
    console.log(`campaigns/stats (${request.params.account})`);    
    console.log(`${request.body.dateFrom} - ${request.body.dateTo}`);    

    // { campaigns:[ { id, name, metrics } ] }
    try {
        let result = await adwordsAPI.getCampaignsWithStats(request.token, request.params.account, request.body.dateFrom, request.body.dateTo);
        console.log(result.campaigns);
        response.json(result.campaigns);
    }
    catch (ex) {           
        console.log(`[EX] ${ex.message}`);
        response.status(ex.statusCode).send(ex.message);
    }
});

router.get('/test', authorize, async (request, response) => {     
    try {
        let result = await adwordsAPI.getCampaignsUsingAWQL(request.token, '8837814803');
        response.json(result.campaigns);
    }
    catch (ex) {
        console.log(`[EX] ${ex.message}`);
        response.status(ex.statusCode).send(ex.message);
    }
});

module.exports = router;