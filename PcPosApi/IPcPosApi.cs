using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PcPosApi
{
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [Guid("519C6E04-BB94-471A-9F19-92340C031A6A")]
  ///  static const GUID <<name>> = 
///{ 0x519c6e04, 0xbb94, 0x471a, { 0x9f, 0x19, 0x92, 0x34, 0xc, 0x3, 0x1a, 0x6a } };

    [ComVisible(true)]
    public interface IPcPosApi
    {
        string RefNo { get; set; }
        string[] ports { get; set; }
        // int Status { get; set; }
        string Err { get; set; }

        string POS_Count { get; set; }


        bool Run(int nCommandIndex, string strParamater);

         bool  close(int nCommandIndex, string strParamater);

        bool Reset_semRecieveMessage();

        bool GetUSBPorts();

        
    }
   
}
