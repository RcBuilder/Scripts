
let MyModule = require('./MyModule.js');
let MyModule2 = require('./MyModule2.js');
import { MyModule3A, MyModule3B, MyModule3Config, MyModule3eColors } from './MyModule3.js';
let Entities = require('./Entities.js');

let exM1 = new MyModule(1, 'module1');
console.log(exM1);

MyModule2.fun1();
MyModule2.fun2();

let exM3a = new MyModule3A(3);
exM3a.add('itemA');
exM3a.add('itemB');
console.log(exM3a.items);

let exM3b = new MyModule3B();
exM3b.sayHello('Roby');

console.log(MyModule3eColors.GREEN);
console.log(MyModule3Config.key1);

let exC1 = new Entities.MyClass1(10, 'item1');
console.log(exC1);

let exC2 = new Entities.MyClass2(20, 'item2');
console.log(exC2);

let exC3 = new Entities.MyClass3(30, 'item3');
console.log(exC3);

/*
require('./MyFile.js');

console.log(f_var_1); // exception!! f_var_1 is not defined
f_foo(); // exception!! f_foo is not defined
let f = new MyFClass('fff'); // exception!! MyFClass is not defined
console.log(f.name);
*/