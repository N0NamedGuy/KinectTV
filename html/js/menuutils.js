"use strict"
var ANIMATION_TIME = 200;
function buildMenu(options, menuClass, buildFun) {
    var lst = $("<ul>").addClass(menuClass);
    var i = 0;
    
    $.each(options, function (k, v) {
        var li =
            $("<li>").append(
                $("<a>").attr("href", "#").text(v.title)
            ).attr("id", "option" + i).click(v.fun)
            .addClass(v.class ? v.class : "")
            .addClass("unselected");
        
        if (buildFun !== undefined) {
            buildFun(li, v, i);
        }
        
        lst.append(li);
        i++;
    });

    return lst;
}

function selectMenu(option, ctx) {
    if (ctx === undefined) {
        ctx = this;
    }
    var lastOpt = ctx.currentOption;
    
    if (option === undefined) {
        option = lastOpt;
    }

    var len = ctx.options.length;
    var opt = option;
    
    if (opt >= len || opt < 0) return;

    ctx.menuElem.children("#option" + ctx.currentOption)
        .switchClass("selected", "unselected", ANIMATION_TIME);
    ctx.menuElem.children("#option" + opt)
        .switchClass("unselected", "selected", ANIMATION_TIME);

    var parent = ctx.menuElem.parent();
    var diff = lastOpt - option;
    var selW = ctx.menuElem.children("#option" + opt).outerWidth(true);
    var menuW = Math.max(selW * (len + 1), parent.width());

    ctx.menuElem.animate({
        "left": "+=" + diff * selW
    }, ANIMATION_TIME);

    ctx.currentOption = opt;
}

function acceptMenu(option, ctx) {
    var opt;
    if (ctx === undefined) {
        ctx = this;
    }

    if (option !== undefined) {
        opt = option; 
    } else {
        opt = ctx.currentOption;
    }
    
    
    var fun = ctx.options[opt]["fun"];
    if (fun !== undefined) fun();
}

function centerMenu(ctx) {
    if (ctx === undefined) ctx = this;
    var nOpts = ctx.options.length;
    
    var parent = ctx.menuElem.parent();
    var parentW = parent.parent().outerWidth(true);

    var selW = ctx.menuElem.children("#option0").outerWidth(true);
    var menuW = selW * (nOpts + 1);
    parent.css("width", Math.max(menuW, parentW));
    ctx.menuElem.css("left", (parentW / 2) - selW);
}