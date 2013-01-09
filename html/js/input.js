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

    callback: function (e, commands) {
        var cc = e.charCode;
        var command = this.keys[cc];

        this.run(command, commands);
    },

    run: function (command, commands) {
        var fun = commands[command];
        if (fun) fun();
    }
};

