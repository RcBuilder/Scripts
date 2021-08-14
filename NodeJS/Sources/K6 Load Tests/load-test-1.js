// execute an api call 30 times with max of 30 virtual users
import http from "k6/http";

export let options = {
    vus: 30,
    iterations: 30
};

export default function() {
    let response = http.get('https://test-api.k6.io/public/crocodiles/');  // api call     
};