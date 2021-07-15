"use strict";
exports.__esModule = true;
var express = require("express");
var router = express.Router();
/*
    router.<verb>(<path>, (request, response) => { ... })
*/
/*
router.get('*', (request, response) => {
    //let fullUrl = request.protocol.concat('://', request.headers.host, request.originalUrl);
    //url.parse();

    let query = request.originalUrl.indexOf('?') == -1 ? '' : request.originalUrl.split('?')[1];
    let keys = query.split('&').map(x => x.split('=')[0]);
    if (keys.indexOf('isPreview') != -1) return;
    query = '?'.concat(query, (query == '' ? '' : '&'), 'isPreview=true');
    response.redirect('localhost:9001?isPreview=true');
});
*/
router.get('/', function (request, response) {
    response.render('index', { title: 'index' });
});
router.get('/page1', function (request, response) {
    response.render('page1', { title: 'page1' });
});
router.get('/page2', function (request, response) {
    response.render('page2', { title: 'page2' });
});
router.get('/page3', function (request, response) {
    response.render('page3', { title: 'page3' });
});
router.get('/page4', function (request, response) {
    response.render('page4', { title: 'page4' });
});
module.exports = router;
