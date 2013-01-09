
var GestureDetector = function(gestureChecker, handler) {
    this._history = [];
    this._windowSize = 40;
    this._gestCheck = gestureChecker;
    this._handler = handler;
}

/*
  Adds a new position to the _history, while deleting old positions
  (those who fall outside of the window)
 */
GestureDetector.prototype.add = function (pos) {
    this._history.push({
        pos: pos,
        time: Date.now()
    });

    if (this._history.length > this._windowSize) {
        this._history.shift();
    }

    var gest = this._gestCheck();
    if (gest) this._handler(gest);
}

var SwipeDetector = function (handler) {
    this.minLen = 0.5;
    this.maxHeight = 0.2;
    this.minDuration = 250;
    this.maxDuration = 1500;

    var _history = [];
    var _windowSize = 40;
    var _scanPositions = function (history,
        heightFun, dirFun, lenFun,
        minTime, maxTime) {

        var start = 0;
        for (var i = 1; i < history.length - 1; i++) {
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
    var _gestCheck = function () {
        var heightFun = function (p1, p2) { Math.abs(p2.y - p1.y) < this.maxHeight };
        var lenFun = function (p1, p2) { Math.abs(p2.y - p1.y) > this.minLen };

        var rightDir = function (p1, p2) { p2.x - p1.x > -0.01 };
        var leftDir = function (p1, p2) { p2.x - p1.x < 0.01 };

        if (_scanPositions(this._history, heightFun, rightDir, lenFun, this.minDuration, this.maxDuration)) {
            return "right";
        } else if (_scanPositions(this._history, heightFun, leftDir, lenFun, this.minDuration, this.maxDuration)) {
            return "left";
        };

        return null;
    };

    var _handler = handler;
    
    return new GestureDetector(_gestCheck, _handler);
}

SwipeDetector.prototype = new GestureDetector();