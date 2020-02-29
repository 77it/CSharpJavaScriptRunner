debugger;

const test = require("./lib/test.js");

const myfs = require("myfs.js");
const fs = new myfs.Myfs();

fs.appendFile('filename', test);
var fileContent = fs.readFile('filename');
if (fileContent != test) {
    throw 'content error';
};
