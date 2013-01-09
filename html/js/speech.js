var SpeechHandler = {
    onRecognized: function (command, recognized, confidence) {
        if (recognized)
            $("div#speech").notify(
                recognized + " (" + confidence + ")",
                NOTIFY_DELAY
             );

        var fun = inCommands[command.toLowerCase()];
        if (fun) fun();
    }
};