const mongo = require('mongodb').MongoClient;
const connStr: string = 'mongodb://localhost:27017';

class Item{
    constructor(public id: number, public name: string){}
}

namespace DAL{
    export class Callbacks{        
        static add(item: Item){        
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').insertOne(item, (err, result) => {
                    if(err){
                        console.log(`[ERROR] insertOne > ${err}`);
                        return;
                    } 
            
                    console.log(`item #${ result.insertedId } added.`);
                    client.close();
                });
            });
        }
        
        static getById(id: number){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').findOne({ id: id }, (err, item: Item) => {
                    if(err){
                        console.log(`[ERROR] getById > ${err}`);
                        return;
                    } 
            
                    console.log(item);
                    client.close();
                });
            });
        }

        static getByName(name: string){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').findOne({ name: name }, (err, item: Item) => {
                    if(err){
                        console.log(`[ERROR] getByName > ${err}`);
                        return;
                    } 
            
                    console.log(item);
                    client.close();
                });
            });
        }

        static update(item: Item){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').updateOne({ id: item.id }, { '$set': item }, (err, result) => {
                    if(err){
                        console.log(`[ERROR] update > ${err}`);
                        return;
                    } 
            
                    console.log(`item updated > ${result.modifiedCount}/${result.matchedCount}`);
                    client.close();
                });
            });
        }

        static delete(id: number){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB')
                db.collection('items').deleteOne({ id: id }, (err, result) => {
                    if(err){
                        console.log(`[ERROR] delete > ${err}`);
                        return;
                    } 
            
                    console.log(`${result.deletedCount} items deleted`);
                    client.close();
                });
            });
        }        
    }

    export class Promises{
        static add(item: Item){        
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').insertOne(item)
                .then(result => {  
                    console.log(`item #${ result.insertedId } added.`);
                    client.close();
                })
                .catch(err => {
                    console.log(`[ERROR] insertOne > ${err}`);
                });                
            });
        }
        
        static getById(id: number){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').findOne({ id: id })
                .then((item: Item) => {  
                    console.log(item);
                    client.close();
                })
                .catch(err => {
                    console.log(`[ERROR] getById > ${err}`);
                });      
            });
        }

        static getByName(name: string){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').findOne({ name: name })
                .then((item: Item) => {  
                    console.log(item);
                    client.close();
                })
                .catch(err => {
                    console.log(`[ERROR] getByName > ${err}`);
                });    
            });
        }

        static update(item: Item){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').updateOne({ id: item.id }, { '$set': item })
                .then(result => {  
                    console.log(`item updated > ${result.modifiedCount}/${result.matchedCount}`);
                    client.close();
                })
                .catch(err => {
                    console.log(`[ERROR] update > ${err}`);
                });    
            });
        }

        static delete(id: number){
            mongo.connect(connStr,  {useNewUrlParser: true }, (err, client) => {
                if(err){
                    console.log(`[ERROR] db.connect > ${err}`);
                    return;
                } 

                let db = client.db('testDB');
                db.collection('items').deleteOne({ id: id })
                .then(result => {  
                    console.log(`${result.deletedCount} items deleted`);
                    client.close();
                })
                .catch(err => {
                    console.log(`[ERROR] delete > ${err}`);
                });   
            });
        }  
    }

    export class AsyncAwait{
        static async add(item: Item){   
            try{ 
                let client = await mongo.connect(connStr,  {useNewUrlParser: true });

                let db = client.db('testDB');
                try{
                    let result = await db.collection('items').insertOne(item);
                    console.log(`item #${ result.insertedId } added.`);
                    client.close();
                }
                catch(err){
                    console.log(`[ERROR] insertOne > ${err}`);
                };     
            }
            catch(err){
                console.log(`[ERROR] db.connect > ${err}`);
            }
        }
        
        static async getById(id: number){
            try{ 
                let client = await mongo.connect(connStr,  {useNewUrlParser: true });

                let db = client.db('testDB');
                try{
                    let item = await db.collection('items').findOne({ id: id });
                    console.log(item);
                    client.close();
                }
                catch(err){
                    console.log(`[ERROR] getById > ${err}`);
                };     
            }
            catch(err){
                console.log(`[ERROR] db.connect > ${err}`);
            }
        }

        static async getByName(name: string){
            try{ 
                let client = await mongo.connect(connStr,  {useNewUrlParser: true });

                let db = client.db('testDB');
                try{
                    let item = await db.collection('items').findOne({ name: name });
                    console.log(item);
                    client.close();
                }
                catch(err){
                    console.log(`[ERROR] getByName > ${err}`);
                };     
            }
            catch(err){
                console.log(`[ERROR] db.connect > ${err}`);
            }
        }

        static async update(item: Item){
            try{ 
                let client = await mongo.connect(connStr,  {useNewUrlParser: true });

                let db = client.db('testDB');
                try{
                    let result = await db.collection('items').updateOne({ id: item.id }, { '$set': item });
                    console.log(`item updated > ${result.modifiedCount}/${result.matchedCount}`);
                    client.close();
                }
                catch(err){
                    console.log(`[ERROR] update > ${err}`);
                };     
            }
            catch(err){
                console.log(`[ERROR] db.connect > ${err}`);
            }
        }

        static async delete(id: number){
            try{ 
                let client = await mongo.connect(connStr,  {useNewUrlParser: true });

                let db = client.db('testDB');
                try{
                    let result = await db.collection('items').deleteOne({ id: id });
                    console.log(`${result.deletedCount} items deleted`);
                    client.close();
                }
                catch(err){
                    console.log(`[ERROR] delete > ${err}`);
                };     
            }
            catch(err){
                console.log(`[ERROR] db.connect > ${err}`);
            }
        }  
    }
}

let item = new Item(1, 'item1');

/*
DAL.Callbacks.add(item);
DAL.Callbacks.getById(item.id);
DAL.Callbacks.getByName(item.name);

item.name = 'item2-updated';
DAL.Callbacks.update(item);
DAL.Callbacks.delete(item.id);
*/

/*
DAL.Promises.add(item);
DAL.Promises.getById(item.id);
DAL.Promises.getByName(item.name);

item.name = 'item3-updated';
DAL.Promises.update(item);
DAL.Promises.delete(item.id);
*/

DAL.AsyncAwait.add(item);
DAL.AsyncAwait.getById(item.id);
DAL.AsyncAwait.getByName(item.name);

item.name = `${item.name}-updated`;
DAL.AsyncAwait.update(item);
DAL.AsyncAwait.delete(item.id);