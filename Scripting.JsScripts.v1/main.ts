import { Myfs } from './lib/myfs';

class Student {
    fullName: string;
    constructor(public firstName, public middleInitial, public lastName) {
        this.fullName = firstName + " " + middleInitial + " " + lastName;
    }
}

interface Person {
    firstName: string;
    lastName: string;
}

function greeter(person: Person) {
    return "Hello, " + person.firstName + " " + person.lastName;
}

var user = new Student("Jane", "M.", "User");

console.log(greeter(user));

console.log('ciao');

//with Node.js, after having installed package fs
//$ npm install fs
const test = require("./lib/test.js");
console.log(test);
console.log();

const test2 = require("./lib/test.js");
console.log(test2);

// require accurate-finance
var Finance = require('./lib/accurate-finance.js');
var finance = new Finance();
// To calculate Amortization
const test3 = finance.AM(20000, 7.5, 5, 0);
console.log(test3);
// => 400.76
console.log();

debugger;  // activate debugger if possible

const flagFilePath: string = 'zzz flagfile';
const flagFileValue: string = 'flag value';

const fs: Myfs = new Myfs();
fs.deleteFile(flagFilePath);
fs.appendFile(flagFilePath, flagFileValue);
var fileContent = fs.readFile(flagFilePath);
console.log(fileContent);
if (fileContent != flagFileValue) {
    throw 'content error';
}
else {
    console.log('success');
};
console.log('end');
