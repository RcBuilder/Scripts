// execute an api call 30 times for a single virtual user with sleep of 1 second each execution
import http from "k6/http";
import { sleep } from "k6";

export let options = {
    vus: 1,
    iterations: 30
};

export default function() {
    let response = http.get('https://test-api.k6.io/public/crocodiles/');  // api call   
    sleep(1);  // sleep for 1 seconds
};