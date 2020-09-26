// npm install --save eslint
const distance = require('euclidean-distance'); // npm install --save euclidean-distance
const { v4: uuid } = require('uuid'); // npm install --save uuid

// ---
console.log('[euclidean-distance]');

let graphA = [2, 2];
let graphB = [5, 4];
let graphC = [3, 5];
console.log(`distance A -> B:  ${distance(graphA, graphB)}`);  // 3.60
console.log(`distance A -> C:  ${distance(graphA, graphC)}`);  // 3.16

// ---
console.log('[uuid]');

for (let i = 0; i < 5; i++)
    console.log(`uuid:  ${uuid()}`); 

// ---
console.log('[eslint]');

let var1 = 1;
let var2 = 2;  // eslint: 'var2' is assigned a value but never used (no-unused-vars)
console.log(var1);

function some_func (x, y) // Unexpected space before function parentheses (space-before-function-paren)
{  
    console.log(`x: ${x}, y: ${y}`);    
}
some_func(10,20);

let arr = ['a', 'b', "c"];    // Strings must use singlequote (quotes))
console.log(arr);

let obj = {
    id: 1,
    name: 'joe', // Strings must use singlequote (comma-dangle)
};    
console.log(obj);

alert('abc');  // Unexpected alert (no-alert)

// ---