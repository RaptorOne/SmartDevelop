using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartDevelop.Model.Projecting;
using SmartDevelop.Model.CodeLanguages;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SmartDevelop.AHK.AHKv1.Projecting
{
    public class SmartCodeProjectAHK : SmartCodeProject
    {
        CodeLanguageAHKv1 _language;

        public SmartCodeProjectAHK(string name, CodeLanguage language)
            : base(name, language) {
                _language = language as CodeLanguageAHKv1;
        }


        public override void Run() {
            string output = "";
            string errOutput = "";



            Solution.ClearOutput();
            Solution.AddNewOutputLine("Running: " + this.StartUpCodeDocument.FilePath);

            if(this.StartUpCodeDocument != null) {

                // this.StartUpCodeDocument.FilePath

                if(!File.Exists(_language.Settings.InterpreterPath)) {

                    // Displays an OpenFileDialog so the user can select a Cursor.
                    var openFileDialog1 = new OpenFileDialog();
                    openFileDialog1.Filter = "Autohotkey Interpreter|*.exe";
                    openFileDialog1.Title = "Select yout Autohotkey Interpreter";

                    if(openFileDialog1.ShowDialog() == DialogResult.OK) {
                        _language.Settings.InterpreterPath = openFileDialog1.FileName;
                    }
                }

                if(!File.Exists(_language.Settings.InterpreterPath)) {
                    output = "Can't find AHK Interpreter!";
                }

                // Start the child process.
                Process p = new Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = _language.Settings.InterpreterPath;
                p.StartInfo.Arguments = "/ErrorStdOut " + this.StartUpCodeDocument.FilePath;
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                output = p.StandardOutput.ReadToEnd();
                errOutput = p.StandardError.ReadToEnd();
                p.WaitForExit();
            }

            Solution.AddNewOutputLine(errOutput);
            return;
        }

        public override bool CanRun {
            get {
                return Solution != null && StartUpCodeDocument != null;
            }
        }

    }
}
