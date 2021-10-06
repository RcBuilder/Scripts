const express = require('express');  // npm install --save express
const app = express();
const routes = require('./routes');

const port = process.env.PORT || 5001;

// register routes
app.use('/', routes);

// start
app.listen(port, () => console.log(`Server running at port ${port}`)); 