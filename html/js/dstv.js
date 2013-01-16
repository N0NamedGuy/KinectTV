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

    // Awesomium bug
    // Global objects are not avaiable at this stage
    window.setTimeout(function () {
        KinectHelper.linearTrack("hand_right", "left");
        KinectHelper.linearTrack("hand_right", "down");
        KinectHelper.linearTrack("hand_left", "right");
    }, 1000);

    Overlay.create();

    Overlay.hide();
    Overlay.toggle();
    inCommands = overlayCommands;

    //Player.fadeIn();
    //Player.play();
    //Player.setVolume(50, 100);
});


var mouseSwipeDetect = new SwipeDetector(function (direction) {
    console.log("Mouse swipe: " + direction);
});
/*
$(document).mousemove(function (e) {
    var p = {
        x: Math.lerp((e.pageX / $(document).width()), -1, 1),
        y: Math.lerp((e.pageY / $(document).height()), -1, 1) };
    //debug(p.x + " " + p.y);
    mouseSwipeDetect.add(p);
});
*/