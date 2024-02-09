// TwinCAT ADS RW Test
// Written by N. Smith, 09/02/2024
// Test code to communicate with a local Beckhoff XAE runtime, reading and writing to variables on the PLC
// Add a global variable list named 'GVL_Vars'. you will need to add 2 variables as described below
// Required variables: 
// 1. testABC (type=bool,init val=FALSE)
// 2. test123 (type=UINT,init val=0)
// You MUST run this command before running the program: dotnet add package Beckhoff.TwinCAT.Ads --version 6.1.154


using System;
using TwinCAT.Ads; // dotnet add package Beckhoff.TwinCAT.Ads --version 6.1.154

namespace TwinCAT_ADS_RW_Test
{
    class Program
    {
        public static void Main()
        {

            AdsClient myClient = new AdsClient();
            myClient.Connect(AmsNetId.Local,851); // connect to TC3 PLC runtime 1

            System.Console.WriteLine("ADS Client has been intialised");

            uint bTestABChandle = myClient.CreateVariableHandle("GVL_Vars.testABC"); // requires global variable in list GVL named testABC
            uint dTest123handle = myClient.CreateVariableHandle("GVL_Vars.test123");

            bool readABCVal = myClient.ReadAny<bool>(bTestABChandle); // read the current value of testABC global variable
            System.Console.WriteLine(readABCVal); // console log the response

            ushort read123Val = myClient.ReadAny<ushort>(dTest123handle); // read the current value of test123 global variable
            System.Console.WriteLine(read123Val); // console log the response


            //bool invResp = !Convert.ToBoolean(resp);
            //System.Console.WriteLine(invResp);

            while(true) // loop for eternity, flipping state of PLC boolean variable & incrementing the integer variable
            {
                readABCVal = myClient.ReadAny<bool>(bTestABChandle); // read current value of testABC PLC global variable
                bool invResp = !readABCVal; // invert
                myClient.WriteAny(bTestABChandle,invResp); // write inverted response to PLC
                System.Console.WriteLine(invResp); // console log the new status of PLC global variable
                
                read123Val = myClient.ReadAny<ushort>(dTest123handle); // read current value of test123 PLC global variable
                var increment = 10; // define value to increment by
                ushort newVal = (ushort)(read123Val + increment); // increment value by increment amount, with explicit ushort cast
                System.Console.WriteLine(newVal); // console log the new status of PLC global variable
                myClient.WriteAny(dTest123handle,newVal); // write incremented value to PLC
                
                Thread.Sleep(1000); // sleep for 1 sec 
            }
            
        }
    }
}