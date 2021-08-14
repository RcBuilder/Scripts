// use checks to validate data
import http from "k6/http";
import { check } from "k6";

export let options = {
    vus: 1,
    iterations: 1
};

export default function() {
    let response = http.get('https://test-api.k6.io/public/crocodiles/');  // api call   

    // http response must returns status OK (200) and use HTTP/2.0
    let check1 = check(response, {
        'http2 is used': x => x.proto === 'HTTP/2.0',
        'status is 200': x => x.status === 200        
    });
    console.log(`result for http response: ${check1}`);  // true

    // number must be an even, positive and small number (less than 10)
    let criteria = {
        'less than 10': x => x < 10,
        'even number': x => x % 2 == 0,
        'positive number': x => x >= 0
    };
    
    let check2 = check(8, criteria);
    console.log(`result for number 8: ${check2}`);  // true

    let check3 = check(25, criteria);
    console.log(`result for number 25: ${check3}`); // false
    
    let check4 = check(7, criteria);
    console.log(`result for number 7: ${check4}`);  // false
    
    let check5 = check(-7, criteria);
    console.log(`result for number -7: ${check5}`); // false
};