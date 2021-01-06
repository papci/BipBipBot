using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BipBipBot.DataEngine
{
    public class PrivateMessageEvent : IrcEvent
    {
        public string SenderName { get; set; }
        public string SenderHost { get; set; }
        public string Destination { get; set; }
        public string ContentText { get; set; }

        public PrivateMessageEvent()
        {
            
        }

        public PrivateMessageEvent(string rawMessage)
        {
            var split = rawMessage.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            SetSenderData(split[0]);
            SetContent(split.Skip(2).ToArray());

        }

        private void SetContent(string[] skip)
        {
            this.Destination = skip[0];
            this.ContentText = skip[1].TrimStart(':');
        }

        private void SetSenderData(string s)
        {
            var regx = new Regex(":([^!]*)!(.*)");
            var match = regx.Match(s);
            if (match.Success)
            {
                SenderName = match.Groups[1].Value;
                SenderHost = match.Groups[2].Value;
            }
            else
            {
                throw new ArgumentException($"Invalid private message, sender data are not valid. Raw sender {s}");
            }

        }
    }
}