var express = require('express');
var app = express();
var router = require('./router');
var config = {
    port: 9001,
    viewsFolder: './views'
};
// view engine setup
app.set('views', config.viewsFolder);
app.set('view engine', 'pug');
app.use('*', function (request, response, next) {
    //let fullUrl = request.protocol.concat('://', request.headers.host, request.originalUrl);
    //url.parse();
    if (request.headers.accept.indexOf('text/html') == -1)
        return next();
    var query = request.originalUrl.indexOf('?') == -1 ? '' : request.originalUrl.split('?')[1];
    var keys = query.split('&').map(function (x) { return x.split('=')[0]; });
    if (keys.indexOf('isPreview') != -1)
        return next();
    query = '?'.concat(query, (query == '' ? '' : '&'), 'isPreview=true');
    var p = request.protocol.concat('://', request.headers.host, request.baseUrl, query);
    console.log(p);
    response.redirect(p);
});
app.use('/', router);
app.listen(config.port, function () { return console.log("Server running at http://127.0.0.1:" + config.port + "/"); });
