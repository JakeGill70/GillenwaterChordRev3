using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    public interface IMessageProcessor
    {
        public Task<Message> ProcessMsgAsync(Message msg);
    }
}
