const express = require('express');
const http = require('http');
const webSocket = require('ws');

const app = express();
const httpServer = http.createServer(app);

const PORT = process.env.PORT || 3000;

const wsServer = new webSocket.Server({
    server: httpServer
}, () => console.log(`WS server is listening at ws://localhost:${WS_PORT}`)); // listen to WS (web-sockets) [ws://]

let connectedClients = [];
wsServer.on('connection', (webSocket, request) => {
    let clientName = request.url.replace('/?u=', '');
    if (clientName == '/') clientName = 'Anonymous';

    console.log(`socket Connected -> ${clientName}`);
    connectedClients.push({
        clientName: clientName,
        ws: webSocket
    });    

    // message: { type, data }
    webSocket.on('message', (sMessage) => {
        let message = JSON.parse(sMessage);
        console.log(`incoming ${message.type} message`);
        
        switch (message.type) {            
            case 'peer2peer': peer2peer(message.data.clientName, message.data.body);
                break;
            case 'broadcast': broadcast(message.data);
                break;            
            default:
                break;
        }
    });

    broadcast = (data) => {
        connectedClients.forEach((cc, i) => {
            if (cc.ws.readyState === cc.ws.OPEN) cc.ws.send(data); // send data through socket
            else connectedClients.splice(i, 1);  // closed socket - remove from collection
        });
    }

    peer2peer = (clientName, data) => {
        let result = connectedClients.filter(cc => cc.clientName == clientName);
        let clientSocket = result.length == 0 ? null : result[0].ws;
        if (!clientSocket) return;
        clientSocket.send(data);
    }
});

app.get('/', (request, response) => {
    response.send('<p>WebSocket SAMPLE</p>');
});
httpServer.listen(PORT, () => console.log(`HTTP server listening at http://localhost:${PORT}`)); // listen to HTTP [http://]
