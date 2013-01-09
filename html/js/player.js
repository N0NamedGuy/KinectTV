"use strict"
var Player = {
    videoElem: {},
    
    timeListener: function (time) {
        $("#time_elapsed").text(time);
    },
    
    listeners: {},

    _newVideoElement: function () {
        var elem = $("<video>")
            .attr("width", 640)
            .attr("height", 480);
            
        elem[0].addEventListener("timeupdate", function(e) {
            var sec_numb = Math.floor(this.currentTime);
            var hours   = Math.floor(sec_numb / 3600);
            var minutes = Math.floor((sec_numb - (hours * 3600)) / 60);
            var seconds = sec_numb - (hours * 3600) - (minutes * 60);

            if (hours   < 10) {hours   = "0"+hours;}
            if (minutes < 10) {minutes = "0"+minutes;}
            if (seconds < 10) {seconds = "0"+seconds;}
            var time    = hours+':'+minutes+':'+seconds;

            Player.timeListener(time);
        });
        
        $.each(this.listeners, function (ev, list) {
            $.each(list, function (i, l) {
                elem[0].addEventListener(ev, function(e) {
                    l(e);
                });
            });
        });

        
        return elem;
    },

    addVolumeListener: function (fun) {
        var list = this.listeners["volumechange"];
        if (list == undefined) {
            this.listeners["volumechange"] = [fun];
        } else {
            list.push(fun);
        }

        if (this.videoElem[0] !== undefined) {
            this.videoElem[0].addEventListener("volumechange", fun);
        }
    },
    
    load: function (url, selector) {
        if (selector === undefined) {
            selector = $("div#player");
        }
        selector.empty();
        
        var newVideoElem = this._newVideoElement();
        newVideoElem.css("display", "none");
        
        this.videoElem = newVideoElem;
        selector.append(this.videoElem);
        this.videoElem.attr("src", url); 
    },
    
    play: function () {
        if (this.videoElem !== undefined) {
            this.fadeIn();
            this.videoElem[0].play();
        }
    },
    
    pause: function () {
        if (this.videoElem !== undefined) {
            this.videoElem[0].pause();
        }
    },
    
    playPause: function () {
        if (this.videoElem !== undefined) {
            var vid = this.videoElem[0];
            if (vid.paused) {
                vid.play();
            } else {
                vid.pause();
            }
        }
    },
    
    stop: function() {
        if (this.videoElem !== undefined) {
            var video = this.videoElem[0];
            video.pause();
            video.currentTime = 0;
        }
    },
    
    setVolume: function (volume, max) {
        if (this.videoElem !== undefined) {
            this.videoElem[0].volume =
                (max == undefined ? volume : volume / max);
        }
    },

    getVolume: function (max) {
        if (this.videoElem !== undefined) {
            var vol = this.videoElem[0].volume;
            return max == undefined ? vol : vol * max;
        }
    },
    
    fadeIn: function (speed, callback) {
        if (this.videoElem !== undefined) {
            this.videoElem.fadeIn(speed, callback);
        }
    },
    
    fadeOut: function (speed, callback) {
        if (this.videoElem !== undefined) {
            this.videoElem.fadeOut(speed, callback);
        }
    }
};

function _showMenu() {
    Player.fadeIn("fast", function () {
        Player.playPause();
        MainMenu.show();
        inCommands = menuCommands;
    });
};

var playerCommands = {
    "enter": _showMenu,    
    "exit": _showMenu
};