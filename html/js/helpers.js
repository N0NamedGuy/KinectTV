Math.lerp = function(v, min, max) {
    return (v * (max - min)) + min
}

Math.sq = function(a) {
    return a * a;
}

Math.distance = function(a, b) {
    return Math.sqrt(
        Math.sq(a.x - b.x) +
        Math.sq(a.y - b.y) +
        Math.sq(a.z - b.z)
    );
}