
import { Item } from './Entities.js';
import { DAL } from './DAL.js';

let item = new Item(3, 'item3');

/*
DAL.Callbacks.add(item);
DAL.Callbacks.getById(item.id);
DAL.Callbacks.getByName(item.name);

item.name = 'item3-updated';
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

/**/
DAL.AsyncAwait.add(item);
DAL.AsyncAwait.getById(item.id);
DAL.AsyncAwait.getByName(item.name);

item.name = `${item.name}-updated`;
DAL.AsyncAwait.update(item);
DAL.AsyncAwait.delete(item.id);
