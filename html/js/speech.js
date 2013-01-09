var SpeechHandler = {
    onRecognized: function (command, recognized, confidence) {
        if (KinectHelper.userIsFacing()) {

            $("div#speech").notify(
                recognized + " (" + confidence.toFixed(2) + ")",
                NOTIFY_DELAY
            );

            Input.run(command.toLowerCase(), inCommands);
        }
    }
};