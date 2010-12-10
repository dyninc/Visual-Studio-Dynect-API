using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynSoapWrapper.DynClasses;

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

        }
    }
}
