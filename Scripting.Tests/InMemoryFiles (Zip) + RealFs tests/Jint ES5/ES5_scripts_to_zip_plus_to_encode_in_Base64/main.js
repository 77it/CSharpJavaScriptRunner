// converted with Babeljs, es2015 settings

"use strict";

debugger;

var test = require("./lib/test.js");

var myfs = require("myfs.js");

var fs = new myfs.Myfs();
fs.deleteFile('filename');
fs.appendFile('filename', test);
var fileContent = fs.readFile('filename');

if (fileContent != test) {
    throw 'content error';
}

;