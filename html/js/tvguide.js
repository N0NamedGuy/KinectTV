var TVGUIDE_ANIM_SLIDE = 500;
var TVGUIDE_ANIM_TIME = 100;
var	pixels_per_minute = 3;
var TVGuide = {
    timeline_elem: null,
    tvguide_elem: null,
    info_elem: null,
    schedule: null,
    cur_chan: null,
    cur_show: 0,

    info: {
        name_elem: null,
        start_elem: null,
        end_elem: null,
        thumb_elem: null,
    },

    create_timeline: function () {
        var showlist = $("<ul>").attr("class", "shows");
        var times = $("<div>").addClass("times");
        var chan = this.cur_chan;
        this.schedule = CHANNELS[chan].schedule

        $.each(this.schedule, function (i, show) {
            var showW = show.duration * pixels_per_minute;
            showlist.append(
                $("<li>")
                .text(show.name)
                .css("background-image", "url(" + show.banner + ")")
                .css("width", showW)
                .addClass("unselected")
            );
            times.append($("<span>").addClass("time").css("width", showW).text(show.start));
        });

        var timeline_elem = $("<div>")
            .attr("id", "timeline")
            .append(
                times
            ).append(
                $("<div>").addClass("shows").append(showlist)
            );

        var showinfo_elem = $("<div>").addClass("info");

        var tvguide_elem = $("div#tvguide")
            .empty()
            .append(
                $("<h1>")
                .attr("id", "network")
                .text(CHANNELS[chan].name + " - TV Guide")
            ).append(timeline_elem)
            .append(showinfo_elem);

        //Find correct size for the timeline
        var timeline_w = 0;
        $.each($("ul.shows li"), function (i, li) {
            timeline_w += $(li).outerWidth(true);
        });
        showlist.width(timeline_w);
        times.width(timeline_w);

        this.timeline_elem = timeline_elem;
        this.tvguide_elem = tvguide_elem;
    },

    create_info: function () {
        this.info.thumb_elem = $("<span>").addClass("thumb").attr("id", "show_thumb")
        this.info.start_elem = $("<span>").attr("id", "show_start");
        this.info.end_elem = $("<span>").attr("id", "show_end");
        this.info.name_elem = $("<h2>").attr("id", "show_name");

        var info_elem = $("<div>").addClass("info")
            .append(this.info.thumb_elem)
            .append(
                $("<div>")
                .addClass("desc")
                .append(this.info.name_elem)
                .append(
                    $("<span>")
                    .addClass("time")
                    .append(this.info.start_elem)
                    .append(" - ")
                    .append(this.info.end_elem)
                )
            );

        this.info_elem = info_elem;

        $("div#tvguide").append(info_elem);
    },

    update_info: function () {
        var show = CHANNELS[this.cur_chan].schedule[this.cur_show];

        this.info.thumb_elem.css("background-image", "url(" + show.banner + ")");
        this.info.name_elem.text(show.name);
        this.info.start_elem.text(show.start);
        this.info.end_elem.text(show.end);
    },

    show: function (chan) {
        this.cur_chan = chan;
        this.create_timeline();
        this.create_info();
        this.tvguide_elem.fadeIn("fast");
        this.select(0);
    },

    hide: function () {
        this.tvguide_elem.fadeOut("slow");
    },

    select: function (show_i) {
        //Out of bounds protections
        if (show_i < 0 || show_i >= this.schedule.length) return;

        var menu = this.timeline_elem;
        var menuW = menu.outerWidth(true);

        var selShow = $($($($(menu.children()[1]).children()[0]).children())[show_i]);
        var selShowOld = $($($($(menu.children()[1]).children()[0]).children())[this.cur_show]);
        var selShowW = selShow.outerWidth(true);

        //selShow.css("background-color","rgba(23,23,233,0.8)");

        var newPos = ((menuW - selShowW) / 2) - selShow.position().left;
        menu.animate({
            "left": newPos
        }, ANIMATION_TIME, "linear");

        //Aqui falta meter tipo mexer o actual ligeiramente para baixo (para mostrar que
        //  esta selectionado e mexer o antigo seleccionado de volta pa posicao normal)
        selShowOld.switchClass("selected", "unselected", ANIMATION_TIME);
        selShow.switchClass("unselected", "selected", ANIMATION_TIME);

        this.cur_show = show_i;

        this.update_info();
    },

    slideLeft: function (chan) {
        this.select(this.cur_show - 1);
    },

    slideRight: function (chan) {
        this.select(this.cur_show + 1);
    },

    enter: function () {
        Overlay.setShow(this.cur_chan, this.cur_show);
        Overlay.show();
        TVGuide.hide();
        inCommands = overlayCommands;
    }
}

var tvguideCommands = {
    "exit": function () {
        TVGuide.hide();
        MainMenu.show();
        inCommands = menuCommands;
    },

    "enter": function () {
        TVGuide.enter();
    },

    "left": function () {
        TVGuide.slideLeft();
    },

    "right": function () {
        TVGuide.slideRight();
    }
}