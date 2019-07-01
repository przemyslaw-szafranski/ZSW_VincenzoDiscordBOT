using System;
using System.Collections.Generic;
using System.Text;

namespace VincenzoBot
{
    public interface ILogger
    {
        void Log(string msg, [System.Runtime.CompilerServices.CallerMemberName] string caller="");
    }
}
