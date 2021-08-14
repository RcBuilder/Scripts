// using setup (onInit) and teardown (onCompleted) methods
export function setup() {
    console.log('INIT');
    return { cnt: 1 };
}

export let options = {
    vus: 1,
    iterations: 50    
};

export default function(data) {
    console.log(data.cnt);
    data.cnt++;
}

export function teardown(data) {
    console.log('COMPLETED');
}