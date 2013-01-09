var SpeechHandler = {
    onRecognized: function (command, recognized) {
        if (recognized) $("div#speech").notify(recognized, NOTIFY_DELAY);

        var fun = inCommands[command.toLowerCase()];
        if (fun) fun();
    }
};