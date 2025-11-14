document.addEventListener('contextmenu', function (e) {
    e.preventDefault();
});

document.addEventListener('keydown', function (e) {
    if (e.keyCode == 123 ||
        (e.ctrlKey && e.shiftKey && e.keyCode == 73) ||  
        (e.ctrlKey && e.shiftKey && e.keyCode == 74) ||  
        (e.ctrlKey && e.shiftKey && e.keyCode == 67) ||  
        (e.ctrlKey && e.keyCode == 85) ||                
        (e.ctrlKey && e.keyCode == 80)) {                
        e.preventDefault();
        return false;
    }
});

setInterval(function () {
    debugger;
}, 1000);

var devtools = function () { };
devtools.toString = function () {
    if (!this.opened) {
        this.opened = true;
    }
};
console.log('%c', devtools);

document.getElementById('detectDevTools').addEventListener('click', function () {
});

document.getElementById('viewSource').addEventListener('click', function () {
});

function detectDevToolsOpen() {
    var widthThreshold = window.outerWidth - window.innerWidth > 160;
    var heightThreshold = window.outerHeight - window.innerHeight > 160;

    if (widthThreshold || heightThreshold) {
        window.location.reload();
    }
}

setInterval(detectDevToolsOpen, 1000);
 