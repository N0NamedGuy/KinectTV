"use strict"

var Channels = {
    options: [],

    currentOption: 0,

    menuElem: undefined,

    loadChannels: function () {
        // TODO: Load from an AJAX source
        this.options = [];
        $.each(CHANNELS, function (k, v) {
            Channels.options.push({
                "title": v.name,
                "thumbnail": v.banner,
                "chan_id": k,
				"schedule": v.schedule
            });
        });
    },

    create: function () {
        this.loadChannels();

        this.menuElem = buildMenu(this.options, "channels",
            function (li, data, index) {
                li.append($("<p>").text(data.subtitle))
                    .css("background-image", "url(" + data.thumbnail + ")");
            });

        $("div#channels").append(this.menuElem);
        centerMenu(this);
    },

    select: selectMenu,

    accept: function (option) {
        if (!option) option = this.currentOption;
        MainMenu.hide();
        this.hide();
        TVGuide.show(this.options[option].chan_id);
        inCommands = tvguideCommands;
    },

    show: function () {
        if (this.menuElem === undefined) {
            this.create();
        }

        this.select();
        $("div#channels").fadeIn("slow");
    },

    hide: function () {
        $("div#channels").fadeOut("fast");
    },

    nextOption: function () {
        this.select(this.currentOption + 1);
    },

    prevOption: function () {
        this.select(this.currentOption - 1);
    }

};

var channelsCommands = {
    "exit": function () {
        Channels.hide();
        inCommands = menuCommands;
    },
    
    "enter": function () {
        Channels.accept();
    },
    
    "left": function () {
        Channels.prevOption();
    },
    
    "right": function () {
        Channels.nextOption();
    }
}