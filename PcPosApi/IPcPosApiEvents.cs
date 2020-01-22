using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PcPosApi
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface IPcPosApiEvents
    {
        [DispId(46200)]
        void ConnectionCompleted();
    }
   
}
