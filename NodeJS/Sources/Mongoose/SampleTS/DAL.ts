// install mongo (SERVER) + compass (Managment Tools)
// mongodb-win32-x86_64-2012plus-4.2.0-signed.msi
// -----
// mongoose is a schema-based model for mongodb
// > npm install --save mongoose

const mongoose = require('mongoose');  // npm install --save mongoose
let Schema = mongoose.Schema;

/*
    source:
    https://mongoosejs.com/docs/guide.html
    https://mongoosejs.com/docs/api/model.html

    schema types:    
    String, Number, Array, Map etc.    
    
    create a schema:
    <schema> <schemaName> = new Schema({ ... });

    create a model:
    <model> <modelName> = mongoose.model(<name>, <schema>);
    
    use the model to connect to the db
    <model>.<method>

*/

let CatSchema = new Schema({
    name: String,
    ownerName: String,
    collarColor: String
});

export class DAL {
    static async connect() {
        await mongoose.connect('mongodb://localhost:27017/testDB', {
            useNewUrlParser: true,
            useUnifiedTopology: true
        });

        console.log('connected!');
    }

    static async createCats(count) {
        let names = ['Kitty', 'Michi', 'Tom', 'Mao'];
        let colors = ['Red', 'Green', 'Blue', 'Pink', 'Yellow'];

        const CatModel = mongoose.model('Cat', CatSchema);

        let catsToCreate = [];
        for (let i = 0; i < count;i++) {
            catsToCreate.push(new CatModel({
                name: names[Math.floor(Math.random() * names.length)],
                ownerName: 'John doe',
                collarColor: colors[Math.floor(Math.random() * colors.length)]
            }));
        }

        // parallel
        await Promise.all(catsToCreate.map(async c => await c.save()));        
        console.log('all say: meowwwwww');
    }

    static async addCat(name, ownerName, collarColor) {        
        const CatModel = mongoose.model('Cat', CatSchema);
        let kitty = new CatModel({ name, ownerName, collarColor });
        await kitty.save();        
        console.log(`${name} says meowwww`);

        ///kitty.save().then(() => console.log(`${name} says meowwww`));        
    }

    static async getCats() {
        const CatModel = mongoose.model('Cat', CatSchema);
        return await CatModel.find();
    }

    static async getCatsByName(name: string) {
        const CatModel = mongoose.model('Cat', CatSchema);
        return await CatModel.find({ name });
    }

    static async getCatsByCollar(color) {
        const CatModel = mongoose.model('Cat', CatSchema);
        return await CatModel.find({ collarColor: color });
    }

    static async getCatById(id: string) {
        try {
            const CatModel = mongoose.model('Cat', CatSchema);
            return await CatModel.findById({ _id: id });
        }
        catch{ return null; }
    }

    static async countCats() {
        const CatModel = mongoose.model('Cat', CatSchema);
        return await CatModel.countDocuments();
    }

    static async updateCatName(id: string, newName: string) {
        const CatModel = mongoose.model('Cat', CatSchema);        

        // note: use replaceOne() to update a full model instead of properties
        let result = await CatModel.updateOne({ _id: id }, { name: newName });

        // result: { n: 1, nModified: 1, ok: 1 }
        return result.nModified == 1;
    }

    static async deleteCat(id: string) {
        try {
            const CatModel = mongoose.model('Cat', CatSchema);
            let result = await CatModel.deleteOne({ _id: id });

            // { n: 0, ok: 1, deletedCount: 0 }
            return result.deletedCount == 1;
        }
        catch{ return null; }
    }
}