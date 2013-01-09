var VOLCONTROL_ANIM_DURATION = 500;
var VOLCONTROL_TIMEOUT = 3000;
var VOLCONTROL_ANIM_MOVE = 100;

var VolumeControl = {
    visible: false,

    hide_timer: null,

    volume_elem: null,
    volume_handle: null,
    volume_bar: null,

    get_control: function () {
        var handle = $("<div>").addClass("hs-handle");
        var bar = $("<div>").addClass("bar");

        var ctl = $("<div>")
            .addClass("volume")
            .append(
                $("<div>")
                .addClass("hscroll")
                .append(bar)
                .append(handle)
            );

        this.volume_handle = handle;
        this.volume_bar = bar;
        this.volume_elem = ctl;

        Player.addVolumeListener(function (e) {

            VolumeControl.volume_handle.animate({
                "bottom": (10 + (e.currentTarget.volume * 90)) + "%"
            }, VOLCONTROL_ANIM_MOVE, "linear");

            VolumeControl.volume_bar.animate({
                "height": (10 + (e.currentTarget.volume * 90)) + "%"
            }, VOLCONTROL_ANIM_MOVE, "linear");

        });

        return ctl;
    },

    show: function () {
        if (this.visible) return;

        this.visible = true;

        this.volume_elem.switchClass("hidden", "", VOLCONTROL_ANIM_DURATION);

        this.setTimeout();
    },

    setTimeout: function (callback) {
        this.hide_timer = setTimeout(function () {
            VolumeControl.hide();
            if (callback !== undefined) callback();
        }, VOLCONTROL_TIMEOUT);
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
        this.volume_elem.switchClass("", "hidden", OVERLAY_ANIM_DURATION);
    },

    toggle: function () {
        if (this.visible === true) {
            this.hide();
        } else {
            this.show();
        }
    },

    setVolume: function (value, max) {
        this.resetTimeout();
        if (max == undefined) max = 1.0;
        if (value > max) value = max;
        if (value < 0) value = 0;

        console.log(value);

        Player.setVolume(value, max);
    },

    raise: function (value, max) {
        this.show();
        if (max == undefined) max = 1.0;

        var vol = Player.getVolume(max) + value;
        this.setVolume(vol, max);
    },

    lower: function (value, max) {
        this.raise(-value, max);
    }
};