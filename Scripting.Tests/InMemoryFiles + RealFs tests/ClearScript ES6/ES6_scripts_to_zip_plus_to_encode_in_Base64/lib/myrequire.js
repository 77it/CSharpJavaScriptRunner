// this code must not be called/referenced directly by startup program (as main.js)
// this code is not required by node.js
// this code is required by C#, that must run it BEFORE any calling any other JavaScript code using CommonJs/Node.js modules

// ispired to https://github.com/stefano77it/require-example (forked from https://github.com/musikele/require-example/ )
// inspiration code published and commented in https://michelenasti.com/2018/10/02/let-s-write-a-simple-version-of-the-require-function.html 

function setRequire() {
    if (typeof require !== 'undefined') {  // if require is defined, return it
        return require;
    }
    else {  // if require is not defined, build it
        const libraryPath = './lib/';

        if (typeof _scriptingContext === 'undefined') { throw '_scriptingContext not defined'; }

        const readFile = function (name) { return _scriptingContext.ReadFile(name) };

        const requireCache = Object.create(null);

        return function (name) {
            //console.log(`Evaluating file ${name}`);
            if (requiredFileIsALibrary(name)) { name = libraryPath + name; }
            if (requiredFileIsMissingExtension(name)) { name = name + '.js'; }
            if (!(name in requireCache)) {
                //console.log(`${name} is not in cache; reading from disk`);
                let code = readFile(name);
                let module = { exports: {} };
                requireCache[name] = module;
                let wrapper = Function('require, exports, module', code);
                wrapper(require, module.exports, module);
            }

            //console.log(`${name} is in cache. Returning it...`);
            return requireCache[name].exports;
        }

        function requiredFileIsALibrary(name) {
            name = name.trim();
            if (name.startsWith('./')) { return false; }
            if (name.startsWith('../')) { return false; }
            if (name.startsWith('.\\')) { return false; }
            if (name.startsWith('..\\')) { return false; }
            return true;
        }

        function requiredFileIsMissingExtension(name) {
            name = name.trim().toLowerCase();
            if (name.endsWith('.js')) { return false; }
            return true;
        }
    }
}

var require = setRequire();
