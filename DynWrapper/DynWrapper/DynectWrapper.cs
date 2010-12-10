using System;
using System.Collections.Generic;
using System.Text;

namespace DynSoapWrapper.DynClasses
{
    public class DynectWrapper
    {
        #region enums
        public enum GSLBRegionOption
        {
            USWEST,
            USCENTRAL,
            USEAST,
            EUWEST,
            EUCENTRAL,
            EUEAST,
            ASIA,
            GLOBAL
        }

        public enum FailoverMode
        {
            ip,
            cname,
            global
        }

        public enum TTLSeconds
        {
            NONE = 0,
            THIRTY = 30,
            SIXTY = 60,
            ONE_HUNDRED_FIFTY = 150,
            THREE_HUNDRED = 300,
            FOUR_HUNDRED_FIFTY = 450
        }

        public enum IPWeight
        {
            NONE,
            ONE = 1,
            TWO,
            THREE,
            FOUR,
            FIVE,
            SIX,
            SEVEN,
            EIGHT,
            NINE,
            TEN,
            ELEVEN,
            TWELVE,
            THIRTEEN,
            FOURTEEN,
            FIFTEEN
        }

        public enum IPServeMode
        {
            always,
            obey,
            remove,
            no
        }

        public enum MonitorInterval
        {
            ONE = 1,
            FIVE = 5,
            TEN = 10,
            FIFTEEN = 15
        }

        public enum MonitorProtocol
        {
            HTTP,
            HTTPS,
            PING,
            STMP
        }

        public enum MonitorNotificationEvent
        {
            ip,
            svc,
            both,
            neither
        }

        public enum ZoneSerialStyle
        {
            increment,
            epoch,
            day,
            minute
        }

        #endregion

        #region enum conversion methods
        public static string MonitorNotificationEventToString(MonitorNotificationEvent e)
        {
            switch(e)
            {
                case MonitorNotificationEvent.ip:
                    return "ip";
                case MonitorNotificationEvent.svc:
                    return "svc";
                case MonitorNotificationEvent.both:
                    return "svc,ip";
                case MonitorNotificationEvent.neither:
                    return string.Empty;
            }

            return string.Empty;
        }

        public static string GSLBRegionToString(GSLBRegionOption r)
        {
            switch (r)
            {
                case GSLBRegionOption.USWEST:
                        return "US West";
                case GSLBRegionOption.USCENTRAL:
                        return "US Central";
                case GSLBRegionOption.USEAST:
                        return "US East";
                case GSLBRegionOption.EUWEST:
                        return "EU West";
                case GSLBRegionOption.EUCENTRAL:
                        return "EU Central";
                case GSLBRegionOption.EUEAST:
                        return "EU East";
                case GSLBRegionOption.ASIA:
                        return "Asia";
                case GSLBRegionOption.GLOBAL:
                       return "global";
            }

            return "global";
        }

        public static GSLBRegionOption StringToGSLBRegion(string r)
        {
            switch (r.ToLower())
            {
                case "us west":
                    return GSLBRegionOption.USWEST;
                case "us central":
                    return GSLBRegionOption.USCENTRAL;
                case "us east":
                    return GSLBRegionOption.USEAST;
                case "eu west":
                    return GSLBRegionOption.EUWEST;
                case "eu central":
                    return GSLBRegionOption.EUCENTRAL;
                case "eu east":
                    return GSLBRegionOption.EUEAST;
                case "asia":
                    return GSLBRegionOption.ASIA;
                default:
                    return GSLBRegionOption.GLOBAL;
            }
        }
        #endregion

        #region private members
        private net.dynect.api2.Dynect dynectWsdl = null;
        private net.dynect.api2.SessionLoginData sessionData = null;
        #endregion

        #region Constructor/Destructor
        ~DynectWrapper()
        {
            SessionDisconnect();
        }

        public DynectWrapper()
        {
            dynectWsdl = new net.dynect.api2.Dynect();
            sessionData = null;
        }

        public DynectWrapper(string customerName, string userName, string password)
        {
            dynectWsdl = new net.dynect.api2.Dynect();
            
            sessionData = null;
            SessionConnect(customerName, userName, password);
        }
        #endregion

        #region Zones
        /// <summary>
        /// Creates a new primary zone
        /// </summary>
        /// <param name="rname">Administrative contact for this zone</param>
        /// <param name="serial_style"> The style of the zone's serial. Defaults to 'increment'. increment - Serials are incremented by 1 on every changeepoch - Serials will be the UNIX timestamp at the time of the publishday - Serials will be in the form of YYYYMMDDxx where xx is incremented by one for each change during that particular day.minute - Serials will be in the form of YYMMDDHHMM.</param>
        /// <param name="ttl">Default TTL (in seconds) for records in the zone.</param>
        /// <param name="zone">Name of zone to add the record to</param>
        /// <returns>The created zone or null if it failed to create</returns>
        public net.dynect.api2.ZoneData CreateZone(string rname, ZoneSerialStyle serial_style, TTLSeconds ttl, string zone)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.ZoneData retVal = null;
            try
            {
                net.dynect.api2.CreateZoneRequestType request = new DynSoapWrapper.net.dynect.api2.CreateZoneRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.ttl = (int)ttl;
                request.serial_style = serial_style.ToString();
                request.rname = rname;
                net.dynect.api2.CreateZoneResponseType response = dynectWsdl.CreateZone(request);

                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves a list of node names under the specified node.
        /// </summary>
        /// <param name="zone">zone to get nodes of</param>
        /// <returns>An array of strings of the zone names</returns>
        public string[] GetNodeList(string zone)
        {
            if (sessionData == null)
                return null;

            string[] retVal = null;
            try
            {
                net.dynect.api2.GetNodeListRequestType request = new DynSoapWrapper.net.dynect.api2.GetNodeListRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                net.dynect.api2.GetNodeListResponseType response = dynectWsdl.GetNodeList(request);

                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Publishs changes to a primary zone
        /// </summary>
        /// <param name="zone">zone to publish</param>
        /// <returns>The published zone</returns>
        public net.dynect.api2.ZoneData PublishZone(string zone)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.ZoneData retVal = null;
            try
            {
                net.dynect.api2.PublishZoneRequestType request = new DynSoapWrapper.net.dynect.api2.PublishZoneRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                net.dynect.api2.PublishZoneResponseType response = dynectWsdl.PublishZone(request);

                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves an existing zone
        /// </summary>
        /// <param name="zone">zone to get data for</param>
        /// <returns>Zone data of the zone or null if it fails or does not exist</returns>
        public net.dynect.api2.ZoneData GetOneZone(string zone)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.ZoneData retVal = null;
            try
            {
                net.dynect.api2.GetOneZoneRequestType request = new DynSoapWrapper.net.dynect.api2.GetOneZoneRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                net.dynect.api2.GetOneZoneResponseType response = dynectWsdl.GetOneZone(request);

                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }
        #endregion

        #region Public Helper Functions
        /// <summary>
        /// Add or create a LoadBalanceAddress array to send to the load balance service 
        /// </summary>
        /// <param name="currenetLoadBalanceAddressArray">The LoadBalanceAddress array to add the LoadBalanceAddress to or null to start a new array</param>
        /// <param name="address">The IPv4 address of this Node IP</param>
        /// <param name="label">A descriptive string describing this IP (may be string.empty)</param>
        /// <param name="weight">A number from 1-15 describing how often this record should be served. Higher means more</param>
        /// <param name="mode">Sets the behavior of this particular record. always - always serve this IP address, obey - Serve this address based upon its monitoring status, remove - Serve this address based upon its monitoring status. However, if it goes down, don't automatically bring it back up when monitoring reports it up, no - Never serve this IP address</param>
        /// <returns>An array of LoadBalanceAddress to pass to update or create</returns>
        public net.dynect.api2.LoadBalanceAddress[] AddLoadBalanceAddress(net.dynect.api2.LoadBalanceAddress[] currenetLoadBalanceAddressArray, 
                                                        string address, string label, IPWeight weight, IPServeMode mode)
        {
            net.dynect.api2.LoadBalanceAddress[] retVal = null;
            int index = 0;
            if (currenetLoadBalanceAddressArray == null)
            {
                retVal = new DynSoapWrapper.net.dynect.api2.LoadBalanceAddress[1];
            }
            else
            {
                retVal = new DynSoapWrapper.net.dynect.api2.LoadBalanceAddress[currenetLoadBalanceAddressArray.Length + 1];
                for(index = 0; index < currenetLoadBalanceAddressArray.Length; index++)
                {
                    retVal[index] = currenetLoadBalanceAddressArray[index];
                }
            }

            retVal[index] = new DynSoapWrapper.net.dynect.api2.LoadBalanceAddress();
            retVal[index].address = address;
            retVal[index].label = label;
            if (weight != IPWeight.NONE)
                retVal[index].weight = (int)weight;
            else
                retVal[index].weight = 1;
            retVal[index].serve_mode = mode.ToString();

            return retVal;
        }

        /// <summary>
        /// Add or create a GSLB array to send to the load balance service 
        /// </summary>
        /// <param name="currenetLoadBalanceAddressArray">The GSLB array to add the GSLB to or null to start a new array</param>
        /// <param name="address">The IPv4 address of this Node IP</param>
        /// <param name="label">A descriptive string describing this IP (may be string.empty)</param>
        /// <param name="weight">A number from 1-15 describing how often this record should be served. Higher means more</param>
        /// <param name="mode">Sets the behavior of this particular record. always - always serve this IP address, obey - Serve this address based upon its monitoring status, remove - Serve this address based upon its monitoring status. However, if it goes down, don't automatically bring it back up when monitoring reports it up, no - Never serve this IP address</param>
        /// <returns>An array of GSLB to pass to update or create</returns>
        public net.dynect.api2.GSLBAddress[] AddLoadBalanceAddress(net.dynect.api2.GSLBAddress[] currenetGSLBAddressArray,
                                                        string address, string label, IPWeight weight, IPServeMode mode)
        {
            net.dynect.api2.GSLBAddress[] retVal = null;
            int index = 0;
            if (currenetGSLBAddressArray == null)
            {
                retVal = new DynSoapWrapper.net.dynect.api2.GSLBAddress[1];
            }
            else
            {
                retVal = new DynSoapWrapper.net.dynect.api2.GSLBAddress[currenetGSLBAddressArray.Length + 1];
                for (index = 0; index < currenetGSLBAddressArray.Length; index++)
                {
                    retVal[index] = currenetGSLBAddressArray[index];
                }
            }

            retVal[index] = new DynSoapWrapper.net.dynect.api2.GSLBAddress();
            retVal[index].address = address;
            retVal[index].label = label;
            if (weight != IPWeight.NONE)
                retVal[index].weight = (int)weight;
            else
                retVal[index].weight = 1;
            retVal[index].serve_mode = mode.ToString();

            return retVal;
        }

        /// <summary>
        /// Add or create a Region array to send to the GSLB service 
        /// </summary>
        /// <param name="currenetGSLBRegionArray">The LoadBalanceAddress array to add the LoadBalanceAddress to or null to start a new array</param>
        /// <param name="poolGSLBAddresses">The pool of ip addresses for the added region</param>
        /// <param name="region">GSLB region location</param>
        /// <param name="serveCount">How many records will be returned in each DNS response or -1 to not specify</param>
        /// <param name="failoverMode">Dynect default is 'global': ip - Failover to a particular IP, cname - Failover to a particular CNAME, global - Failover to the global IP address pool</param>
        /// <param name="failoverData">If failover_mode is 'ip', this should be an IPv4 address, If failover_mode is 'cname', this should be a CNAME, If failover_mode is 'global' this should be null or empty</param>
        /// <returns>An array of GSLBRegions to pass to update or create</returns>
        public net.dynect.api2.GSLBRegion[] AddGSLBRegion(net.dynect.api2.GSLBRegion[] currenetGSLBRegionArray, net.dynect.api2.GSLBAddress[] poolGSLBAddresses,
                                                GSLBRegionOption region, int serveCount, FailoverMode failoverMode, string failoverData)
        {
            net.dynect.api2.GSLBRegion[] retVal = null;
            int index = 0;
            if (currenetGSLBRegionArray == null)
            {
                retVal = new DynSoapWrapper.net.dynect.api2.GSLBRegion[1];
            }
            else
            {
                retVal = new DynSoapWrapper.net.dynect.api2.GSLBRegion[currenetGSLBRegionArray.Length + 1];
                for (index = 0; index < currenetGSLBRegionArray.Length; index++)
                {
                    retVal[index] = currenetGSLBRegionArray[index];
                }
            }

            retVal[index] = new DynSoapWrapper.net.dynect.api2.GSLBRegion();
            retVal[index].failover_data = failoverMode == FailoverMode.global ? string.Empty : failoverData;
            retVal[index].failover_mode = failoverMode.ToString();
            retVal[index].region_code = GSLBRegionToString(region);
            if (serveCount > 0)
            {
                retVal[index].serve_count = serveCount;
                retVal[index].serve_countSpecified = true;
            }
            else
            {
                retVal[index].serve_countSpecified = false;
            }

            retVal[index].pool = poolGSLBAddresses;

            return retVal;
        }

        /// <summary>
        /// Call this to build the the MonitorData to send to the load balance service 
        /// </summary>
        /// <param name="protocol">The protocol to monitor </param>
        /// <param name="interval">How often to run the monitor. </param>
        /// <param name="retries">How many retries the monitor should attempt on failure before giving up or -1 to use default</param>
        /// <param name="port">For HTTP(S)/SMTP probes, an alternate port to connect to or -1 to use the default port</param>
        /// <param name="path">For HTTP(S) probes, a specific path to request (can be string.empty)</param>
        /// <param name="host">For HTTP(S) probes, a value to pass in to the Host: header (can be string.empty)</param>
        /// <param name="headers">For HTTP(S) probes, additional header fields/values to pass in</param>
        /// <param name="expected">For HTTP(S) probes, a string to search for in the response. For SMTP probes, a string to compare the banner against. Failure to find this string means the monitor will report a 'down' status</param>
        /// <returns>The built MonitorData object to be passed to a Create or Update LoadBalance</returns>
        public net.dynect.api2.MonitorData BuildMonitorData(MonitorProtocol protocol, MonitorInterval interval, int retries, 
                                                    int port, string path, string host, string[] headers, string expected)
        {
            net.dynect.api2.MonitorData md = new DynSoapWrapper.net.dynect.api2.MonitorData();
            try
            {
                md.protocol = protocol.ToString();
                md.interval = (int)interval;
                if (retries > -1)
                {
                    md.retries = retries;
                    md.retriesSpecified = true;
                }
                else
                {
                    md.retriesSpecified = false;
                }

                if (port > -1)
                {
                    md.port = port;
                    md.portSpecified = true;
                }
                else
                {
                    md.portSpecified = false;
                }

                md.path = path;
                md.expected = expected;

                md.host = host;

                string headerString = String.Join("\\n", headers);
                md.header = headerString;

            }
            catch (Exception ex)
            {
                // TODO: log this exception somewhere....
            }
            return md;
        }

        #endregion

        #region LoadBalancing

        /// <summary>
        /// Creates the load balance service on the node
        /// </summary>
        /// <param name="zone">The zone to attach the LoadBalance to</param>
        /// <param name="fqdn">The fqdb to attach the LoadBalance to</param>
        /// <param name="contactNickname">Name of contact to receive notifications</param>
        /// <param name="loadBalancePool">The IP Pool list for this service</param>
        /// <param name="monitorData">The health monitor for the service</param>
        /// <param name="autoRecover">True if service should come out of failover automatically when IPs come back up, False if the service should stay in failover until a user explicitly takes the service out of failover</param>
        /// <param name="ttl">Time To Live in seconds of records in the service. Must be less than 1/2 of the Health Probe's monitoring interval or "None" to not specify</param>
        /// <param name="notifyEvents">What events to send notifications on: ip - Send notifications when individual IPs change status, svc - Send notifications when the service state change</param>
        /// <param name="serveCount">How many records will be returned in each DNS response or -1 to not specify</param>
        /// <param name="failoverMode">Dynect default is 'global': ip - Failover to a particular IP, cname - Failover to a particular CNAME, global - Failover to the global IP address pool</param>
        /// <param name="failoverData">If failover_mode is 'ip', this should be an IPv4 address, If failover_mode is 'cname', this should be a CNAME, If failover_mode is 'global' this should be null or empty</param>
        /// <returns>The created load balance data or null if failed to create</returns>
        public net.dynect.api2.LoadBalanceData CreateLoadBalance(string zone, string fqdn, string contactNickname, net.dynect.api2.LoadBalanceAddress[] loadBalancePool,
                                                net.dynect.api2.MonitorData monitorData, Boolean autoRecover, TTLSeconds ttl, MonitorNotificationEvent notifyEvents, int serveCount,
                                                FailoverMode failoverMode, string failoverData)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData retVal = null;
            try
            {
                net.dynect.api2.CreateLoadBalanceRequestType request = new DynSoapWrapper.net.dynect.api2.CreateLoadBalanceRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.failover_mode = failoverMode.ToString();
                request.auto_recover = autoRecover ? "Y" : "N";
                request.contact_nickname = contactNickname;
                request.failover_data = failoverMode == FailoverMode.global ? string.Empty : failoverData;
                request.monitor = monitorData;
                request.notify_events = MonitorNotificationEventToString(notifyEvents);
                request.pool = loadBalancePool;
                if (serveCount > -1)
                {
                    request.serve_count = serveCount;
                    request.serve_countSpecified = true;
                }
                else
                {
                    request.serve_countSpecified = false;
                }
                if (ttl == TTLSeconds.NONE)
                {
                    request.ttlSpecified = false;
                }
                else
                {
                    request.ttl = (int)ttl;
                    request.ttlSpecified = true;
                }
                net.dynect.api2.CreateLoadBalanceResponseType response = dynectWsdl.CreateLoadBalance(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Updates the load balance service on the node
        /// </summary>
        /// <param name="zone">The zone to attach the LoadBalance to</param>
        /// <param name="fqdn">The fqdb to attach the LoadBalance to</param>
        /// <param name="contactNickname">Name of contact to receive notifications</param>
        /// <param name="loadBalancePool">The IP Pool list for this service</param>
        /// <param name="monitorData">The health monitor for the service</param>
        /// <param name="autoRecover">True if service should come out of failover automatically when IPs come back up, False if the service should stay in failover until a user explicitly takes the service out of failover</param>
        /// <param name="ttl">Time To Live in seconds of records in the service. Must be less than 1/2 of the Health Probe's monitoring interval or "None" to not specify</param>
        /// <param name="notifyEvents">What events to send notifications on: ip - Send notifications when individual IPs change status, svc - Send notifications when the service state change</param>
        /// <param name="serveCount">How many records will be returned in each DNS response or -1 to not specify</param>
        /// <param name="failoverMode">Dynect default is 'global': ip - Failover to a particular IP, cname - Failover to a particular CNAME, global - Failover to the global IP address pool</param>
        /// <param name="failoverData">If failover_mode is 'ip', this should be an IPv4 address, If failover_mode is 'cname', this should be a CNAME, If failover_mode is 'global' this should be null or empty</param>
        /// <returns>The updated load balance data or null if failed to create</returns>
        public net.dynect.api2.LoadBalanceData UpdateLoadBalance(string zone, string fqdn, string contactNickname, net.dynect.api2.LoadBalanceAddress[] loadBalancePool,
                                                net.dynect.api2.MonitorData monitorData, Boolean autoRecover, TTLSeconds ttl, MonitorNotificationEvent notifyEvents, int serveCount,
                                                FailoverMode failoverMode, string failoverData)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData retVal = null;
            try
            {
                net.dynect.api2.UpdateLoadBalanceRequestType request = new DynSoapWrapper.net.dynect.api2.UpdateLoadBalanceRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.failover_mode = failoverMode.ToString();
                request.auto_recover = autoRecover ? "Y" : "N";
                request.contact_nickname = contactNickname;
                request.failover_data = failoverMode == FailoverMode.global ? string.Empty : failoverData;
                request.monitor = monitorData;
                request.notify_events = MonitorNotificationEventToString(notifyEvents);
                request.pool = loadBalancePool;
                if (serveCount > -1)
                {
                    request.serve_count = serveCount;
                    request.serve_countSpecified = true;
                }
                else
                {
                    request.serve_countSpecified = false;
                }
                if (ttl == TTLSeconds.NONE)
                {
                    request.ttlSpecified = false;
                }
                else
                {
                    request.ttl = (int)ttl;
                    request.ttlSpecified = true;
                }
                net.dynect.api2.UpdateLoadBalanceResponseType response = dynectWsdl.UpdateLoadBalance(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Get all of the LoadBalanceData service instances in the zone
        /// </summary>
        /// <param name="zone">The zone to get the LoadBalanceData of</param>
        /// <returns> An array of LoadBalanceData for all fqdn in the zone</returns>
        public net.dynect.api2.LoadBalanceData[] GetLoadBalances(string zone)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData[] retVal = null;
            try
            {
                net.dynect.api2.GetLoadBalancesRequestType request = new DynSoapWrapper.net.dynect.api2.GetLoadBalancesRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                net.dynect.api2.GetLoadBalancesResponseType response = dynectWsdl.GetLoadBalances(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Get the LoadBalanceData for a specific zone and fqdn
        /// </summary>
        /// <param name="zone">The zone to get the LoadBalanceData of</param>
        /// <param name="fqdn">The fqdn to get the LoadBalanceData of</param>
        /// <returns>The LoadBalanceData object or null if failed</returns>
        public net.dynect.api2.LoadBalanceData GetOneLoadBalance(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData retVal = null;
            try
            {
                net.dynect.api2.GetOneLoadBalanceRequestType request = new DynSoapWrapper.net.dynect.api2.GetOneLoadBalanceRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.GetOneLoadBalanceResponseType response = dynectWsdl.GetOneLoadBalance(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deletes an existing Load Balancing service for a specific zone and fqdn
        /// </summary>
        /// <param name="zone">The zone to delete the LoadBalanceData from</param>
        /// <param name="fqdn">The fqdn to delete the LoadBalanceData from</param>
        /// <returns>True if no errors are encountered during the delete</returns>
        public Boolean DeleteOneLoadBalance(string zone, string fqdn)
        {
            if (sessionData == null)
                return false;

            Boolean retVal = false;
            try
            {
                net.dynect.api2.DeleteOneLoadBalanceRequestType request = new DynSoapWrapper.net.dynect.api2.DeleteOneLoadBalanceRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.DeleteOneLoadBalanceResponseType response = dynectWsdl.DeleteOneLoadBalance(request);

                retVal = true;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Activates an existing Load Balancing service for a specific zone and fqdn
        /// </summary>
        /// <param name="zone">The zone to activate the LoadBalanceData from</param>
        /// <param name="fqdn">The fqdn to activate the LoadBalanceData on</param>
        /// <returns>The activated LoadBalanceData or null if failed</returns>
        public net.dynect.api2.LoadBalanceData ActivateLoadBalance(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData retVal = null;
            try
            {
                net.dynect.api2.ActivateLoadBalanceRequestType request = new DynSoapWrapper.net.dynect.api2.ActivateLoadBalanceRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.ActivateLoadBalanceResponseType response = dynectWsdl.ActivateLoadBalance(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deactivates an existing Load Balancing service for a specific zone and fqdn
        /// </summary>
        /// <param name="zone">The zone to deactivate the LoadBalanceData on</param>
        /// <param name="fqdn">The fqdn to deactivate the LoadBalanceData on</param>
        /// <returns>The deactivated LoadBalanceData or null if failed</returns>
        public net.dynect.api2.LoadBalanceData DeactivateLoadBalance(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData retVal = null;
            try
            {
                net.dynect.api2.DeactivateLoadBalanceRequestType request = new DynSoapWrapper.net.dynect.api2.DeactivateLoadBalanceRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.DeactivateLoadBalanceResponseType response = dynectWsdl.DeactivateLoadBalance(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Recovers an existing Load Balancing service for a specific zone and fqdn
        /// </summary>
        /// <param name="zone">The zone to recover the LoadBalanceData on</param>
        /// <param name="fqdn">The fqdn to recover the LoadBalanceData on</param>
        /// <returns>The recovered LoadBalanceData or null if failed</returns>
        public net.dynect.api2.LoadBalanceData RecoverLoadBalance(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData retVal = null;
            try
            {
                net.dynect.api2.RecoverLoadBalanceRequestType request = new DynSoapWrapper.net.dynect.api2.RecoverLoadBalanceRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.RecoverLoadBalanceResponseType response = dynectWsdl.RecoverLoadBalance(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Recovers a node IP from an existing Load Balancing service
        /// </summary>
        /// <param name="zone">The zone to recover the LoadBalanceData on</param>
        /// <param name="fqdn">The fqdn to recover the LoadBalanceData on</param>
        /// <param name="address">The ip address to recover</param>
        /// <returns>The recovered LoadBalanceData or null if failed</returns>
        public net.dynect.api2.LoadBalanceData RecoverLoadBalanceIP(string zone, string fqdn, string address)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalanceData retVal = null;
            try
            {
                net.dynect.api2.RecoverLoadBalanceIPRequestType request = new DynSoapWrapper.net.dynect.api2.RecoverLoadBalanceIPRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                net.dynect.api2.RecoverLoadBalanceIPResponseType response = dynectWsdl.RecoverLoadBalanceIP(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Creates a new entry in a Load Balancing service pool
        /// </summary>
        /// <param name="zone">The zone to add the entry to</param>
        /// <param name="fqdn">The fqdn to add the entry to</param> 
        /// <param name="address">The IPv4 address of this Node IP</param>
        /// <param name="label">A descriptive string describing this IP (may be string.empty)</param>
        /// <param name="weight">A number from 1-15 describing how often this record should be served. Higher means more</param>
        /// <param name="mode">Sets the behavior of this particular record. always - always serve this IP address, obey - Serve this address based upon its monitoring status, remove - Serve this address based upon its monitoring status. However, if it goes down, don't automatically bring it back up when monitoring reports it up, no - Never serve this IP address</param>
        /// <returns>The LoadBalancePoolEntry that is created or null if failed</returns>
        public net.dynect.api2.LoadBalancePoolEntry CreateLoadBalancePoolEntry(string zone, string fqdn, string address, string label, IPWeight weight, IPServeMode mode)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalancePoolEntry retVal = null;
            try
            {
                net.dynect.api2.CreateLoadBalancePoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.CreateLoadBalancePoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                request.label = label;
                if (weight != IPWeight.NONE)
                    request.weight = (int)weight;
                else
                    request.weight = 1;
                request.serve_mode = mode.ToString();
                net.dynect.api2.CreateLoadBalancePoolEntryResponseType response = dynectWsdl.CreateLoadBalancePoolEntry(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Gets all entries from a Load Balancing service pool
        /// </summary>
        /// <param name="zone">The zone to get the entry from</param>
        /// <param name="fqdn">The fqdn to get the entry from</param> 
        /// <returns>Array of LoadBalancePoolEntry objects</returns>
        public net.dynect.api2.LoadBalancePoolEntry[] GetLoadBalancePoolEntries(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalancePoolEntry[] retVal = null;
            try
            {
                net.dynect.api2.GetLoadBalancePoolEntriesRequestType request = new DynSoapWrapper.net.dynect.api2.GetLoadBalancePoolEntriesRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.GetLoadBalancePoolEntriesResponseType response = dynectWsdl.GetLoadBalancePoolEntries(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Gets an entry from a Load Balancing service pool based on the ip address
        /// </summary>
        /// <param name="zone">The zone to get the entry from</param>
        /// <param name="fqdn">The fqdn to get the entry from</param> 
        /// <param name="address">The address to get the entry of</param> 
        /// <returns>Array of LoadBalancePoolEntry objects</returns>
        public net.dynect.api2.LoadBalancePoolEntry GetOneLoadBalancePoolEntry(string zone, string fqdn, string address)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalancePoolEntry retVal = null;
            try
            {
                net.dynect.api2.GetOneLoadBalancePoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.GetOneLoadBalancePoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                net.dynect.api2.GetOneLoadBalancePoolEntryResponseType response = dynectWsdl.GetOneLoadBalancePoolEntry(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Updates an existing entry in a Load Balancing service poo
        /// </summary>
        /// <param name="zone">The zone to update the entry in</param>
        /// <param name="fqdn">The fqdn to update the entry in</param> 
        /// <param name="address">The IPv4 address of this Node IP to update</param>
        /// <param name="label">A descriptive string describing this IP (may be string.empty)</param>
        /// <param name="weight">A number from 1-15 describing how often this record should be served. Higher means more</param>
        /// <param name="mode">Sets the behavior of this particular record. always - always serve this IP address, obey - Serve this address based upon its monitoring status, remove - Serve this address based upon its monitoring status. However, if it goes down, don't automatically bring it back up when monitoring reports it up, no - Never serve this IP address</param>
        /// <returns>The LoadBalancePoolEntry that is created or null if failed</returns>
        public net.dynect.api2.LoadBalancePoolEntry UpdateLoadBalancePoolEntry(string zone, string fqdn, string address, string label, IPWeight weight, IPServeMode mode)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.LoadBalancePoolEntry retVal = null;
            try
            {
                net.dynect.api2.UpdateLoadBalancePoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.UpdateLoadBalancePoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                request.label = label;
                if (weight != IPWeight.NONE)
                    request.weight = (int)weight;
                else
                    request.weight = 1;
                request.serve_mode = mode.ToString();
                net.dynect.api2.UpdateLoadBalancePoolEntryResponseType response = dynectWsdl.UpdateLoadBalancePoolEntry(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Deletes an entry from a Load Balancing service pool based on ip address
        /// </summary>
        /// <param name="zone">The zone to remove the entry from</param>
        /// <param name="fqdn">The fqdn to remove the entry from</param> 
        /// <param name="address">The address of the entry to remove</param> 
        /// <returns>True if no errors are encountered</returns>
        public Boolean DeleteOneLoadBalancePoolEntry(string zone, string fqdn, string address)
        {
            if (sessionData == null)
                return false;

            Boolean retVal = false;
            try
            {
                net.dynect.api2.DeleteOneLoadBalancePoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.DeleteOneLoadBalancePoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                net.dynect.api2.DeleteOneLoadBalancePoolEntryResponseType response = dynectWsdl.DeleteOneLoadBalancePoolEntry(request);


                retVal = true;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        #endregion

        #region Failover

        /// <summary>
        /// Creates a new Failover service at a zone/node
        /// </summary>
        /// <param name="zone">zone to create failover at</param>
        /// <param name="fqdn">address to create failover at</param>
        /// <param name="address">ip address</param>
        /// <param name="failoverAddress">ip address to failover to</param>
        /// <param name="autoRecover"> true if the service should come out of failover automatically when IPs come back up, false if the service should stay in failover until a user explicitly takes the service out of failover</param>
        /// <param name="notifyEvents">What events to send notifications on: ip - Send notifications when individual IPs change status, svc - Send notifications when the service state changes</param>
        /// <param name="monitorData">The health monitor for the service</param>
        /// <param name="contactNickname">Name of contact to receive notifications</param>
        /// <param name="ttl">Time To Live in seconds of records in the service. Must be less than 1/2 of the Health Probe's monitoring interval.</param>
        /// <returns>The created service object if successfull</returns>
        public net.dynect.api2.FailoverData CreateFailover(string zone, string fqdn, string address, string failoverAddress, Boolean autoRecover, MonitorNotificationEvent notifyEvents,
                                                net.dynect.api2.MonitorData monitorData, string contactNickname, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.FailoverData retVal = null;
            try
            {
                net.dynect.api2.CreateFailoverRequestType request = new DynSoapWrapper.net.dynect.api2.CreateFailoverRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                request.failover_address = failoverAddress;
                request.auto_recover = autoRecover ? "Y" : "N";
                request.notify_events = MonitorNotificationEventToString(notifyEvents);
                request.monitor = monitorData;
                request.contact_nickname = contactNickname;
                if (ttl == TTLSeconds.NONE)
                {
                    request.ttlSpecified = false;
                }
                else
                {
                    request.ttl = (int)ttl;
                    request.ttlSpecified = true;
                }
                net.dynect.api2.CreateFailoverResponseType response = dynectWsdl.CreateFailover(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves information about all existing Failover services in a zone
        /// </summary>
        /// <param name="zone">The zone to get the failover services for</param>
        /// <returns>an array of all of the failover services if sucessfull</returns>
        public net.dynect.api2.FailoverData[] GetFailovers(string zone)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.FailoverData[] retVal = null;
            try
            {
                net.dynect.api2.GetFailoversRequestType request = new DynSoapWrapper.net.dynect.api2.GetFailoversRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                net.dynect.api2.GetFailoversResponseType response = dynectWsdl.GetFailovers(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves information about all existing Failover services in a zone
        /// </summary>
        /// <param name="zone">The zone to get the failover service for</param>
        /// <param name="fqdn">The fqdn to get the failover service for</param>
        /// <returns>the failover service if sucessfull</returns>
        public net.dynect.api2.FailoverData GetOneFailover(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.FailoverData retVal = null;
            try
            {
                net.dynect.api2.GetOneFailoverRequestType request = new DynSoapWrapper.net.dynect.api2.GetOneFailoverRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.GetOneFailoverResponseType response = dynectWsdl.GetOneFailover(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Updates an existing Failover service
        /// </summary>
        /// <param name="zone">zone to create failover at</param>
        /// <param name="fqdn">address to create failover at</param>
        /// <param name="address">ip address</param>
        /// <param name="failoverAddress">ip address to failover to</param>
        /// <param name="autoRecover"> true if the service should come out of failover automatically when IPs come back up, false if the service should stay in failover until a user explicitly takes the service out of failover</param>
        /// <param name="notifyEvents">What events to send notifications on: ip - Send notifications when individual IPs change status, svc - Send notifications when the service state changes</param>
        /// <param name="monitorData">The health monitor for the service</param>
        /// <param name="contactNickname">Name of contact to receive notifications</param>
        /// <param name="ttl">Time To Live in seconds of records in the service. Must be less than 1/2 of the Health Probe's monitoring interval.</param>
        /// <returns>The updated service object if successfull</returns>
        public net.dynect.api2.FailoverData UpdateFailover(string zone, string fqdn, string address, string failoverAddress, Boolean autoRecover, MonitorNotificationEvent notifyEvents,
                                                net.dynect.api2.MonitorData monitorData, string contactNickname, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.FailoverData retVal = null;
            try
            {
                net.dynect.api2.UpdateFailoverRequestType request = new DynSoapWrapper.net.dynect.api2.UpdateFailoverRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                request.failover_address = failoverAddress;
                request.auto_recover = autoRecover ? "Y" : "N";
                request.notify_events = MonitorNotificationEventToString(notifyEvents);
                request.monitor = monitorData;
                request.contact_nickname = contactNickname;
                if (ttl == TTLSeconds.NONE)
                {
                    request.ttlSpecified = false;
                }
                else
                {
                    request.ttl = (int)ttl;
                    request.ttlSpecified = true;
                }
                net.dynect.api2.UpdateFailoverResponseType response = dynectWsdl.UpdateFailover(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deletes an existing Failover service
        /// </summary>
        /// <param name="zone">The zone to get the failover service to remove</param>
        /// <param name="fqdn">The fqdn to get the failover service remove</param>
        /// <returns>true if no errors are encountered</returns>
        public Boolean DeleteOneFailover(string zone, string fqdn)
        {
            if (sessionData == null)
                return false;

            Boolean retVal = false;
            try
            {
                net.dynect.api2.DeleteOneFailoverRequestType request = new DynSoapWrapper.net.dynect.api2.DeleteOneFailoverRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.DeleteOneFailoverResponseType response = dynectWsdl.DeleteOneFailover(request);

                retVal = true;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Activates an existing Failover service
        /// </summary>
        /// <param name="zone">The zone to activate the failover service on</param>
        /// <param name="fqdn">The fqdn to activate the failover service on</param>
        /// <returns>the failover service if sucessfull</returns>
        public net.dynect.api2.FailoverData ActivateFailover(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.FailoverData retVal = null;
            try
            {
                net.dynect.api2.ActivateFailoverRequestType request = new DynSoapWrapper.net.dynect.api2.ActivateFailoverRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.ActivateFailoverResponseType response = dynectWsdl.ActivateFailover(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deactivates an existing Failover service
        /// </summary>
        /// <param name="zone">The zone to deactivate the failover service on</param>
        /// <param name="fqdn">The fqdn to deactivate the failover service on</param>
        /// <returns>the failover service if sucessfull</returns>
        public net.dynect.api2.FailoverData DeactivateFailover(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.FailoverData retVal = null;
            try
            {
                net.dynect.api2.DeactivateFailoverRequestType request = new DynSoapWrapper.net.dynect.api2.DeactivateFailoverRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.DeactivateFailoverResponseType response = dynectWsdl.DeactivateFailover(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Recovers an existing Failover service
        /// </summary>
        /// <param name="zone">The zone to deactivate the failover service on</param>
        /// <param name="fqdn">The fqdn to deactivate the failover service on</param>
        /// <returns>the failover service if sucessfull</returns>
        public net.dynect.api2.FailoverData RecoverFailover(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.FailoverData retVal = null;
            try
            {
                net.dynect.api2.RecoverFailoverRequestType request = new DynSoapWrapper.net.dynect.api2.RecoverFailoverRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.RecoverFailoverResponseType response = dynectWsdl.RecoverFailover(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        #endregion

        #region GSLB

        /// <summary>
        /// Creates a new Global Server Load Balancing service
        /// </summary>
        /// <param name="zone">zone to create GSLB at</param>
        /// <param name="fqdn">address to create GSLB at</param>
        /// <param name="regions">a list of regions</param>
        /// <param name="autoRecover"> true if the service should come out of failover automatically when IPs come back up, false if the service should stay in failover until a user explicitly takes the service out of failover</param>
        /// <param name="notifyEvents">What events to send notifications on: ip - Send notifications when individual IPs change status, svc - Send notifications when the service state changes</param>
        /// <param name="monitorData">The health monitor for the service</param>
        /// <param name="contactNickname">Name of contact to receive notifications</param>
        /// <param name="ttl">Time To Live in seconds of records in the service. Must be less than 1/2 of the Health Probe's monitoring interval.</param>
        /// <returns>The created service object if successfull</returns>
        public net.dynect.api2.GSLBData CreateGSLB(string zone, string fqdn, net.dynect.api2.GSLBRegion[] regions, Boolean autoRecover, MonitorNotificationEvent notifyEvents,
                                                net.dynect.api2.MonitorData monitorData, string contactNickname, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData retVal = null;
            try
            {
                net.dynect.api2.CreateGSLBRequestType request = new DynSoapWrapper.net.dynect.api2.CreateGSLBRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn; 
                request.region = regions;
                request.auto_recover = autoRecover ? "Y" : "N";
                request.notify_events = MonitorNotificationEventToString(notifyEvents);
                request.monitor = monitorData;
                request.contact_nickname = contactNickname;
                if (ttl == TTLSeconds.NONE)
                {
                    request.ttlSpecified = false;
                }
                else
                {
                    request.ttl = (int)ttl;
                    request.ttlSpecified = true;
                }
                net.dynect.api2.CreateGSLBResponseType response = dynectWsdl.CreateGSLB(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Updates an existing Global Server Load Balancing service
        /// </summary>
        /// <param name="zone">zone to create GSLB at</param>
        /// <param name="fqdn">address to create GSLB at</param>
        /// <param name="regions">a list of regions</param>
        /// <param name="autoRecover"> true if the service should come out of failover automatically when IPs come back up, false if the service should stay in failover until a user explicitly takes the service out of failover</param>
        /// <param name="notifyEvents">What events to send notifications on: ip - Send notifications when individual IPs change status, svc - Send notifications when the service state changes</param>
        /// <param name="monitorData">The health monitor for the service</param>
        /// <param name="contactNickname">Name of contact to receive notifications</param>
        /// <param name="ttl">Time To Live in seconds of records in the service. Must be less than 1/2 of the Health Probe's monitoring interval.</param>
        /// <returns>The created service object if successfull</returns>
        public net.dynect.api2.GSLBData UpdateGSLB(string zone, string fqdn, net.dynect.api2.GSLBRegion[] regions, Boolean autoRecover, MonitorNotificationEvent notifyEvents,
                                                net.dynect.api2.MonitorData monitorData, string contactNickname, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData retVal = null;
            try
            {
                net.dynect.api2.UpdateGSLBRequestType request = new DynSoapWrapper.net.dynect.api2.UpdateGSLBRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.region = regions;
                request.auto_recover = autoRecover ? "Y" : "N";
                request.notify_events = MonitorNotificationEventToString(notifyEvents);
                request.monitor = monitorData;
                request.contact_nickname = contactNickname;
                if (ttl == TTLSeconds.NONE)
                {
                    request.ttlSpecified = false;
                }
                else
                {
                    request.ttl = (int)ttl;
                    request.ttlSpecified = true;
                }
                net.dynect.api2.UpdateGSLBResponseType response = dynectWsdl.UpdateGSLB(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves information about all existing Global Server Load Balancing services in a zone
        /// </summary>
        /// <param name="zone">The zone to get the GSLB services for</param>
        /// <returns>an array of all of the GSLB services if sucessfull</returns>
        public net.dynect.api2.GSLBData[] GetGSLBs(string zone)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData[] retVal = null;
            try
            {
                net.dynect.api2.GetGSLBsRequestType request = new DynSoapWrapper.net.dynect.api2.GetGSLBsRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                net.dynect.api2.GetGSLBsResponseType response = dynectWsdl.GetGSLBs(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Retrieves information about an existing Global Server Load Balancing service
        /// </summary>
        /// <param name="zone">The zone to get the gslb service for</param>
        /// <param name="fqdn">The fqdn to get the gslb service for</param>
        /// <returns>the gslb service if sucessfull</returns>
        public net.dynect.api2.GSLBData GetOneGSLB(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData retVal = null;
            try
            {
                net.dynect.api2.GetOneGSLBRequestType request = new DynSoapWrapper.net.dynect.api2.GetOneGSLBRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.GetOneGSLBResponseType response = dynectWsdl.GetOneGSLB(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deletes an existing GSLB service
        /// </summary>
        /// <param name="zone">The zone to get the GSLB service to remove</param>
        /// <param name="fqdn">The fqdn to get the GSLB service remove</param>
        /// <returns>true if no errors are encountered</returns>
        public Boolean DeleteOneGSLB(string zone, string fqdn)
        {
            if (sessionData == null)
                return false;

            Boolean retVal = false;
            try
            {
                net.dynect.api2.DeleteOneGSLBRequestType request = new DynSoapWrapper.net.dynect.api2.DeleteOneGSLBRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.DeleteOneGSLBResponseType response = dynectWsdl.DeleteOneGSLB(request);

                retVal = true;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Activates an existing GSLB service
        /// </summary>
        /// <param name="zone">The zone to activate the GSLB service on</param>
        /// <param name="fqdn">The fqdn to activate the GSLB service on</param>
        /// <returns>the GSLB service if sucessfull</returns>
        public net.dynect.api2.GSLBData ActivateGSLB(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData retVal = null;
            try
            {
                net.dynect.api2.ActivateGSLBRequestType request = new DynSoapWrapper.net.dynect.api2.ActivateGSLBRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.ActivateGSLBResponseType response = dynectWsdl.ActivateGSLB(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deactivates an existing GSLB service
        /// </summary>
        /// <param name="zone">The zone to deactivate the GSLB service on</param>
        /// <param name="fqdn">The fqdn to deactivate the GSLB service on</param>
        /// <returns>the GSLB service if sucessfull</returns>
        public net.dynect.api2.GSLBData DeactivateGSLB(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData retVal = null;
            try
            {
                net.dynect.api2.DeactivateGSLBRequestType request = new DynSoapWrapper.net.dynect.api2.DeactivateGSLBRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.DeactivateGSLBResponseType response = dynectWsdl.DeactivateGSLB(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Recovers an existing GSLB service
        /// </summary>
        /// <param name="zone">The zone to deactivate the GSLB service on</param>
        /// <param name="fqdn">The fqdn to deactivate the GSLB service on</param>
        /// <returns>the GSLB service if sucessfull</returns>
        public net.dynect.api2.GSLBData RecoverGSLB(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData retVal = null;
            try
            {
                net.dynect.api2.RecoverGSLBRequestType request = new DynSoapWrapper.net.dynect.api2.RecoverGSLBRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.RecoverGSLBResponseType response = dynectWsdl.RecoverGSLB(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Recovers a node IP from an existing GSLB service
        /// </summary>
        /// <param name="zone">The zone to recover the GSLB on</param>
        /// <param name="fqdn">The fqdn to recover the GSLB on</param>
        /// <param name="address">The ip address to recover</param>
        /// <returns>The recovered GSLB or null if failed</returns>
        public net.dynect.api2.GSLBData RecoverGSLBIP(string zone, string fqdn, string address)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBData retVal = null;
            try
            {
                net.dynect.api2.RecoverGSLBIPRequestType request = new DynSoapWrapper.net.dynect.api2.RecoverGSLBIPRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                net.dynect.api2.RecoverGSLBIPResponseType response = dynectWsdl.RecoverGSLBIP(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Creates a new entry in a GSLB Region pool
        /// </summary>
        /// <param name="zone">The zone to add the entry to</param>
        /// <param name="fqdn">The fqdn to add the entry to</param> 
        /// <param name="address">The IPv4 address of this Node IP</param>
        /// <param name="region">The region to create the pool entry for</param>
        /// <param name="label">A descriptive string describing this IP (may be string.empty)</param>
        /// <param name="weight">A number from 1-15 describing how often this record should be served. Higher means more</param>
        /// <param name="mode">Sets the behavior of this particular record. always - always serve this IP address, obey - Serve this address based upon its monitoring status, remove - Serve this address based upon its monitoring status. However, if it goes down, don't automatically bring it back up when monitoring reports it up, no - Never serve this IP address</param>
        /// <returns>The GSLBRegionPoolEntry that is created or null if failed</returns>
        public net.dynect.api2.GSLBRegionPoolEntry CreateGSLBRegionPoolEntry(string zone, string fqdn, GSLBRegionOption region, string address, string label, IPWeight weight, IPServeMode mode)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBRegionPoolEntry retVal = null;
            try
            {
                net.dynect.api2.CreateGSLBRegionPoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.CreateGSLBRegionPoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                request.label = label;
                request.region_code = GSLBRegionToString(region);
                if (weight != IPWeight.NONE)
                    request.weight = (int)weight;
                else
                    request.weight = 1;
                request.serve_mode = mode.ToString();
                net.dynect.api2.CreateGSLBRegionPoolEntryResponseType response = dynectWsdl.CreateGSLBRegionPoolEntry(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Gets all entries from a GSLB service pool
        /// </summary>
        /// <param name="zone">The zone to get the entry from</param>
        /// <param name="fqdn">The fqdn to get the entry from</param> 
        /// <param name="region">The region to get the entry from</param> 
        /// <returns>Array of GSLBRegionPoolEntry objects</returns>
        public net.dynect.api2.GSLBRegionPoolEntry[] GetGSLBRegionPoolEntries(string zone, string fqdn, GSLBRegionOption region)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBRegionPoolEntry[] retVal = null;
            try
            {
                net.dynect.api2.GetGSLBRegionPoolEntriesRequestType request = new DynSoapWrapper.net.dynect.api2.GetGSLBRegionPoolEntriesRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.region_code = GSLBRegionToString(region);
                net.dynect.api2.GetGSLBRegionPoolEntriesResponseType response = dynectWsdl.GetGSLBRegionPoolEntries(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Gets an entry from a Load Balancing service pool based on the ip address
        /// </summary>
        /// <param name="zone">The zone to get the entry from</param>
        /// <param name="fqdn">The fqdn to get the entry from</param> 
        /// <param name="address">The address to get the entry of</param> 
        /// <returns>Array of GSLBRegionPoolEntry objects</returns>
        public net.dynect.api2.GSLBRegionPoolEntry GetOneGSLBRegionPoolEntry(string zone, string fqdn, string address)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBRegionPoolEntry retVal = null;
            try
            {
                net.dynect.api2.GetOneGSLBRegionPoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.GetOneGSLBRegionPoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                net.dynect.api2.GetOneGSLBRegionPoolEntryResponseType response = dynectWsdl.GetOneGSLBRegionPoolEntry(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Updates an existing entry in a GSLB service pool
        /// </summary>
        /// <param name="zone">The zone to update the entry in</param>
        /// <param name="fqdn">The fqdn to update the entry in</param> 
        /// <param name="address">The IPv4 address of this Node IP to update</param>
        /// <param name="label">A descriptive string describing this IP (may be string.empty)</param>
        /// <param name="weight">A number from 1-15 describing how often this record should be served. Higher means more</param>
        /// <param name="mode">Sets the behavior of this particular record. always - always serve this IP address, obey - Serve this address based upon its monitoring status, remove - Serve this address based upon its monitoring status. However, if it goes down, don't automatically bring it back up when monitoring reports it up, no - Never serve this IP address</param>
        /// <returns>The GSLBRegionPoolEntry that is created or null if failed</returns>
        public net.dynect.api2.GSLBRegionPoolEntry UpdateGSLBRegionPoolEntry(string zone, string fqdn, string address, string label, IPWeight weight, IPServeMode mode)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.GSLBRegionPoolEntry retVal = null;
            try
            {
                net.dynect.api2.UpdateGSLBRegionPoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.UpdateGSLBRegionPoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                request.label = label;
                if (weight != IPWeight.NONE)
                    request.weight = (int)weight;
                else
                    request.weight = 1;
                request.serve_mode = mode.ToString();
                net.dynect.api2.UpdateGSLBRegionPoolEntryResponseType response = dynectWsdl.UpdateGSLBRegionPoolEntry(request);

                retVal = response.data;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }


        /// <summary>
        /// Deletes an entry from a GSLB region service pool based on ip address
        /// </summary>
        /// <param name="zone">The zone to remove the entry from</param>
        /// <param name="fqdn">The fqdn to remove the entry from</param> 
        /// <param name="address">The address of the entry to remove</param> 
        /// <returns>True if no errors are encountered</returns>
        public Boolean DeleteOneGSLBRegionPoolEntry(string zone, string fqdn, string address)
        {
            if (sessionData == null)
                return false;

            Boolean retVal = false;
            try
            {
                net.dynect.api2.DeleteOneGSLBRegionPoolEntryRequestType request = new DynSoapWrapper.net.dynect.api2.DeleteOneGSLBRegionPoolEntryRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.address = address;
                net.dynect.api2.DeleteOneGSLBRegionPoolEntryResponseType response = dynectWsdl.DeleteOneGSLBRegionPoolEntry(request);

                retVal = true;
            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        #endregion

        #region A Records
        /// <summary>
        /// Retrieves all existing A records at a zone/node
        /// </summary>
        /// <param name="zone">The zone to return the A records of</param>
        /// <param name="fqdn">The fqdn to return the A records of</param>
        /// <returns></returns>
        public net.dynect.api2.ARecordData[] GetARecords(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.ARecordData[] retVal = null;
            
            try
            {
                net.dynect.api2.GetARecordsRequestType request = new DynSoapWrapper.net.dynect.api2.GetARecordsRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.GetARecordsResponseType response = dynectWsdl.GetARecords(request);
                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Creates a new A record at a zone/node
        /// </summary>
        /// <param name="zone">The zone to create the A record in</param>
        /// <param name="fqdn">The fqdn to create the A record in</param>
        /// <param name="address">The ip address to add</param>
        /// <param name="ttl">The ttl to add</param>
        /// <returns></returns>
        public net.dynect.api2.ARecordData CreateARecord(string zone, string fqdn, string address, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.ARecordData retVal = null;

            try
            {
                net.dynect.api2.CreateARecordRequestType request = new DynSoapWrapper.net.dynect.api2.CreateARecordRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.rdata = new DynSoapWrapper.net.dynect.api2.RDataA();
                request.rdata.address = address;
                request.ttl = (int)ttl;
                net.dynect.api2.CreateARecordResponseType response = dynectWsdl.CreateARecord(request);
                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deletes an existing A record at a zone/node
        /// </summary>
        /// <param name="zone">The zone to remove the A record from</param>
        /// <param name="fqdn">The fqdn to remove the A record from</param>
        /// <param name="address">The ip address to remove</param>
        /// <returns></returns>
        public Boolean DeleteOneARecord(string zone, string fqdn, string address)
        {
            if (sessionData == null)
                return false;

            Boolean retVal = false;

            try
            {
                net.dynect.api2.DeleteOneARecordRequestType request = new DynSoapWrapper.net.dynect.api2.DeleteOneARecordRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.rdata = new DynSoapWrapper.net.dynect.api2.RDataA();
                request.rdata.address = address;
                net.dynect.api2.DeleteOneARecordResponseType response = dynectWsdl.DeleteOneARecord(request);
                retVal = true;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }
        
        /// <summary>
        /// Updates an existing A record at a zone/node
        /// </summary>
        /// <param name="zone">The zone to update the A record in</param>
        /// <param name="fqdn">The fqdn to update the A record in</param>
        /// <param name="address">The ip address to update</param>
        /// <param name="ttl">The ttl to update</param>
        /// <returns></returns>
        public net.dynect.api2.ARecordData UpdateARecord(string zone, string fqdn, string address, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.ARecordData retVal = null;

            try
            {
                net.dynect.api2.UpdateARecordRequestType request = new DynSoapWrapper.net.dynect.api2.UpdateARecordRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.rdata = new DynSoapWrapper.net.dynect.api2.RDataA();
                request.rdata.address = address;
                request.ttl = (int)ttl;
                net.dynect.api2.UpdateARecordResponseType response = dynectWsdl.UpdateARecord(request);
                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }
        #endregion

        #region CNAME Records
        /// <summary>
        /// Retrieves all existing CNAME records at a zone/node
        /// </summary>
        /// <param name="zone">The zone to return the CNAME records of</param>
        /// <param name="fqdn">The fqdn to return the CNAME records of</param>
        /// <returns></returns>
        public net.dynect.api2.CNAMERecordData[] GetCNAMERecords(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.CNAMERecordData[] retVal = null;

            try
            {
                net.dynect.api2.GetCNAMERecordsRequestType request = new DynSoapWrapper.net.dynect.api2.GetCNAMERecordsRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.GetCNAMERecordsResponseType response = dynectWsdl.GetCNAMERecords(request);
                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Creates a new CNAME record at a zone/node
        /// </summary>
        /// <param name="zone">The zone to create the CNAME record in</param>
        /// <param name="fqdn">The fqdn to create the CNAME record in</param>
        /// <param name="hostname">The hostname to add</param>
        /// <param name="ttl">The ttl to add</param>
        /// <returns></returns>
        public net.dynect.api2.CNAMERecordData CreateCNAMERecord(string zone, string fqdn, string hostname, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.CNAMERecordData retVal = null;

            try
            {
                net.dynect.api2.CreateCNAMERecordRequestType request = new DynSoapWrapper.net.dynect.api2.CreateCNAMERecordRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.rdata = new DynSoapWrapper.net.dynect.api2.RDataCNAME();
                request.rdata.cname = hostname;
                request.ttl = (int)ttl;
                net.dynect.api2.CreateCNAMERecordResponseType response = dynectWsdl.CreateCNAMERecord(request);
                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Deletes an existing CNAME record at a zone/node
        /// </summary>
        /// <param name="zone">The zone to remove the CNAME record from</param>
        /// <param name="fqdn">The fqdn to remove the CNAME record from</param>
        /// <param name="hostanme">The hostname to remove</param>
        /// <returns></returns>
        public Boolean DeleteOneCNAMERecord(string zone, string fqdn, string hostname)
        {
            if (sessionData == null)
                return false;

            Boolean retVal = false;

            try
            {
                net.dynect.api2.DeleteOneCNAMERecordRequestType request = new DynSoapWrapper.net.dynect.api2.DeleteOneCNAMERecordRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.rdata = new DynSoapWrapper.net.dynect.api2.RDataCNAME();
                request.rdata.cname = hostname;
                net.dynect.api2.DeleteOneCNAMERecordResponseType response = dynectWsdl.DeleteOneCNAMERecord(request);
                retVal = true;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        /// <summary>
        /// Updates an existing CNAME record at a zone/node
        /// </summary>
        /// <param name="zone">The zone to update the CNAME record in</param>
        /// <param name="fqdn">The fqdn to update the CNAME record in</param>
        /// <param name="hostname">The hostname to update</param>
        /// <param name="ttl">The ttl to update</param>
        /// <returns></returns>
        public net.dynect.api2.CNAMERecordData UpdateCNAMERecord(string zone, string fqdn, string hostname, TTLSeconds ttl)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.CNAMERecordData retVal = null;

            try
            {
                net.dynect.api2.UpdateCNAMERecordRequestType request = new DynSoapWrapper.net.dynect.api2.UpdateCNAMERecordRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                request.rdata = new DynSoapWrapper.net.dynect.api2.RDataCNAME();
                request.rdata.cname = hostname;
                request.ttl = (int)ttl;
                net.dynect.api2.UpdateCNAMERecordResponseType response = dynectWsdl.UpdateCNAMERecord(request);
                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }

        #endregion

        #region Any Records
        /// <summary>
        /// Retrieves all existing records of any type at a zone/node
        /// </summary>
        /// <param name="zone">The zone to return the records of</param>
        /// <param name="fqdn">The fqdn to return the records of</param>
        /// <returns></returns>
        public net.dynect.api2.ANYRecordData GetANYRecords(string zone, string fqdn)
        {
            if (sessionData == null)
                return null;

            net.dynect.api2.ANYRecordData retVal = null;

            try
            {
                net.dynect.api2.GetANYRecordsRequestType request = new DynSoapWrapper.net.dynect.api2.GetANYRecordsRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                request.zone = zone;
                request.fqdn = fqdn;
                net.dynect.api2.GetANYRecordsResponseType response = dynectWsdl.GetANYRecords(request);
                retVal = response.data;

            }
            catch (Exception ex)
            {
                ;// TODO: Do your custom error handling here....
            }

            return retVal;
        }
        #endregion

        #region Connect/Disconnect
        public Boolean SessionConnect(string customerName, string userName, string password)
        {
            try
            {
                net.dynect.api2.SessionLoginRequestType request = new DynSoapWrapper.net.dynect.api2.SessionLoginRequestType();
                request.customer_name = customerName;
                request.user_name = userName;
                request.password = password;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                net.dynect.api2.SessionLoginResponseType response = dynectWsdl.SessionLogin(request);

                sessionData = response.data;
               
            }
            catch (Exception ex)
            {
                sessionData = null;

                ;// TODO: Do your custom error handling here....
                return false;
            }

            if (sessionData == null)
                return false;

            return true;
        }

        public Boolean SessionDisconnect()
        {
            if (sessionData == null)
                return true;

            try
            {
                net.dynect.api2.SessionLogoutRequestType request = new DynSoapWrapper.net.dynect.api2.SessionLogoutRequestType();
                request.token = sessionData.token;
                request.fault_incompat = 1;
                request.fault_incompatSpecified = true;
                net.dynect.api2.SessionLogoutResponseType response = dynectWsdl.SessionLogout(request);

                sessionData = null;

            }
            catch (Exception ex)
            {
                sessionData = null;

                ;// TODO: Do your custom error handling here....
                return false;
            }

            return true;
        }
        #endregion
    }
}
