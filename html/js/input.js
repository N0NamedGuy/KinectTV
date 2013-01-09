"use strict"
var Input = {
    keys: {
        32: "enter",   // Space
        119: "up",      // w
        115: "down",    // s
        97: "left",    // a   
        100: "right",   // d
        113: "exit",    // q
        105: "input"     // i
    },

    callback: function (e, comands) {
        var cc = e.charCode;
        var key = this.keys[cc];

        var fun = comands[key];
        if (fun) fun();
    }
};

