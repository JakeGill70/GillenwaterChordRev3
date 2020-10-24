using System;
using System.Collections.Generic;
using System.Text;

namespace GillenwaterChordRev3
{
    // Possible message types used by nodes to communicate
    public enum MessageType
    {
        Unknown,
        Testing,
        JoinRequest,
        JoinResponse,
        OwnerOfIdRequest,
        OwnerOfIdResponse,
        AddResourceRequest,
        AddResourceResponse
    }
}
