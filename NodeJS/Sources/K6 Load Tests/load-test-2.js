// execute a method to print a random value 60 times with max of 30 virtual users and for total of 20 seconds
const arr = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'];

export let options = {
    vus: 30,
    iterations: 60,
    duration: '20s'
};

export default function() {    
    let rIndex = Math.floor(Math.random() * arr.length);
    console.log(arr[rIndex]);
}