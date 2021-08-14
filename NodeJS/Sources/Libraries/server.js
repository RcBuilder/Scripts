// [packages]
// npm install --save eslint  // find & fix problems in javaScript code
const distance = require('euclidean-distance'); // npm install --save euclidean-distance
const { v4: uuid } = require('uuid'); // npm install --save uuid
const readXlsxFile = require('read-excel-file/node'); // npm install --save read-excel-file
const XLSX = require('xlsx');  // npm install --save xlsx

// --- euclidean-distance
console.log('[euclidean-distance]');

let graphA = [2, 2];
let graphB = [5, 4];
let graphC = [3, 5];
console.log(`distance A -> B:  ${distance(graphA, graphB)}`);  // 3.60
console.log(`distance A -> C:  ${distance(graphA, graphC)}`);  // 3.16

// --- uuid
console.log('[uuid]');

for (let i = 0; i < 5; i++)
    console.log(`uuid:  ${uuid()}`); 

// --- eslint
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

try{
    alert('abc');  // Unexpected alert (no-alert)
}
catch(e){
    console.log(`ERROR - NO-ALERTS!, ex:${e}`);    
}

// --- excel
console.log('[excel]');

// File path.
///let filePath = 'C:\\Users\\RcBuilder\\Desktop\\ShekelGroup\\Documents\\data.xls';
readXlsxFile('.\\test1.xlsx').then(rows => {
    /* 
        [ 
            [ 'A1', 'B1', 'C1', 'D1' ],
            [ 'A2', 'B2', 'C2', 'D2' ],
            [ 'A3', 'B3', 'C3', 'D3' ],
            [ 'A4', 'B4', 'C4', 'D4' ] 
        ]
    */
    console.log(rows);

    // rows[row][col]
    console.log(`A1: ${rows[0][0]}`);  // row 0, col 0
    console.log(`B1: ${rows[1][0]}`);  // row 1, col 0
    console.log(`B2: ${rows[1][1]}`);  // row 1, col 1
    console.log(`B3: ${rows[1][2]}`);  // row 1, col 2
});

// with json schema
const schema = {
    // map sheet header 'Column1' to json property 'col1'
    'Column1': { // sheet header name
        prop: 'col1',  // schema property name
        type: String   // schema property type
    },
    'Column2': {
        prop: 'col2',
        type: String
    },
    'Column3': {
        prop: 'col3',
        type: String
    },
    'Column4': {
        prop: 'col4',
        type: String
    }
};
readXlsxFile('.\\test2.xlsx', {schema}).then(({ rows, errors } /* { rows, errors } */) => {
    /*
        [ 
            { col1: 'A1', col2: 'A2', col3: 'A3', col4: 'A4' },
            { col1: 'B1', col2: 'B2', col3: 'B3', col4: 'B4' },
            { col1: 'C1', col2: 'C2', col3: 'C3', col4: 'C4' },
            { col1: 'D1', col2: 'D2', col3: 'D3', col4: 'D4' } 
        ]
    */
    console.log(rows);
    rows.forEach(row => console.log(`${row.col1}${row.col2}`));
});

// --- 

console.log('[excel-2]');
let workbook = XLSX.readFile('.\\test2.xlsx');
let sheetNames = workbook.SheetNames;
console.log(`sheets: ${sheetNames.join(' | ')}`);

let sheet = workbook.Sheets[sheetNames[0]];  // first sheet tab
let dataMap = new Map();

let keys = Object.keys(sheet).filter(k => !k.startsWith('!'));
for(let i = 0; i < keys.length; i++)    
    dataMap.set(keys[i], sheet[keys[i]].v);
/*
    Map {
        'A1' => 'Column1',
        'B1' => 'Column2',
        'C1' => 'Column3',
        'D1' => 'Column4',
        'A2' => 'A1',
        'B2' => 'A2',
        'C2' => 'A3',
        'D2' => 'A4',
        'A3' => 'B1',
        'B3' => 'B2',
        'C3' => 'B3',
        'D3' => 'B4',
        'A4' => 'C1',
        'B4' => 'C2',
        'C4' => 'C3',
        'D4' => 'C4',
        'A5' => 'D1',
        'B5' => 'D2',
        'C5' => 'D3',
        'D5' => 'D4' 
    }
*/
console.log(dataMap);    
console.log(`B3 = ${dataMap.get('B3')}`);    
console.log(`A5 = ${dataMap.get('A5')}`);

// --- 
