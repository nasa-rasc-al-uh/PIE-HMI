using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIE_HMI.Util
{

    public enum MessageType
    {
        Note,
        Warning,
        Error
    }

    public struct Message
    {
        public string message;
        public MessageType type;
    }

}
