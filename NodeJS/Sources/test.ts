// pure javascript
function classA(){
    return 'hello world';
}

let str:string = 'hi';
let i:number = 4;
let j:number

//@ts-ignore
j = 'aa'; // Type '"aa"' is not assignable to type 'number'

let str2 = 'string';
//@ts-ignore
str2 = 1; // error: Type 1 is not assignable to type 'string'

interface iUser{
    id: number,
    name:string
}

function classB(user:iUser){
    return `the user ${user.name} has the identity ${user.id}`;
}

let roby: iUser = { name: 'Roby', id: 100 };
console.log(classB(roby)); 

class User {    
    constructor(public id:number, public name:string){}

    sayHi():string {
        return `hi, my name is ${this.name}`;
    }
}

class Employee extends User // Employee inherit from User
{    
    salary:number;
    constructor(id:number, name:string, salary:number){   
        super(id, name);  // call base constructor   
        this.salary = salary;  
    }

    sayHi() {
        return `hi, my name is ${this.name} and i earn ${this.salary} a month`;
    }
}

let employee = new Employee(100, 'Roby', 30000);
console.log(employee.sayHi());

let abc:any = 10;
let bcd: string = <string>abc;

enum eColor{
    red = 12, green, blue
}

let red = eColor.red;
console.log(red);
console.log(red === eColor.red);

//tuple [<data type>,<data type>]
let tuple: [string, number];
tuple = ['A', 65];
console.log(tuple); // ['A', 65] 

function fun(p1:number, p2:string, p3:boolean, p4:any, p5:string[]){}
fun(1, 'str', false, {}, ['A', 'B', 'C']);

let k: boolean|number;
k = 12;
k = true;
//@ts-ignore
k = 'string';  // Type 'string' is not assignable to type 'boolean | number'

let m1:()=>string;
m1 = () => 'hello world';

let m2:(x:number)=>number;
m2 = (x) => x*10;

interface iReadable{
    read():string;
}

interface iWritable{
    write(content:string):void;
}

interface iFile extends iReadable, iWritable{
    open(path:string):boolean;
}

class MyFile implements iFile{
    read():string{ return ''; };
    write(content:string){};
    open(path:string){ return true; };
}

let file:iFile = new MyFile();
file.read();

class SomeClass{ 
    static counts: number = 0;    
    constructor(){        
        SomeClass.counts++;
    }
} 

let s1 = new SomeClass();
let s2 = new SomeClass();
let s3 = new SomeClass();

console.log(SomeClass.counts);

 class classC{
     private p1:number;
     public p2:number;
     protected p3:number;
     p4:number; // public
 }

class classD extends classC{ 
    fun(): void{ }
}

let c1 = new classC();  // c1 has access to p2 and p4
let d1 = new classD();  // d1 has access to p2 and p4

interface iPoint{
    x: number;
    y:number;
}

function append(p1:iPoint, p2:iPoint){
    return {
        x: p1.x + p2.x,
        y: p1.y + p2.y
    }
}

var res = append({x:3,y:6},{x:4,y:7});
console.log(res);  // { x: 7, y: 13 }

namespace ns1{
    export class c1 {}
    export class c2 {}
    class c3 {}    
}

namespace ns2{
    export class c1 {}    
}

let ns1c1 = ns1.c1; // can access c1 and c2
let ns2c1 = ns2.c1;

let decimal: number = 6;
let hex: number = 0xf00d;
let binary: number = 0b1010;
let octal: number = 0o744;

function foo(p1:number, p2:string):boolean{
	return true;	
}

let success = foo(1, 'str');