using System;
using System.Collections.Generic;
using System.Text;
using PcPosClassLibrary;
using VPCPOS;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace PcPosApi
{
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PcPosApi.CPcPosApi")]
    [Guid("EBC5881B-AAB6-4383-AF72-58BC375B5C03")]
  //  static const GUID <<name>> = 
//{ 0xebc5881b, 0xaab6, 0x4383, { 0xaf, 0x72, 0x58, 0xbc, 0x37, 0x5b, 0x5c, 0x3 } };

    [ComSourceInterfaces(typeof(IPcPosApiEvents))]
    [ComVisible(true)]
    public class CPcPosApi : IPcPosApi
    {
      //  public event Action ConnectionCompleted;

       private  PcPosClassLibrary.PCPOS  PEP_Pos;
        RecievedData PEP_RD = new RecievedData();
        string[]  str = { "", "", "", "", "" };

        clsMessage  Sepehr_transaction;
        clsMessage.request_s Sepehr_req;

        public CPcPosApi ()
        {
          
        }


        public string[] ports { get; set; }

        
        private static string _Err = null;  // for preventing remove Err By Garbage collector (in Multi Provider Senario)

        public string Err {
            get
            {
                return _Err;
            }
            set
            {
                _Err = value;
              
            }
        }

        private static string _RefNo = null;  // for preventing remove RefNo By Garbage collector  (in Multi Provider Senario)
        public string RefNo
        {
            get
            {
                return _RefNo;
            }

            set
            {
                _RefNo = value;
            }

        }

        private static string _POS_Count = null;  // for preventing remove POS_Count By Garbage collector  (in Multi Provider Senario)
        public string POS_Count {
            get
            {
                return _POS_Count;
            }

            set
            {
                _POS_Count = value;
            }

        }

        private AutoResetEvent a = new AutoResetEvent(false);

        static Semaphore semSepehr= new Semaphore(0,1); // 0(initial num of thread to enter): sepehr blocked in the beginning of  time  //  1(num of max thread to enter): 
        static Semaphore semRecieveMessage = new Semaphore(1, 1);

        

        //     public System.Timers.Timer t = new System.Timers.Timer(1000);
        public bool Run(int nCommandIndex, string strParamater)
        {
           
                Debug.WriteLine("run section   nCommandIndex: " + nCommandIndex.ToString() + "  strParamater: " + strParamater);
                //  if (ConnectionCompleted!=null)
                //      ConnectionCompleted();
               

                if ((strParamater == ""))
                    strParamater = "7000;192.168.123.34;com1;10000;70000";

                str = strParamater.Split(';');

               
    
                int API_TimeOut_ms = Convert.ToInt32(str[4]) + 10000;

                // 0: port
                // 1: ip
                // 2: usb port
                // 3: price
                // 4: timeout  (ms)

                int nCommandIndex_Provider = nCommandIndex / 100;
                int temp = nCommandIndex % 100;
                int nCommandIndex_TransMode = temp / 10;
                temp = temp % 10;
                int nCommandIndex_ConType = temp ;




                if (PEP_Pos != null)
                    PEP_Pos.Dispose();

            // t.Elapsed += T_Elapsed;

            try
            {

                switch (nCommandIndex_Provider)
                {

                    case 11://pep
                           
                            switch (nCommandIndex_TransMode)
                            {
                                case 1://async
                                    switch (nCommandIndex_ConType)
                                    {
                                        case 1://lan (sync)
                                           
                                            PEP_Pos = new PcPosClassLibrary.PCPOS(Convert.ToInt32(str[0]), str[1]);
                                            PEP_Pos.SetLanReceiveTimeout(Convert.ToInt32(Convert.ToInt32(str[4]) / 1000));
                                            if (Convert.ToInt32(POS_Count) > 1) semSepehr.Release(); //release Sepehr semaphore
                                            CallWithTimeout(() => PEP_RD = PEP_Pos.SyncSale(Convert.ToInt32(str[3])), API_TimeOut_ms);
                                            semRecieveMessage.WaitOne(); //every thread recieved data block  others     
                                           //CallWithTimeout(() => RD = pos.SyncSale(Convert.ToInt32(str[3])),Convert.ToInt32( str[4])); // causing incomplete session in  sync operation
                                            RefNo = PEP_RD.ReferenceNumber;
                                            Err = PEP_RD.ErrorCode.ToString();
                                            break;
                                            case 2: //usb
                                            PEP_Pos = new PcPosClassLibrary.PCPOS(str[2]);
                                            //  t.AutoReset = true;
                                            //  t.Enabled = true;
                                            PEP_Pos.DataRecieved += new PcPosClassLibrary.DataRecievedEventHandler(this.Pep_pos_DataReceived);
                                            if (Convert.ToInt32(POS_Count) > 1) semSepehr.Release(); //release Sepehr semaphore
                                            PEP_Pos.Sale(Convert.ToInt32(str[3]));
                                            a.WaitOne();
                                            //  t.Stop(); 
                                            break;
                                    }
                                    break;
                                case 2: //sync
                                    switch (nCommandIndex_ConType)
                                    {
                                        case 1://lan
                                            Debug.WriteLine("pep : " + nCommandIndex.ToString() + " : " + strParamater);
                                            string str1 = "";
                                            for (int i = 0; i < 5; i++)
                                            str1 += str[i];
                                            Debug.WriteLine("pep : " + str1);

                                            PEP_Pos = new PcPosClassLibrary.PCPOS(Convert.ToInt32(str[0]), str[1]);
                                            PEP_Pos.SetLanReceiveTimeout(Convert.ToInt32(Convert.ToInt32(str[4]) / 1000));
                                            if (Convert.ToInt32(POS_Count) > 1) semSepehr.Release(); //release Sepehr semaphore
                                            CallWithTimeout(() => PEP_RD = PEP_Pos.SyncSale(Convert.ToInt32(str[3])), API_TimeOut_ms);
                                            semRecieveMessage.WaitOne(); //every thread recieved data blocked  others   
                                            Debug.WriteLine("------------- semRecieveMessage  waitOne ");
                                            //CallWithTimeout(() => RD = pos.SyncSale(Convert.ToInt32(str[3])),Convert.ToInt32( str[4])); // causing incomplete session in  sync operation
                                            RefNo = PEP_RD.ReferenceNumber;
                                            Err = PEP_RD.ErrorCode.ToString();
                                            break;
                                        case 2: //usb
                                            PEP_Pos = new PcPosClassLibrary.PCPOS(str[2]);
                                            PEP_Pos.SetLanReceiveTimeout(Convert.ToInt32(Convert.ToInt32(str[4]) / 1000));
                                            semSepehr.Release(); //release Sepehr semaphore
                                            CallWithTimeout(() => PEP_RD = PEP_Pos.SyncSale(Convert.ToInt32(str[3])), API_TimeOut_ms);
                                            semRecieveMessage.WaitOne();   //every thread recieved data block  others   
                                            //CallWithTimeout(() => RD = pos.SyncSale(Convert.ToInt32(str[3])),Convert.ToInt32( str[4])); // causing incomplete session in  sync operation
                                            RefNo = PEP_RD.ReferenceNumber;
                                            Err = PEP_RD.ErrorCode.ToString();
                                            break;
                                    }

                                    break;
                            }
                            break;
                        

                    case 12: //sepehr
                       
                            switch (nCommandIndex_TransMode)
                            {
                                case 1://async
                                    switch (nCommandIndex_ConType)
                                    {
                                        case 1://lan (sync)
                                            return false;
                                        case 2: //usb
                                            return false;
                                    }
                                    break;
                                case 2: //sync
                                    switch (nCommandIndex_ConType)
                                    {
                                        case 1://lan
                                        Debug.WriteLine("sepehr  : " + nCommandIndex.ToString() + " : " + strParamater);
                                        string str1 = "";
                                            for (int i = 0; i < 5; i++)
                                                str1 += str[i];
                                            Debug.WriteLine("sepehr  : " + str1);

                                            clsCommunication clsCommunicationObj = new clsCommunication();
                                            clsCommunicationObj.ConnType = (int)clsCommunication.connectionType.ETHERNET;
                                            clsCommunicationObj.IPAddress = str[1];
                                            clsCommunicationObj.IPPort = Convert.ToInt32(str[0]);
                                            clsCommunicationObj.TimeOut = Convert.ToInt32(Convert.ToInt32(str[4]) / 1000); // force 20 sec
                                            Sepehr_transaction = new clsMessage();
                                            Sepehr_req.msgTyp = clsMessage.msgType.Sale;
                                            Sepehr_req.terminalID = "";
                                            Sepehr_req.amount = str[3];
                                            Sepehr_transaction.request = Sepehr_req;
                                            
                                            if (Convert.ToInt32(POS_Count) > 1)
                                                  semSepehr.WaitOne(); // wait until others begin 
                                            
                                            int SendretCode = Sepehr_transaction.SendMessage(0);
                                            if (SendretCode != 0)
                                            {
                                                Err = (100).ToString();
                                                semRecieveMessage.WaitOne();
                                                Debug.WriteLine("------------- semRecieveMessage  waitOne ");
                                               goto end;
                                        } // not send
                                            int RecieveretCode = 0;
                                            CallWithTimeout(() => RecieveretCode = Sepehr_transaction.ReceiveMessage(0), API_TimeOut_ms);
                                            semRecieveMessage.WaitOne(); //every thread recieved data block  others 
                                            Debug.WriteLine("------------- semRecieveMessage  waitOne ");
                                            if (RecieveretCode != 0)
                                            {
                                                    Err = (RecieveretCode + 110).ToString(); goto end;
                                            }   //not recieved  //  110-1 : 109 : not recived  //  110-5:105 : timeout // else : Message MAC Error
                                            string POSErrCode = Sepehr_transaction.response.AppResponseCode;
                                            if (Sepehr_transaction.response.AppResponseCode != "00")
                                            {
                                                Err = (Convert.ToInt32(POSErrCode) + 200).ToString(); goto end;
                                        }  //POS Err code:  +200

                                            RefNo = Sepehr_transaction.response.SystemTraceNumber== "000000000000" ? "0":Sepehr_transaction.response.SystemTraceNumber;  // 000000000000 -> 0

                                            break;
                                        case 2: //usb

                                            break;
                                    }

                                    break;
                            }
                            break;

                        
                }
                end:
                Debug.WriteLine("Error:"+Err+"\n RefNo:"+RefNo);
            }
            catch (Exception ex)
            {
                if (ex.Message == "The operation has timed out.") //handle  Exception  (CallWithTimeout)
                {
                    if (nCommandIndex_Provider == 11) //PEP
                        Err = "132";
                    else if (nCommandIndex_Provider == 12) //Sepehr
                        Err = "105";
                }
                Debug.WriteLine("Run Api Func Exception:"+ ex.ToString()+ "\n API_TimeOut_ms: " + API_TimeOut_ms);
                
                if (PEP_Pos != null)
                {
                    PEP_Pos.cancelTrans();
                }
                return false;
            }
           
            return true;
        }
        
        static void CallWithTimeout(Action action , int timeoutMilliseconds)
        {
            Thread thresdToKill = null;
            Action wrapedAction = () =>
            {
                thresdToKill = Thread.CurrentThread;
                try
                {
                    action();

                }
                catch (ThreadAbortException ex)
                {
                    Thread.ResetAbort();
                }
            };

            IAsyncResult result = wrapedAction.BeginInvoke(null, null);
            if(result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrapedAction.EndInvoke(result);
            }
            else
            {
                thresdToKill.Abort();
                throw new TimeoutException();
            }
        }

        //private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    t.Stop();
        //    t.Dispose();

            
        //    a.Set();
        //}



        private void Pep_pos_DataReceived(object sender, PcPosClassLibrary.DataRecievedArgs e)
        {
          
            if (e.recievedData != null)
            {
              
                RefNo = e.recievedData.ReferenceNumber;
          
                if (e.recievedData.HasError == true)
                {
                    Err=e.recievedData.ErrorCode.ToString();
                }
                else
                {

                    if (RefNo != "" && RefNo != null)
                    {
                        //    ConnectionCompleted();
                        Err = "0";
                    }
                    if (e.recievedData.RequestCode == "1234" && e.recievedData.ReportPurchaseSucc != null)
                    {

                    }
                }
            }
            else if (e.recievedDataInquiry != null)
            {
                if (e.recievedDataInquiry.Length != e.recievedDataInquiry[0].NumberOfTransactions)
                    MessageBox.Show("تعداد تراکنش ها: " + e.recievedDataInquiry.Length + Environment.NewLine + "مجموع تراکنش ها: " + e.recievedDataInquiry[0].NumberOfTransactions, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                for (int i = 0; i < e.recievedDataInquiry.Length; i++)
                {
                    /**
                    string tran = "";
                    tran = " transaction Status = {0} amount = {1} seq number = {2} card Numer = {3} Date = {4} Transaction Type = {5} POS Status = {6} Error Code = {7}" +
                                        e.recievedDataInquiry[i].TransactionStatus + e.recievedDataInquiry[i].Amount + e.recievedDataInquiry[i].SequenceNumber +
                                        e.recievedDataInquiry[i].CardNumber + e.recievedDataInquiry[i].Date + mapTransactionType(e.recievedDataInquiry[i].TransactionType) +
                                        mapPOSStatus(e.recievedDataInquiry[i].POSStatus) + e.recievedDataInquiry[i].ErrorCode.ToString();
                    SetText(txtReceivedRequestCode, e.recievedDataInquiry[i].RequestCode);
                    txtGroupReport.Text += "RequestCode = " + e.recievedDataInquiry[i].RequestCode + Environment.NewLine;
                    SetText(txtReceivedTransactionStatus, e.recievedDataInquiry[i].TransactionStatus);
                    txtGroupReport.Text += "TransactionStatus = " + e.recievedDataInquiry[i].TransactionStatus + Environment.NewLine;
                    SetText(txtReceivedAmount, e.recievedDataInquiry[i].Amount);
                    txtGroupReport.Text += "Amount = " + e.recievedDataInquiry[i].Amount;
                    SetText(txtReceivedCardNumber, e.recievedDataInquiry[i].CardNumber);
                    txtGroupReport.Text += Environment.NewLine + "CardNumber = " + e.recievedDataInquiry[i].CardNumber;
                    SetText(txtReceivedSequenceNumber, e.recievedDataInquiry[i].SequenceNumber);
                    txtGroupReport.Text += Environment.NewLine + "SequenceNumber = " + e.recievedDataInquiry[i].SequenceNumber + Environment.NewLine;
                    SetText(txtReceivedDateAndTime, e.recievedDataInquiry[i].Date);
                    txtGroupReport.Text += "Date = " + e.recievedDataInquiry[i].Date + Environment.NewLine;
                    SetText(transactionTypeTextBox, mapTransactionType(e.recievedDataInquiry[i].TransactionType));
                    txtGroupReport.Text += "TransactionType = " + mapTransactionType(e.recievedDataInquiry[i].TransactionType) + Environment.NewLine;
                    SetText(posStatusTextBox, mapPOSStatus(e.recievedDataInquiry[i].POSStatus));
                    txtGroupReport.Text += "POSStatus = " + mapPOSStatus(e.recievedDataInquiry[i].POSStatus) + Environment.NewLine;
                    SetText(errorCodeTextBox, e.recievedDataInquiry[i].ErrorCode.ToString());
                    txtGroupReport.Text += "ErrorCode = " + e.recievedDataInquiry[i].ErrorCode.ToString() + Environment.NewLine;
                    txtGroupReport.Text += "--------------------------------" + Environment.NewLine;
                **/
                }
              //  MessageBox.Show("تراکنش با موفقیت انجام شد" + "لطفا فرم ارتباط با دستگاه را ببندید" + 0, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (PEP_Pos != null)
            {

                PEP_Pos.cancelTrans();
                PEP_Pos.Close();
                PEP_Pos.Close();

                PEP_Pos.Dispose();  // for close transaction
            }
            a.Set();
        }

        public  bool close(int nCommandIndex, string strParamater)
        {
            try
            {
                if ((nCommandIndex / 100) == 11)  // Is PEP
                {
                    if (PEP_Pos == null)
                        PEP_Pos = new PcPosClassLibrary.PCPOS(Convert.ToInt32(str[0]), str[1]);

                    if (PEP_Pos != null)
                    {

                        PEP_Pos.cancelTrans();
                        PEP_Pos.Close();
                        PEP_Pos.Close();

                        PEP_Pos.Dispose();  // for close transaction
                    }
                }
                return true;
            }
           catch(Exception ex)
            {
              //  MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool Reset_semRecieveMessage()
        {
            try
            {
                semRecieveMessage.Release();
                Debug.WriteLine("------------ semRecieveMessage released :");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }



        public bool GetUSBPorts()
        {
            try
            {
                 ports = System.IO.Ports.SerialPort.GetPortNames();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

   
      

    }
}


//-------------------------   socket -------------------------------

// string data = null;
// byte[] bytes = new Byte[1024];

//IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
//IPAddress ipaddress = ipHostInfo.AddressList[0];

//while (true)
//{
//    Socket handler = listener.Accept();

//    //while(true)
//    //{
//    //    int byteRec = handler.Receive(bytes);
//    //    data += Encoding.ASCII.GetString(bytes, 0, byteRec);
//    //}
//}


//--------------------------- socket ------------------------------------