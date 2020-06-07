const express = require('express');
const app = express();
const router = express.Router();

router.get('/test1', (request, response) => {
    console.log('router3 > test1');
    response.send('some content from router3 ...');
});

module.exports = router;