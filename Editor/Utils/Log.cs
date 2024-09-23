using System;
using System.Text;
using UnityEngine;

namespace YuebyAvatarTools.Packages.yueby.tools.core.Editor.Utils
{
    public static class Log
    {
        public const string Tag = "<color=#d4e5ef>[YuebyTools]</color>";

        public static void Info(params object[] messages)
        {
            Debug.Log($"{Tag}<b><color=#81c784>[Info]</color></b> {MergeMessage(messages)}");
        }

        public static void Warning(params object[] messages)
        {
            Debug.LogWarning(
                $"{Tag}<b><color=#ffff8d>[Warning]</color></b> {MergeMessage(messages)}"
            );
        }

        public static void Error(params object[] messages)
        {
            Debug.LogError($"{Tag}<b><color=#ff5252>[Error]</color></b> {MergeMessage(messages)}");
        }

        public static void Exception(Exception e, params object[] messages)
        {
            Debug.LogError(
                $"{Tag}<b><color=#e040fb>[Exception]</color></b> {MergeMessage(messages)}\n{e.Message}\n{e.StackTrace}"
            );
        }

        private static string MergeMessage(params object[] messages)
        {
            var stringBuilder = new StringBuilder();
            foreach (var message in messages)
            {
                stringBuilder.Append(message.ToString() + " ");
            }

            return stringBuilder.ToString();
        }
    }
}
