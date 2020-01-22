using System;
using System.Collections.Generic;
using System.Text;
using DotNetPluginCommon;
using PcPosApi;

namespace PcPosApiPlugin
{
    public class CPcPosApiPlugin : ITadbirPluginInterface
    {
        public bool DisplaysAbout(int nCommandIndex)
        {
            return false;
        }

        public bool DisplaysConfig(int nCommandIndex)
        {
            return false;
        }

        public bool ExecuteAbout(int nCommandIndex)
        {
            return false;
        }


        public bool ExecuteConfig(int nCommandIndex)
        {
            return false;
        }

        public string GetAPIVersion()
        {
            return "9.7.0.0";
        }

        public bool GetDebugMode()
        {
           return true;
        }

        public string GetPluginDescription()
        {
           return "this is PcPosApiPlugin";
        }

        public Guid GetPluginGuid()
        {
            return new Guid("{90A8D603-522F-48DD-A950-9E46609E95C7}");
            //{ 0x90a8d603, 0x522f, 0x48dd, { 0xa9, 0x50, 0x9e, 0x46, 0x60, 0x9e, 0x95, 0xc7 } } 
        }

        public string GetPluginName()
        {
            return "PcPosApi plugin";
        }

        public bool QueryCommand(int nCommandIndex, out string strCommandText, out string strCommandDesc)
        {
            switch (nCommandIndex)
            {
                case 1111:
                    strCommandText = "first command";
                    strCommandDesc = "";
                    return true;
             
               
            }
            strCommandText = "";
            strCommandDesc = "";
            return false;
        }
       
        public bool ExecuteCommand(int nCommandIndex, string strParamater)
        {
            PcPosApi.CPcPosApi PcPosApi = new CPcPosApi();

            return PcPosApi.Run( nCommandIndex,  strParamater);
        }

        public bool QueryRequiredPermissions(int nCommandIndex, int nQueryIndex, out TadbirSubsystem nSys, out TadbirForm nForm, out TadbirRight nPos)
        {

            nSys = TadbirSubsystem.ASSET_SYS;
            nForm = TadbirForm.AF_NONE;
            nPos = TadbirRight.PLUGIN_VIEW_RIGHT;

            return false;
        }
    }
}
