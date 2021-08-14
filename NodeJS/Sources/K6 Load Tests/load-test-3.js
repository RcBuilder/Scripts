// using stages
// start executing for 5 seconds with 10 users, then executes 10 seconds with 20 users and finally executes for 5 seconds till there are no users.
export let options = {    
    stages: [
        { duration: '5s', target: 10 },
        { duration: '10s', target: 20 },
        { duration: '5s', target: 0 },
    ]
};

export default function() {        
   // code here... 
}