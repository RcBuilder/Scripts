const express = require('express');
const app = express();
const router = express.Router();

router.get('/test20', (request, response) => {
    console.log('router1 > test20');
    response.send('some content from router1 ...');
});

module.exports = router;