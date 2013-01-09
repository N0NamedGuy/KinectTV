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

    var canvas = $("canvas#skel");
    KinectHandler.ctx = canvas[0].getContext("2d");
    KinectHandler.cw = canvas.width();
    KinectHandler.ch = canvas.height();
    
    Overlay.create();

    Overlay.hide();
    Overlay.toggle();
    inCommands = overlayCommands;

    Player.fadeIn();
    Player.play();
    Player.setVolume(50, 100);
});