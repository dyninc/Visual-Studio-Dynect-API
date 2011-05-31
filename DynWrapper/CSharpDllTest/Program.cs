using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using CSharpDllTest.DynClasses;

namespace CSharpDllTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a dynect wrapper object with the proper credentials
            DynectWrapper wrapper = new DynectWrapper("my_customer_name", "my_user_name", "my_password");

            // Add an A record
            wrapper.CreateARecord("my_zone", "my_fqdn", "1.1.1.1", DynectWrapper.TTLSeconds.THIRTY);

            // publish the zone
            wrapper.PublishZone("my_zone");

            // create the object from the imported wsdl which should appear in your project as a web reference
            /*net.dynect.api2.Dynect dynectWsdl = null;

            dynectWsdl = new CSharpDllTest.net.dynect.api2.Dynect();

            // init the request
            net.dynect.api2.SessionLoginRequestType request = new CSharpDllTest.net.dynect.api2.SessionLoginRequestType();

            // setup the session parameters
            request.customer_name = "customername";
            request.user_name = "username";
            request.password = "password";
            request.fault_incompat = 1;
            request.fault_incompatSpecified = true;

            // do the login
            net.dynect.api2.SessionLoginResponseType response = dynectWsdl.SessionLogin(request);

            // get the data back including the token
            net.dynect.api2.SessionLoginData sessionData = response.data;

            // make sure we got back a valid session data object
            if (sessionData == null)
                return;

            // init the Create A Record Request object
            net.dynect.api2.CreateARecordRequestType aRequest = new CSharpDllTest.net.dynect.api2.CreateARecordRequestType();

            // setup the A record parameters 
            aRequest.token = sessionData.token;
            aRequest.fault_incompat = 1;
            aRequest.fault_incompatSpecified = true;
            aRequest.zone = "myzone.com";
            aRequest.fqdn = "myfqdn.myzone.com";

            // init the Rdata for the A record
            aRequest.rdata = new CSharpDllTest.net.dynect.api2.RDataA();

            // now fill in the rdata
            aRequest.rdata.address = "192.168.1.2";
            aRequest.ttl = 30;

            // Create the record
            net.dynect.api2.CreateARecordResponseType aResponse = dynectWsdl.CreateARecord(aRequest);

            // get the response
            net.dynect.api2.ARecordData retVal = aResponse.data;

            // init the publish zone object
            net.dynect.api2.PublishZoneRequestType pubRequest = new CSharpDllTest.net.dynect.api2.PublishZoneRequestType();

            // setup the publish parameters
            pubRequest.token = sessionData.token;
            pubRequest.fault_incompat = 1;
            pubRequest.fault_incompatSpecified = true;
            pubRequest.zone = "myzone.com";

            // Now publish the zone
            net.dynect.api2.PublishZoneResponseType pubResponse = dynectWsdl.PublishZone(pubRequest);

            // get the response
            net.dynect.api2.ZoneData pubRetVal = pubResponse.data;*/

        }
    }
}
