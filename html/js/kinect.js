function lerpf(v, min, max) {
    return (v * (max - min)) + min
}

function fromSkelToCanvas(joint, w, h) {
    return {
        x: (joint.x + 0.5) * w,
        y: (0.5 - joint.y) * h
    };
}



var KinectFunctions = {
    drawSkeleton: function (skeldata) {
        var canvas = $("canvas#skel");
        var w = canvas.width();
        var h = canvas.height();
        var ctx = canvas[0].getContext("2d");

        var joint;
        ctx.clearRect(0, 0, w, h);

        ctx.fillStyle = 'blue';
        $.each(skeldata, function (name, joint) {
            var point = fromSkelToCanvas(joint, w, h);
            ctx.fillRect(point.x - 5, point.y - 5, 10, 10);
        });

        ctx.strokeStyle = 'red';
        ctx.beginPath();

        joint = fromSkelToCanvas(skeldata["head"], w, h); ctx.moveTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["shoulder_center"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["spine"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["hip_center"], w, h); ctx.lineTo(joint.x, joint.y);

        joint = fromSkelToCanvas(skeldata["hip_center"], w, h); ctx.moveTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["hip_left"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["knee_left"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["ankle_left"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["foot_left"], w, h); ctx.lineTo(joint.x, joint.y);

        joint = fromSkelToCanvas(skeldata["hip_center"], w, h); ctx.moveTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["hip_right"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["knee_right"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["ankle_right"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["foot_right"], w, h); ctx.lineTo(joint.x, joint.y);

        joint = fromSkelToCanvas(skeldata["shoulder_center"], w, h); ctx.moveTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["shoulder_left"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["elbow_left"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["wrist_left"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["hand_left"], w, h); ctx.lineTo(joint.x, joint.y);

        joint = fromSkelToCanvas(skeldata["shoulder_center"], w, h); ctx.moveTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["shoulder_right"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["elbow_right"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["wrist_right"], w, h); ctx.lineTo(joint.x, joint.y);
        joint = fromSkelToCanvas(skeldata["hand_right"], w, h); ctx.lineTo(joint.x, joint.y);

        ctx.stroke();
    },

    doCursor: function (cursor, joint) {
        var npos = {
            left: (((joint.x + 0.5) / 2) * 100) + "%",
            top: (100 - (((joint.y + 0.5) / 2) * 100)) + "%"
        };

        cursor.css(npos);
    }
};

var KinectHandler = {
    /**
    * Gets called when new skeleton data arrives from the kinect
    */
    onSkeleton: function (skeldata) {
        KinectFunctions.drawSkeleton(skeldata);
        KinectFunctions.doCursor(
            $("div#cursor_r"),
            skeldata["hand_right"]
        );
        KinectFunctions.doCursor(
            $("div#cursor_l"),
            skeldata["hand_left"]
        );

        if (skeldata["hand_right"].y > skeldata["head"].y) {
            var fun = inCommands["input"];
            if (fun) fun();
        }
    },

    onGesture: function (e) {
        if (!KinectHelper.userIsFacing()) return;
        //debug(e.joint + ": " + e.gesture);

        if (e.gesture === "swipe_left" && e.joint === "hand_right") {
            debug("Swipe Left");
            var fun = inCommands["left"];
            if (fun) fun();
        } else if (e.gesture === "swipe_right" && e.joint === "hand_left") {
            debug("Swipe Right");
            var fun = inCommands["right"];
            if (fun) fun();
        }

    }
};