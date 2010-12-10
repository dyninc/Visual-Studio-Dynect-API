Module Module1

    Sub Main()
        'Create the Dyn Wrapper object
        Dim wrapper As DynSoapWrapper.DynClasses.DynectWrapper = New DynSoapWrapper.DynClasses.DynectWrapper("my_customer_name", "my_user_name", "my_password")

        'Create an A record
        wrapper.CreateARecord("my_zone", "my_fqdn", "1.1.1.1", DynSoapWrapper.DynClasses.DynectWrapper.TTLSeconds.THIRTY)

        'publish the zone
        wrapper.PublishZone("my_zonet")

    End Sub

End Module
