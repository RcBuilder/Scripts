const express = require('express');
const router = express.Router();

router.get('/', (request, response) => {    
    response.send('HELLO WORLD!');
});
router.get('/ping', (request, response) => {    
    response.send('PONG');
});

module.exports = router;