using Scripting.Js.v1;
using System;
using System.Collections.Generic;

namespace BenchmarksProject
{
    public class ObjectCloning
    {
        JsScriptRunner Runner { get; }

        public ObjectCloning(JsScriptRunner runner)
        {
            if (runner is null) throw new ArgumentNullException(nameof(runner));
            Runner = runner;

            Runner.Run(new Dictionary<string, string>(Settings.MyRequireEs5.GetScript())["myrequire"]);  // execute myrequire
        }

        public void ObjectCloning_with_Stringify()
        {
            // from from https://stackoverflow.com/questions/122102/what-is-the-most-efficient-way-to-deep-clone-an-object-in-javascript
            Runner.Run(@"
//log('ObjectCloning_with_Stringify');
(function () {
// Make Host object from JSON
var sampleSimObject = {
    Numbers: [1, 2, 5.001 ],
    Dates: [new Date(Date.UTC(2019, 11, 23)), new Date(Date.UTC(2019, 11, 25)) ],
    Choices: [true, false, false, true],
    Texts: ['ciao', 'mamma guarda come mi diverto'],
    Date: new Date(Date.UTC(2020, 11, 25)),
    Text: 'sometimes',
    Number: 69,
    Choice: true
};
var sampleSimObject_cloned = JSON.parse(JSON.stringify(sampleSimObject));
sampleSimObject_cloned.Number = 8;

if (sampleSimObject.Number != 69) {throw 'error message';}
if (sampleSimObject_cloned.Number != 8) {throw 'error message';}
})();
");
        }

        #region rfdc
        public void ObjectCloning_with_rfdc()
        {
            ObjectCloning_with_rfdc__Init();
            ObjectCloning_with_rfdc__Run();
        }

        public void ObjectCloning_with_rfdc__Init()
        {
            Runner.Run(@"
var clone = require('./lib/rfdc.js')({ proto: true });
");
        }

        public void ObjectCloning_with_rfdc__Run()
        {
            // from https://medium.com/javascript-in-plain-english/how-to-deep-copy-objects-and-arrays-in-javascript-7c911359b089
            Runner.Run(@"
//log('ObjectCloning_with_rfdc');
(function () {
// Make Host object from JSON
var sampleSimObject = {
    Numbers: [1, 2, 5.001 ],
    Dates: [new Date(Date.UTC(2019, 11, 23)), new Date(Date.UTC(2019, 11, 25)) ],
    Choices: [true, false, false, true],
    Texts: ['ciao', 'mamma guarda come mi diverto'],
    Date: new Date(Date.UTC(2020, 11, 25)),
    Text: 'sometimes',
    Number: 69,
    Choice: true
};
var sampleSimObject_cloned = clone(sampleSimObject);
sampleSimObject_cloned.Number = 8;

if (sampleSimObject.Number != 69) {throw 'error message';}
if (sampleSimObject_cloned.Number != 8) {throw 'error message';}
})();
");
        }
        #endregion rfdc

        #region Lodash
        public void DONT_WORK_WITH_JINT_ObjectCloning_with_Lodash()
        {
            DONT_WORK_WITH_JINT_ObjectCloning_with_Lodash__Init();
            DONT_WORK_WITH_JINT_ObjectCloning_with_Lodash__Run();
        }

        public void DONT_WORK_WITH_JINT_ObjectCloning_with_Lodash__Init()
        {
            Runner.Run(@"
var clone = require('./lib/lodash.clonedeep.js');
");
        }

        public void DONT_WORK_WITH_JINT_ObjectCloning_with_Lodash__Run()
        {
            // from https://medium.com/javascript-in-plain-english/how-to-deep-copy-objects-and-arrays-in-javascript-7c911359b089
            Runner.Run(@"
//log('ObjectCloning_with_Lodash');
(function () {
// Make Host object from JSON
var sampleSimObject = {
    Numbers: [1, 2, 5.001 ],
    Dates: [new Date(Date.UTC(2019, 11, 23)), new Date(Date.UTC(2019, 11, 25)) ],
    Choices: [true, false, false, true],
    Texts: ['ciao', 'mamma guarda come mi diverto'],
    Date: new Date(Date.UTC(2020, 11, 25)),
    Text: 'sometimes',
    Number: 69,
    Choice: true
};
var sampleSimObject_cloned = clone(sampleSimObject);
sampleSimObject_cloned.Number = 8;

if (sampleSimObject.Number != 69) {throw 'error message';}
if (sampleSimObject_cloned.Number != 8) {throw 'error message';}
})();
");
        }
        #endregion Lodash
    }
}
