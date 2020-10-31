﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GillenwaterChordRev3.Messages;

namespace GillenwaterChordRev3
{
    public interface IMessageProcessor
    {
        public Task<Message> ProcessMsgAsync(Message msg);
    }
}
