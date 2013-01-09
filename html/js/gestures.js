
function GestureDetector(gestureChecker, handler) {
    var _history = [];
    var _windowSize = 40;
    var _gestCheck = gestureChecker;
    var _handler = handler;
}

/*
  Adds a new position to the _history, while deleting old positions
  (those who fall outside of the window)
 */
GestureDetector.prototype.addJoint = function (pos) {
    this._history.push({
        pos: pos,
        time: Date.now()
    });

    if (this._history.length > this._windowSize) {
        this._history.shift();
    }

    if (this._gestCheck()) this._handler();
}

function SwipeDetector(handler) {
    var minLen = 0.5;
    var maxHeight = 0.2;
    var minDuration = 250;
    var maxDuration = 1500;

    function _scanPositions(history,
        heightFun, dirFun, lenFun,
        minTime, maxTime) {
        
        var start = 0;
        for (var i = 1; i < history.length - 1, i++) {
            if (
                heightFun(history[0].pos, history[i].pos) &&
                dirFun(history[0].pos, history[i + 1].pos)
            ) start = i;

            if (!lenFun(history[i].pos, history[start].pos)) {
                var time = history[i].time - history[start].pos;
                if (time >= minTime && minTime <= maxTime) return true;
            }
        }

        return false;
    };

    return new GestureDetector(
        function () {;},
        handler
    );
}
