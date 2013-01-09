"use strict"

var MagicPlaylist = {
    options: [{
        "title": "Creative Commons",
        "subtitle": "An intro to creative commons",
        "path": "video/cc.ogv",
        "thumbnail": "video/cc.png"
    }, {
        "title": "Gizmo",
        "subtitle": "A simple gizmo",
        "path": "video/gizmo.webm",
        "thumbnail": "video/gizmo.png"
    }, {
        "title": "Gizmo",
        "subtitle": "A simple gizmo",
        "path": "video/gizmo.webm",
        "thumbnail": "video/gizmo.png"
    }],

    currentOption: 0,

    menuElem: undefined,

    create: function () {
        this.menuElem = buildMenu(this.options, "magicplaylist",
            function (li, data, index) {
                li.append($("<p>").text(data.subtitle))
                    .css("background-image", "url(" + data.thumbnail + ")");
            });

        $("div#magic").append(this.menuElem);
        centerMenu(this);
    },

    select: selectMenu,

    accept: function (option) {
        if (option === undefined) {
            option = this.currentOption;
        }

        acceptMenu(option, this);
        Player.load(this.options[option].path);
        Player.play();
        MagicPlaylist.hide();
        MainMenu.hide();
        inCommands = overlayCommands;
    },

    show: function () {
        if (this.menuElem === undefined) {
            this.create();
        }

        this.select();
        $("div#magic").fadeIn("slow");
    },

    hide: function () {
        $("div#magic").fadeOut("fast");
    },

    nextOption: function () {
        this.select(this.currentOption + 1);
    },

    prevOption: function () {
        this.select(this.currentOption - 1);
    }

};

var magicCommands = {
    "exit": function () {
        MagicPlaylist.hide();
        inCommands = menuCommands;
    },
    
    "enter": function () {
        MagicPlaylist.accept();
    },
    
    "left": function () {
        MagicPlaylist.prevOption();
    },
    
    "right": function () {
        MagicPlaylist.nextOption();
    }
}