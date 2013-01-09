/**

 Notify jQuery plugin.
 Turns any element into a notify thingy

*/
(function ($) {
    $.fn.notify = function (str, delay) {
        return $(this).clearQueue().html(str).fadeIn("fast", function () {
            $(this).delay(delay).fadeOut("slow", function () {
                $(this).html("");
            });
        });
    }
})(jQuery);