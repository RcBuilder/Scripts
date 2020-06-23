const express = require('express');
const app = express();

const config = {
    port: 3336,
    connStr: 'mongodb://localhost:27017/testDB'
}

// -------------------------

// Important! MUST run the MongoDB server
const mongoose = require('mongoose');  // npm install --save mongoose
let Schema = mongoose.Schema;  // to generate Schemas

// create a Schema to represents an Item
const ItemSchema = new Schema({
    id: Number,
    name: String,
    price: { type: Number, default: 0 },    
    createdDate: { type: Date, default: Date.now }
});
const ItemModel = mongoose.model('Item', ItemSchema); // create an Item model

(async function () {
    await mongoose.connect(config.connStr, {
        useNewUrlParser: true,
        useUnifiedTopology: true
    });
    console.log('connected to mongo');
})();

// -------------------------

// set views folder
app.set('views', __dirname + '/views');

// add jsx engine
app.engine('jsx', require('express-react-views').createEngine());  // npm install express-react-views react react-dom
app.set('view engine', 'jsx');

// add body parser middleware
app.use(require('body-parser').json());  // npm install --save body-parser 

// map a route to static contents
app.use(express.static(__dirname + '/static'));

// GET http://127.0.0.1:3336/
app.get('/', async (request, response) => {
    console.log('root');        
    
    try {
        let items = await ItemModel.find();                                
        response.send(items.map(x => x.id));  // array of ids
    }
    catch (error) {
        console.error(error);
    };
});

// GET http://127.0.0.1:3336/get/100
app.get('/get/:id', async (request, response) => {
    console.log('get');

    try {
        let id = parseInt(request.params.id);
        let item = await ItemModel.findOne({ id });
        response.json(item); 
    }
    catch (error) {
        console.error(error);
    };
});

// GET http://127.0.0.1:3336/delete/100
app.get('/delete/:id', async (request, response) => {
    console.log('delete');

    try {
        let id = parseInt(request.params.id);
        let item = await ItemModel.findOne({ id });
        let result = await item.deleteOne();        
        console.log(`item ${id} was deleted`);
    }
    catch (error) {
        console.error(error);
    };

    response.send('DONE');
});

// POST http://127.0.0.1:3336/set/100
// H: content-type: application/json
// B: {"name":"item1","price":40.99}
app.post('/set/:id', async (request, response) => {
    console.log('set');
    
    try {
        let id = parseInt(request.params.id);
        let item = await ItemModel.findOne({ id });
        item.name = request.body.name;
        item.price = request.body.price;
                
        let result = await item.save();        
        console.log(`item ${id} was updated`);
    }
    catch (error) {
        console.error(error);
    };
   
    response.send('DONE');
});

// POST http://127.0.0.1:3336/add
// H: content-type: application/json
// B: {"id": 100, "name":"item1","price":40.99}
app.post('/add', async (request, response) => {
    console.log('add');

    try {        
        let newItem = new ItemModel();
        newItem.set(request.body);       
        let result = await newItem.save();
        console.log(`item ${newItem.id} was added`);
    }
    catch (error) {
        console.error(error);
    };

    response.send('DONE');
});

// POST http://127.0.0.1:3336/addMany
// H: content-type: application/json
// B: [{"id": 110, "name":"item110"}, {"id": 111, "name":"item111"}]
app.post('/addMany', async (request, response) => {
    console.log('addMany');

    try {
        let arrItems = request.body;
        let result = await ItemModel.insertMany(arrItems); // use the Model itself!
        console.log(`${arrItems.length} items were added`);
    }
    catch (error) {
        console.error(error);
    };

    response.send('DONE');
});

// start
app.listen(config.port, () => console.log(`Server running at http://127.0.0.1:${config.port}/`)); 