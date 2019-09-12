import http = require('http');
import fs = require('fs');
import path = require('path');
import cors = require('cors');

const config = {
    port: 9002,
    resourcesFolder: './resources'
};

const contentTypeMap = new Map();
contentTypeMap.set('.html', 'text/html');
contentTypeMap.set('.txt', 'text/plain');
contentTypeMap.set('.js', 'text/javascript');
contentTypeMap.set('.css', 'text/css');
contentTypeMap.set('.json', 'application/json');
contentTypeMap.set('.png', 'image/png');
contentTypeMap.set('.jpg', 'image/jpg');
contentTypeMap.set('.jfif', 'image/jpg');
contentTypeMap.set('.wav', 'audio/wav');

try {
    http.createServer((request, response) => {
        let filePath = config.resourcesFolder.concat(request.url);
        console.log(`request file ${filePath}`);

        let extname = path.extname(filePath);
        let contentType = contentTypeMap.get(extname) || contentTypeMap.get('.txt');

        let headers = {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': 'OPTIONS, POST, GET'                        
        };
        
        fs.readFile(filePath, function (error, content) {
            if (error) {
                response.writeHead(500, headers);
                response.end(`error: ${error.code}`);
                response.end();
            }
            else {
                headers['Content-Type'] = contentType;
                response.writeHead(200, headers);
                response.end(content, 'utf-8');
            }
        });

    }).listen(config.port);
    console.log(`Server running at http://127.0.0.1:${config.port}/`);
} catch (err) {
    console.log(`Server error: ${err}`);
}