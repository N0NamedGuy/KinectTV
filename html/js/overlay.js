"use strict"
var OVERLAY_ANIM_DURATION = 500;
var OVERLAY_TIMEOUT = 3000;

var Overlay = {
    visible: false,

    hide_timer: null,

    cur_show: "rtp1",
    cur_network: 0,

    create: function () {
        $("#overlay").append(VolumeControl.get_control());
        Player.addVolumeListener(function (e) {
            var vol = e.currentTarget.volume * 100;
            $("#overlay #vol").text(Math.round(vol) + "%");
        });
    },

    show: function () {
        if (this.visible) return;
        this.visible = true;

        $("#overlay .panel")
            .removeClass("pop")
            .switchClass("hidden", "", OVERLAY_ANIM_DURATION);

        this.setTimeout();
    },

    setTimeout: function () {
        this.hide_timer = setTimeout(function () {
            Overlay.hide();
        }, OVERLAY_TIMEOUT);
    },

    clearTimeout: function () {
        clearTimeout(this.hide_timer);
    },

    resetTimeout: function () {
        this.clearTimeout();
        this.setTimeout();
    },

    hide: function () {
        this.clearTimeout();
        if (!this.visible) return;

        this.visible = false;
        $("#overlay").children()
            .switchClass("", "hidden", OVERLAY_ANIM_DURATION);
    },

    toggle: function () {
        if (this.visible === true) {
            this.hide();
        } else {
            this.show();
        }
    },

    popSide: function (right) {
        var str = "#" + (right ? "right" : "left");

        this.resetTimeout();

        $("#overlay " + str)
            .toggleClass("pop", OVERLAY_ANIM_DURATION)
            .delay(OVERLAY_ANIM_DURATION)
            .toggleClass("pop", OVERLAY_ANIM_DURATION);

    },

    setShow: function (network_id, show_id) {
        if (network_id < 0 || network_id >= CHANNELS.length) return;

        var network = CHANNELS[network_id];
        // This is ugly
        // if network_id is an integer, find the corresponding key
        if (network == undefined) {
            var networks = Object.keys(CHANNELS);
            network_id = networks[network_id];
            network = CHANNELS[network_id];

            console.log(networks, network);
        }

        var show = network.schedule[show_id];
        console.log(network, show);

        this.cur_network = network.number;
        this.cur_show = show_id;

        $("#cur_show").html(network.name);
        $("#cur_network").html(show.name);
        $("#time_total").html("0:00:00");

        Player.load(network.stream);
        Player.fadeIn();
        Player.play();
    },

    nextChannel: function () {
        Overlay.popSide(true);
        this.setShow(this.cur_network + 1, 0);
    },

    prevChannel: function () {
        Overlay.popSide(false);
        this.setShow(this.cur_network - 1, 0);
    }
};

var overlayCommands = {
    "exit": function () {
        return; 
        if (window.App && App.exit) {
            App.exit();
        } else {
            MainMenu.resumePlay();
            Player.stop();
        }
    },

    "input": function () {
        if (Overlay.visible === true) {
            Overlay.hide();
            MainMenu.show();
            inCommands = menuCommands;
        } else {
            Overlay.show();
        }
    },

    "down": function () {
        if (Overlay.visible === true) {
            VolumeControl.lower(10, 100);
            Overlay.resetTimeout();
        }
    },

    "up": function () {
        if (Overlay.visible === true) {
            VolumeControl.raise(10, 100);
            Overlay.resetTimeout();
        }
    },

    "right": function () {
        if (Overlay.visible === true) {
            Overlay.nextChannel();
            Overlay.resetTimeout();
        }
    },

    "left": function () {
        if (Overlay.visible === true) {
            Overlay.prevChannel();
            Overlay.resetTimeout();
        }
    }
}