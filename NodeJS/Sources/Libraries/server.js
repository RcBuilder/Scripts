console.log('euclidean-distance');
const distance = require('euclidean-distance'); //npm install --save euclidean-distance

let graphA = [2, 2];
let graphB = [5, 4];
let graphC = [3, 5];
console.log(`distance A -> B:  ${distance(graphA, graphB)}`);  // 3.60
console.log(`distance A -> C:  ${distance(graphA, graphC)}`);  // 3.16

// ---

