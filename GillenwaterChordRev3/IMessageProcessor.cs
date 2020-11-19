using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GillenwaterChordRev3.Messages;

namespace GillenwaterChordRev3
{
    public interface IMessageProcessor
    {
        public Message ProcessMsg(Message msg);
        public string GetLocalResource(string resourceId);
        public void SetLocalResource(string resourceId, string resourceName, string resourceContent);
    }
}
