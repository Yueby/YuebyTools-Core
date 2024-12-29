using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Yueby.Core.Utils
{
    public struct LogLevelColors
    {
        public readonly string LevelColor;
        public readonly string MessageColor;

        public LogLevelColors(string levelColor, string messageColor)
        {
            LevelColor = levelColor;
            MessageColor = messageColor;
        }
    }

    public static class YuebyLogger
    {
        private const string Tag = "<b>[</b>✦<b>]</b>";
        private const string HeartTag = "<color=#ff6196>❤</color>";

        private static readonly Dictionary<string, LogLevelColors> LogLevelColors = new Dictionary<string, LogLevelColors>
        {
            { "Info", new LogLevelColors("#16a085", "#c8e6c9") },
            { "Warning", new LogLevelColors("#d6a100", "#fff9c4") },
            { "Error", new LogLevelColors("#b10c0c", "#ffcdd2") },
            { "Exception", new LogLevelColors("#d46ae6", "#e1bee7") },
        };


        private static string FormatMessage(string logLevel, params object[] messages)
        {
            var colors = GetColors(logLevel);
            var tag = $"<b>[</b>✦<b><color={colors.LevelColor}>{logLevel}</color>]</b> ";

            var messageSb = new StringBuilder();

            if (messages != null && messages.Length > 0)
            {
                foreach (var message in messages)
                {
                    if (message != null)
                        messageSb.Append(message).Append(' ');
                }
            }

            var result = $"{tag} {messageSb.ToString().Trim()}"; // {HeartTag}
            return result;
        }

        private static LogLevelColors GetColors(string logLevel)
        {
            return LogLevelColors.TryGetValue(logLevel, out var colors) ? colors : new LogLevelColors("#ffffff", "#ffffff");
        }

        public static void LogInfo(params object[] messages) => Debug.Log(FormatMessage("Info", messages));

        public static void LogWarning(params object[] messages) => Debug.LogWarning(FormatMessage("Warning", messages));

        public static void LogError(params object[] messages) => Debug.LogError(FormatMessage("Error", messages));

        public static void LogException(Exception e, params object[] messages)
        {
            var array = new List<object> { e.Message };
            if (messages != null && messages.Length > 0)
            {
                array.Add("<color=#ffd180>|</color> ");
                array.AddRange(messages);
            }

            var formattedMessage = FormatMessage("Exception", array.ToArray());

            var message = $"{formattedMessage}\n{e.StackTrace}";
            Debug.LogError(message);
        }
    }
}