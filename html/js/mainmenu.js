"use strict"

var MainMenu = {
    options: [{
        "title": "TV Guide",
        "class": "tvguide",
        "fun": function () {
            Channels.show();
            inCommands = channelsCommands;
        }
    }, {
        "title": "Suggestions",
        "class": "suggestions",
        "fun": function () {
            MagicPlaylist.show();
            inCommands = magicCommands;
        }
    }, {
        "title": "Switch Off",
        "class": "turnoff",
        "fun": function () {
            if (window.App && App.exit) {
                App.exit();
            } else {
                MainMenu.resumePlay();
                Player.stop();
            }
        }
    }],

    currentOption: 0,

    menuElem: undefined,

    create: function () {
        this.menuElem = buildMenu(this.options, "mainmenu");
        $("div#menu div#main").append(this.menuElem);
        centerMenu(this);
    },

    select: selectMenu,

    accept: acceptMenu,

    show: function () {
        if (this.menuElem === undefined) {
            this.create();
            this.select(1);
        }
        $("div#menu").fadeIn("slow");
    },

    hide: function () {
        $("div#menu").hide();
    },

    nextOption: function () {
        this.select(this.currentOption + 1);
    },

    prevOption: function () {
        this.select(this.currentOption - 1);
    },

    resumePlay: function () {
        MainMenu.hide();
        inCommands = overlayCommands;
        Overlay.show();
    }
};

var menuCommands = {
    "exit": function () {
        MainMenu.resumePlay();
    },
    
    "enter": function () {
        MainMenu.accept();
    },
    
    "left": function () {
        MainMenu.prevOption();
    },
    
    "right": function () {
        MainMenu.nextOption();
    }
}