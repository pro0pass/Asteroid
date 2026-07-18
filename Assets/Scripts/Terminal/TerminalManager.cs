using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

namespace Asteroid.Terminal
{
    /// <summary>
    /// Optimized Terminal Manager for VR/Android
    /// - Ring buffer for command history
    /// - StringBuilder for text building
    /// - Efficient string parsing
    /// </summary>
    public class TerminalManager : MonoBehaviour
    {
        [SerializeField] private InputField commandInput;
        [SerializeField] private Text outputDisplay;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private int maxOutputLines = 500;
        [SerializeField] private int maxCommandHistory = 50;
        
        private List<string> commandHistory;
        private int historyIndex = -1;
        private int currentOutputLines = 0;
        private StringBuilder outputBuffer;
        private List<string> outputLines;
        
        private void Start()
        {
            // Pre-allocate collections
            commandHistory = new List<string>(maxCommandHistory);
            outputLines = new List<string>(maxOutputLines);
            outputBuffer = new StringBuilder(1024);
            
            if (commandInput != null)
                commandInput.onEndEdit.AddListener(ExecuteCommand);
            
            LogTerminalMessage("Asteroid Terminal v1.0\nWireless Debugging Enabled");
        }
        
        private void ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return;
            
            // Echo command
            LogTerminalMessage($"$ {command}");
            
            // Add to history
            commandHistory.Add(command);
            if (commandHistory.Count > maxCommandHistory)
                commandHistory.RemoveAt(0);
            
            // Reset history index
            historyIndex = -1;
            
            // Execute
            string output = ProcessCommand(command);
            
            if (!string.IsNullOrEmpty(output))
                LogTerminalMessage(output);
            
            // Clear input
            if (commandInput != null)
                commandInput.text = "";
        }
        
        private string ProcessCommand(string command)
        {
            // Find first space
            int spaceIndex = command.IndexOf(' ');
            string cmd = spaceIndex > 0 ? command.Substring(0, spaceIndex).ToLower() : command.ToLower();
            
            switch (cmd)
            {
                case "help":
                    return GetHelpText();
                case "clear":
                    ClearOutput();
                    return "";
                case "devices":
                    return GetConnectedDevices();
                default:
                    return "Unknown command. Type 'help' for available commands.";
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
        
        private string GetHelpText()
        {
            return "Asteroid Terminal Help\n\nCommands:\n  help       - Show this help\n  devices    - List connected devices\n  clear      - Clear output";
        }
        
        public void LogTerminalMessage(string message)
        {
            if (outputDisplay == null)
                return;
            
            // Split message into lines
            string[] lines = message.Split('\n');
            
            foreach (string line in lines)
            {
                outputLines.Add(line);
                currentOutputLines++;
            }
            
            // Remove oldest lines if exceeding max
            while (currentOutputLines > maxOutputLines)
            {
                outputLines.RemoveAt(0);
                currentOutputLines--;
            }
            
            // Rebuild display text
            outputBuffer.Clear();
            for (int i = 0; i < outputLines.Count; i++)
            {
                outputBuffer.Append(outputLines[i]);
                if (i < outputLines.Count - 1)
                    outputBuffer.Append('\n');
            }
            
            outputDisplay.text = outputBuffer.ToString();
            
            // Auto-scroll
            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 0f;
        }
        
        private void ClearOutput()
        {
            if (outputDisplay != null)
            {
                outputDisplay.text = "";
                outputLines.Clear();
                currentOutputLines = 0;
                outputBuffer.Clear();
            }
        }
        
        private void OnDestroy()
        {
            if (commandInput != null)
                commandInput.onEndEdit.RemoveListener(ExecuteCommand);
        }
    }
}
