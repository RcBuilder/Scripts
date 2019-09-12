const express = require('express');
const app = express();
const router = require('./router');

const config = {
    port: 9001,
    viewsFolder: './views'
};

// view engine setup
app.set('views', config.viewsFolder);
app.set('view engine', 'pug');

app.use('*', (request, response, next) => {
    //let fullUrl = request.protocol.concat('://', request.headers.host, request.originalUrl);
    //url.parse();

    if (request.headers.accept.indexOf('text/html') == -1) 
        return next();

    let query = request.originalUrl.indexOf('?') == -1 ? '' : request.originalUrl.split('?')[1];
    let keys = query.split('&').map(x => x.split('=')[0]);
    if (keys.indexOf('isPreview') != -1)
        return next();

    query = '?'.concat(query, (query == '' ? '' : '&'), 'isPreview=true');
    let p: string = request.protocol.concat('://', request.headers.host, request.baseUrl, query);
    console.log(p);
    response.redirect(p);
});

app.use('/', router);
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`));