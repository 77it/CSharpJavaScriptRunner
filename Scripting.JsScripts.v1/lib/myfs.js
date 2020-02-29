"use strict";
// implementation of fs functions using node.js or _scriptingContext object
Object.defineProperty(exports, "__esModule", { value: true });
class Myfs {
    constructor() {
        this.isNodeDefined = false;
        this.nodeFs; // contains node fs, if 'isNodeDefined' == true
        this.isScriptingContextDefined = false;
        // try to use ClearScript fs functions and set flag
        if (!(typeof _scriptingContext === 'undefined')) {
            this.isScriptingContextDefined = true;
        }
        else {
            // try to use node.js fs functions and set flag
            try {
                this.nodeFs = require('fs'); // if it not goes in error, the next instruction will be executed
                this.isNodeDefined = true;
            }
            catch (error) { }
            ; // do nothing, continue to the next test
        }
        ;
        if (!this.isNodeDefined && !this.isScriptingContextDefined) {
            throw 'both node.js and _scriptingContext are not defined';
        }
        ;
    }
    readFile(path) {
        if (this.isNodeDefined) {
            return this.nodeFs.readFileSync(path, 'utf8'); // see https://nodejs.org/api/fs.html#fs_fs_readfilesync_path_options
        }
        ;
        if (this.isScriptingContextDefined) {
            return _scriptingContext.ReadFile(path);
        }
        ;
    }
    appendFile(path, contents) {
        if (this.isNodeDefined) {
            this.nodeFs.appendFileSync(path, contents); // see https://nodejs.org/api/fs.html#fs_fs_appendfilesync_path_data_options
        }
        ;
        if (this.isScriptingContextDefined) {
            _scriptingContext.AppendFile(path, contents);
        }
        ;
    }
    deleteFile(path) {
        if (this.isNodeDefined) {
            if (this.nodeFs.existsSync(path)) { // see https://nodejs.org/api/fs.html#fs_fs_existssync_path
                this.nodeFs.unlinkSync(path); // see https://nodejs.org/api/fs.html#fs_fs_unlinksync_path
            }
        }
        ;
        if (this.isScriptingContextDefined) {
            if (_scriptingContext.Exists(path)) {
                _scriptingContext.DeleteFile(path);
            }
        }
        ;
    }
}
exports.Myfs = Myfs;
// UNUSED HYPOTHESIS OF MyFs as a function, intead of a class. inspired to the typescript conversion of the class in ES5
/*
var MyFs = (function () {
    // calls _scriptingContext.ReadFile
    // 'option' parameter is unused, provided here only for compatibility to node.js method

    var isNodeDefined = false;
    var nodeFs;  // contains node fs, if 'isNodeDefined' == true
    var isClearScriptDefined = false;

    // try to use node.js fs functions and set flag
    try {
        nodeFs = require('fs');  // if it not goes in error, the next instruction will be executed
        isNodeDefined = true;
    }
    catch (error) { };  // do nothing, continue to the next test

    // try to use ClearScript fs functions and set flag
    if (typeof _scriptingContext === 'undefined') {
        isClearScriptDefined = true;
    };

    if (!isNodeDefined && !isClearScriptDefined) { throw 'both node.js and _scriptingContext are not defined'; };

    myfs.prototype.readFile function (path) {
        if (isNodeDefined) {
            return nodeFs.readFileSync(path, 'utf8');  // see https://nodejs.org/api/fs.html#fs_fs_readfilesync_path_options
        };

        if (isClearScriptDefined) {
            return _scriptingContext.ReadFile(path);
        };
    }

    myfs.prototype.appendFile function (path, contents) {
        if (isNodeDefined) {
            return nodeFs.appendFileSync(path, contents);  // see https://nodejs.org/api/fs.html#fs_fs_appendfilesync_path_data_options
        };

        if (isClearScriptDefined) {
            return _scriptingContext.AppendFile(path, contents);
        };
    }

    return myfs;
}());

module.exports = Myfs
*/ 
//# sourceMappingURL=myfs.js.map