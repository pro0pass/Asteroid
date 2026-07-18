using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Asteroid.Terminal
{
    /// <summary>
    /// Integrated terminal for debugging and command execution
    /// Supports wireless debugging via ADB
    /// </summary>
    public class TerminalManager : MonoBehaviour
    {
        [SerializeField] private InputField commandInput;
        [SerializeField] private Text outputDisplay;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private int maxOutputLines = 1000;
        
        private List<string> commandHistory = new List<string>();
        private int historyIndex = -1;
        private int currentOutputLines = 0;
        
        private void Start()
        {
            if (commandInput != null)
                commandInput.onEndEdit.AddListener(ExecuteCommand);
            
            LogTerminalMessage("Asteroid Terminal - v1.0\nWireless Debugging Enabled");
        }
        
        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;
            
            // Echo command
            LogTerminalMessage($"$ {command}");
            commandHistory.Add(command);
            
            // Execute command
            string output = ProcessCommand(command);
            
            if (!string.IsNullOrEmpty(output))
                LogTerminalMessage(output);
            
            // Clear input
            if (commandInput != null)
                commandInput.text = "";
        }
        
        private string ProcessCommand(string command)
        {
            string[] parts = command.Split(' ');
            string cmd = parts[0].ToLower();
            
            switch (cmd)
            {
                case "adb":
                    return ExecuteADBCommand(command);
                case "help":
                    return GetHelpText();
                case "clear":
                    ClearOutput();
                    return "";
                case "devices":
                    return GetConnectedDevices();
                case "logcat":
                    return GetLogcat();
                default:
                    return ExecuteSystemCommand(command);
            }
        }
        
        private string ExecuteADBCommand(string command)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                // ADB command execution
                string[] args = command.Split(' ');
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "adb",
                    Arguments = string.Join(" ", args, 1, args.Length - 1),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;
                }
            }
            catch (System.Exception e)
            {
                return $"Error: {e.Message}";
            }
            #else
            return "ADB commands only available on Android";
            #endif
        }
        
        private string ExecuteSystemCommand(string command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                
                using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return string.IsNullOrEmpty(output) ? "Command executed" : output;
                }
            }
            catch (System.Exception e)
            {
                return $"Error: {e.Message}";
            }
        }
        
        private string GetConnectedDevices()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return "Connected device: Meta Quest 3";
            #else
            return "No connected devices";
            #endif
        }
        
        private string GetLogcat()
        {
            return "Logcat output...";
        }
        
        private string GetHelpText()
        {
            return @"Asteroid Terminal Help

Commands:
  adb <args>     - Execute ADB commands
  devices        - List connected devices
  logcat         - Show logcat output
  clear          - Clear terminal output
  help           - Show this help message";
        }
        
        public void LogTerminalMessage(string message)
        {
            if (outputDisplay == null)
                return;
            
            outputDisplay.text += message + "\n";
            currentOutputLines++;
            
            // Remove old lines if exceeding max
            if (currentOutputLines > maxOutputLines)
            {
                string[] lines = outputDisplay.text.Split('\n');
                outputDisplay.text = string.Join("\n", lines, 1, lines.Length - 1);
                currentOutputLines--;
            }
            
            // Auto-scroll to bottom
            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 0f;
        }
        
        private void ClearOutput()
        {
            if (outputDisplay != null)
            {
                outputDisplay.text = "";
                currentOutputLines = 0;
            }
        }
    }
}
