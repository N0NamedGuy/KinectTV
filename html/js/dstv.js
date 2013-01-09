"use strict"
var inCommands = {};

var NOTIFY_DELAY = 3000;

function debug(str) {
    $("div#debug").text(str);
}

function notify(str) {
    $("div#notification").notify(str, NOTIFY_DELAY);
}

$(document).keypress(function (e, h) {
    Input.callback(e, inCommands);
    e.preventDefault();
});

$(document).ready(function () {
    /*Player.load("video/cc.ogv");
    Player.fadeIn();
    MainMenu.show();
    inCommands = menuCommands;*/

    Overlay.create();

    Overlay.hide();
    Overlay.toggle();
    inCommands = overlayCommands;

    //Player.fadeIn();
    //Player.play();
    //Player.setVolume(50, 100);
});


var mouseSwipeDetect = new SwipeDetector(function (direction) {
    debug("Mouse swipe: " + direction);
});

$(document).mousemove(function (e) {
    var p = { x: e.pageX, y: e.pageY };
    mouseSwipeDetect.add(p);
}); 