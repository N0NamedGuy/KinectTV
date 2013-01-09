using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.IO;

using Awesomium.Core;

namespace KinectTV
{
    class SpeechHelper
    {
        SpeechRecognitionEngine reco;
        IWebView wv;

        private JSObject getJSSpeechHandler()
        {
            JSValue obj = wv.ExecuteJavascriptWithResult("SpeechHandler");
            return !obj.IsObject ? null : (JSObject)obj;
        }

        private Choices getCommands()
        {
            var commands = new Choices();
            commands.Add(new SemanticResultValue("left", "left"));
            commands.Add(new SemanticResultValue("previous", "left"));

            commands.Add(new SemanticResultValue("right", "right"));
            commands.Add(new SemanticResultValue("next", "right"));

            commands.Add(new SemanticResultValue("enter", "enter"));
            commands.Add(new SemanticResultValue("accept", "enter"));
            commands.Add(new SemanticResultValue("ok", "enter"));
            commands.Add(new SemanticResultValue("go", "enter"));
            commands.Add(new SemanticResultValue("confirm", "enter"));

            commands.Add(new SemanticResultValue("quit", "exit"));
            commands.Add(new SemanticResultValue("exit", "exit"));
            commands.Add(new SemanticResultValue("back", "exit"));
            commands.Add(new SemanticResultValue("cancel", "exit"));
            commands.Add(new SemanticResultValue("return", "exit"));

            commands.Add(new SemanticResultValue("tv", "input"));
            commands.Add(new SemanticResultValue("television", "input"));
            
            commands.Add(new SemanticResultValue("up", "up"));
            commands.Add(new SemanticResultValue("more", "up"));
            commands.Add(new SemanticResultValue("higher", "up"));

            commands.Add(new SemanticResultValue("down", "down"));
            commands.Add(new SemanticResultValue("less", "down"));
            commands.Add(new SemanticResultValue("lower", "down"));

            return commands;
        }

        public SpeechHelper(IWebView wv)
        {
            this.wv = wv;

            reco = new SpeechRecognitionEngine();

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(getCommands());

            Grammar g = new Grammar(gb);
            reco.LoadGrammar(g);

            reco.SpeechRecognized += reco_SpeechRecognized;
            reco.SetInputToDefaultAudioDevice();
            reco.RecognizeAsync(RecognizeMode.Multiple);

        }
        
        void reco_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            JSObject speechHandler = getJSSpeechHandler();
            if (speechHandler == null) return;

            string command = e.Result.Semantics.Value.ToString();
            
            if (wv.IsDocumentReady)
                speechHandler.Invoke(
                    "onRecognized",
                    command,
                    e.Result.Text, e.Result.Confidence
                );
        }
    }
}
