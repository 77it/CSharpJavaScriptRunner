// converted with Babeljs, es2015 settings
// + custom edits

"use strict";

// this code must not be called/referenced directly by startup program (as main.js)
// this code is not required by node.js
// this code is required by C#, that must run it BEFORE any calling any other JavaScript code using CommonJs/Node.js modules
// ispired to https://github.com/stefano77it/require-example (forked from https://github.com/musikele/require-example/ )
// inspiration code published and commented in https://michelenasti.com/2018/10/02/let-s-write-a-simple-version-of-the-require-function.html 
function setRequire() {
    if (typeof require !== 'undefined') {
        // if require is defined, return it
        return require;
    } else {
        var requiredFileIsALibrary = function requiredFileIsALibrary(name) {
            name = name.trim();

            if (name.startsWith('./')) {
                return false;
            }

            if (name.startsWith('../')) {
                return false;
            }

            if (name.startsWith('.\\')) {
                return false;
            }

            if (name.startsWith('..\\')) {
                return false;
            }

            return true;
        };

        var requiredFileIsMissingExtension = function requiredFileIsMissingExtension(name) {
            name = name.trim().toLowerCase();

            if (name.endsWith('.js')) {
                return false;
            }

            return true;
        };

        // if require is not defined, build it
        var libraryPath = './lib/';

        if (typeof _scriptingContext === 'undefined') {
            throw '_scriptingContext not defined';
        }

        var readFile = function readFile(name) {
            return _scriptingContext.ReadFile(name);
        };

        var requireCache = Object.create(null);
        return function (name) {
            //console.log(`Evaluating file ${name}`);
            if (requiredFileIsALibrary(name)) {
                name = libraryPath + name;
            }

            if (requiredFileIsMissingExtension(name)) {
                name = name + '.js';
            }

            if (!(name in requireCache)) {
                //console.log(`${name} is not in cache; reading from disk`);
                var code = readFile(name);
                var module = {
                    exports: {}
                };
                requireCache[name] = module;
                var wrapper = Function('require, exports, module', code);
                wrapper(require, module.exports, module);
            } //console.log(`${name} is in cache. Returning it...`);


            return requireCache[name].exports;
        };
    }
}

var require = setRequire();