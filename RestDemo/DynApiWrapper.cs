using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace RestDemo
{
    public enum RecordType
    {
        A,
        AAAA,
        CNAME,
        MX,
        SRV,
        NS
    }

    public class DynApiWrapper
    {
        private string token;
        private DynnetWSApi.DynectClient clientProxy;

        public DynApiWrapper()
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);

            binding.Name = "DynectSOAP";
            binding.MaxReceivedMessageSize = int.MaxValue;

            EndpointAddress endpointAddress = new EndpointAddress(new Uri("https://api2.dynect.net/SOAP/"));

            clientProxy = new DynnetWSApi.DynectClient(binding, endpointAddress);
        }

        public bool GetJob(int jobid)
        {
            bool ifComplete = false;
            while (!ifComplete)
            {
                DynnetWSApi.GetJobRequestType rt = new DynnetWSApi.GetJobRequestType();
                rt.token = token;
                rt.job_id = jobid;

                DynnetWSApi.GetJobResponseType resposne = clientProxy.GetJob(rt);
                ifComplete = resposne.status.Trim().ToLower().CompareTo("success")==0 ? true : false;
            }

            return true;
        }

        public async Task<DynnetWSApi.ZoneData[]> GetZoneAsync()
        {
            DynnetWSApi.GetZonesRequestType requestType = new DynnetWSApi.GetZonesRequestType();
            requestType.token = token;

            Task<DynnetWSApi.GetZonesResponse> getResponseTask = clientProxy.GetZonesAsync(requestType);

            DynnetWSApi.GetZonesResponse response = await getResponseTask;
            
            Console.WriteLine(response.GetZonesResponse1.status);
            Console.WriteLine(response.GetZonesResponse1.job_id);
            Console.WriteLine(response.GetZonesResponse1.msgs);

            if (response.GetZonesResponse1.status.Trim().ToLower().CompareTo("incomplete") == 0)
            {
                while (true)
                {
                    Console.WriteLine("Try get incomplete job result {0}", response.GetZonesResponse1.job_id);

                    DynnetWSApi.GetJobRequestType getjob = new DynnetWSApi.GetJobRequestType();
                    getjob.token = token;
                    getjob.job_id = response.GetZonesResponse1.job_id;

                    Task<DynnetWSApi.GetJobResponse> getJobTask = clientProxy.GetJobAsync(getjob);

                    DynnetWSApi.GetJobResponse jobResponse = await getJobTask;

                    Console.WriteLine(jobResponse.GetJobResponse1.status);
                    Console.WriteLine(jobResponse.GetJobResponse1.job_id);
                    Console.WriteLine(jobResponse.GetJobResponse1.msgs);
                    Console.WriteLine(((DynnetWSApi.ZoneData[])jobResponse.GetJobResponse1.data).Length);
                    if (jobResponse.GetJobResponse1.status.Trim().ToLower().CompareTo("success") == 0 && ((DynnetWSApi.ZoneData[])jobResponse.GetJobResponse1.data).Length != 0)
                    {
                        foreach (DynnetWSApi.ZoneData zData in (DynnetWSApi.ZoneData[])jobResponse.GetJobResponse1.data)
                        {
                            Console.WriteLine(zData.zone);
                        }

                        return (DynnetWSApi.ZoneData[]) jobResponse.GetJobResponse1.data;
                    }
                }
            }

            foreach (DynnetWSApi.ZoneData zData in response.GetZonesResponse1.data)
            {
                Console.WriteLine(zData.zone);
            }

            return response.GetZonesResponse1.data;
        }

        public DynnetWSApi.ZoneData GetZone(string zoneName)
        {
            DynnetWSApi.GetOneZoneRequestType requestType = new DynnetWSApi.GetOneZoneRequestType();
            requestType.token = token;
            requestType.zone = zoneName;

            DynnetWSApi.GetOneZoneResponseType response = clientProxy.GetOneZone(requestType);

            Console.WriteLine(response.data.zone);

            DynnetWSApi.GetAllRecordsRequestType getAllRequest = new DynnetWSApi.GetAllRecordsRequestType();
            getAllRequest.token = token;
            getAllRequest.zone = zoneName;

            DynnetWSApi.GetAllRecordsResponseType getAllResponse = clientProxy.GetAllRecords(getAllRequest);
            DynnetWSApi.ANYRecordData data = getAllResponse.data;
            if (data.a_records != null)
            {
                foreach (DynnetWSApi.ARecordData aRecord in data.a_records)
                {
                    Console.Write("    A " + aRecord.fqdn + " " + aRecord.ttl + " " + aRecord.rdata.address + "\n");
                }
            }

            if (data.aaaa_records != null)
            {
                foreach (DynnetWSApi.AAAARecordData record in data.aaaa_records)
                {
                    Console.Write("    AAAA" + record.fqdn + " " + record.ttl + " " + record.rdata.address + "\n");
                }
            }

            if (data.cname_records != null)
            {
                foreach (DynnetWSApi.CNAMERecordData record in data.cname_records)
                {
                    Console.Write("     CNAME " + record.fqdn + " " + record.ttl + " " + record.rdata.cname + "\n");
                }
            }

            if (data.ns_records != null)
            {
                foreach (DynnetWSApi.NSRecordData record in data.ns_records)
                {
                    Console.Write("     NS" + record.fqdn + " " + record.ttl + " " + record.rdata.nsdname + "\n");
                }
            }

            return response.data;
        }

        public void AddZone(string zoneName)
        {
            DynnetWSApi.CreateZoneRequestType request = new DynnetWSApi.CreateZoneRequestType();
            request.token = token;
            request.ttl = 3600;

            request.zone = zoneName;
            request.rname = "hanchen@microsoft.com";

            RestDemo.DynnetWSApi.CreateZoneResponseType response = clientProxy.CreateZone(request);

            Console.WriteLine(response.status);
        }

        public DynnetWSApi.ZoneData[] GetZone()
        {
            DynnetWSApi.GetZonesRequestType requestType = new DynnetWSApi.GetZonesRequestType();
            requestType.token = token;

            DynnetWSApi.GetZonesResponseType response = clientProxy.GetZones(requestType);

            if (response.data != null)
            {
                foreach (DynnetWSApi.ZoneData zData in response.data)
                {
                    Console.WriteLine(zData.zone);
                }
            }

            GetJob(response.job_id);

            return response.data;
        }

        //public async Task<DynnetWSApi.ZoneData[]> GetZoneAsync()
        //{
        //    DynnetWSApi.GetZonesRequest request = new DynnetWSApi.GetZonesRequest();
        //    request.GetZonesRequest1.token = token;

        //    Task<DynnetWSApi.GetZonesResponse> getResponseTask = clientProxy.GetZonesAsync(request.GetZonesRequest1);

        //    DynnetWSApi.GetZonesResponse response = await getResponseTask;

        //    if (response.GetZonesResponse1.data != null)
        //    {
        //        foreach (DynnetWSApi.ZoneData zData in response.GetZonesResponse1.data)
        //        {
        //            Console.WriteLine(zData.zone);
        //        }
        //    }

        //    return response.GetZonesResponse1.data;
        //}

        public async Task<bool> AddZoneAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                DynnetWSApi.CreateZoneRequestType request = new DynnetWSApi.CreateZoneRequestType();
                request.token = token;
                request.ttl = 3600;

                request.zone = Guid.NewGuid().ToString().Replace('-', 'i') + ".modedgetest.com";
                request.rname = "hanchen@microsoft.com";

                Task<RestDemo.DynnetWSApi.CreateZoneResponse> getResponseTask = clientProxy.CreateZoneAsync(request);

                RestDemo.DynnetWSApi.CreateZoneResponse response = await getResponseTask;
                Console.WriteLine(response.CreateZoneResponse1.status);

            }

            return true;
        }

        public bool PublishZone(string zoneName)
        {
            DynnetWSApi.PublishZoneRequestType pRequest = new DynnetWSApi.PublishZoneRequestType();
            pRequest.token = token;
            pRequest.zone = zoneName;

            DynnetWSApi.PublishZoneResponseType pResponse = clientProxy.PublishZone(pRequest);

            return true;
        }

        public bool AddZone()
        {
            for (int i = 0; i < 100000; i++)
            {
                DynnetWSApi.CreateZoneRequestType request = new DynnetWSApi.CreateZoneRequestType();
                request.token = token;
                request.ttl = 3600;

                request.zone = Guid.NewGuid().ToString().Replace('-', 'i') + ".modedgetest.com";
                request.rname = "hanchen@microsoft.com";

                RestDemo.DynnetWSApi.CreateZoneResponseType response = clientProxy.CreateZone(request);

                this.PublishZone(request.zone);
                Console.WriteLine(response.status);

            }

            return true;
        }

        public async Task<bool> DeleteZonesAsync()
        {
            Task<DynnetWSApi.ZoneData[]> getZoneTask = this.GetZoneAsync();

            DynnetWSApi.ZoneData[] zones = await getZoneTask;

            DynnetWSApi.DeleteZoneChangesetRequestType request = new DynnetWSApi.DeleteZoneChangesetRequestType();
            request.token = token;

            foreach (DynnetWSApi.ZoneData zData in zones)
            {
                request.zone = zData.zone;
                Task<DynnetWSApi.DeleteZoneChangesetResponse> getResponseTask = clientProxy.DeleteZoneChangesetAsync(request);
                DynnetWSApi.DeleteZoneChangesetResponse response = await getResponseTask;
                Console.WriteLine(zData.zone + " : " + response.DeleteZoneChangesetResponse1.status);
            }

            return true;
        }

        public async Task<bool> DeleteZones()
        {
            Task<DynnetWSApi.ZoneData[]> getZones = this.GetZoneAsync();

            DynnetWSApi.ZoneData[] zones = await getZones;

            foreach (DynnetWSApi.ZoneData zData in zones)
            {
                DeleteZone(zData.zone);
                //DeleteZoneRaw(token, zData.zone);
            }

            return true;
        }

        public bool DeleteZone(string zoneName)
        {
            DynnetWSApi.DeleteOneZoneRequestType request = new DynnetWSApi.DeleteOneZoneRequestType();
            request.token = token;
            request.zone = zoneName;

            DynnetWSApi.DeleteOneZoneResponseType response = clientProxy.DeleteOneZone(request);
            Console.WriteLine(zoneName + " : " + response.status);
            return true;
        }

        public bool AddRecord(string type, string zoneName, string domainName, int ttl, string value)
        {
            switch (type.ToUpper())
            {
                case "A":
                    DynnetWSApi.CreateARecordRequestType Arequest = new DynnetWSApi.CreateARecordRequestType();
                    Arequest.token = token;
                    Arequest.zone = zoneName;
                    Arequest.fqdn = domainName;
                    Arequest.ttl = ttl;
                    Arequest.rdata = new DynnetWSApi.RDataA();
                    Arequest.rdata.address = value;

                    DynnetWSApi.CreateARecordResponseType Aresponse = clientProxy.CreateARecord(Arequest);

                    Console.WriteLine(Aresponse.status);
                    break;

                case "AAAA":
                    DynnetWSApi.CreateAAAARecordRequestType AAAArequest = new DynnetWSApi.CreateAAAARecordRequestType();
                    AAAArequest.token = token;
                    AAAArequest.zone = zoneName;
                    AAAArequest.fqdn = domainName;
                    AAAArequest.ttl = ttl;
                    AAAArequest.rdata = new DynnetWSApi.RDataAAAA();
                    AAAArequest.rdata.address = value;

                    DynnetWSApi.CreateAAAARecordResponseType AAAAresponse = clientProxy.CreateAAAARecord(AAAArequest);

                    Console.WriteLine(AAAAresponse.status);
                    break;

                case "CNAME":
                    DynnetWSApi.CreateCNAMERecordRequestType CNAMErequest = new DynnetWSApi.CreateCNAMERecordRequestType();
                    CNAMErequest.token = token;
                    CNAMErequest.zone = zoneName;
                    CNAMErequest.fqdn = domainName;
                    CNAMErequest.ttl = ttl;
                    CNAMErequest.rdata = new DynnetWSApi.RDataCNAME();
                    CNAMErequest.rdata.cname = value;

                    DynnetWSApi.CreateCNAMERecordResponseType CNAMEresponse = clientProxy.CreateCNAMERecord(CNAMErequest);

                    Console.WriteLine(CNAMEresponse.status);
                    break;

                case "NS":
                    DynnetWSApi.CreateNSRecordRequestType NSrequest = new DynnetWSApi.CreateNSRecordRequestType();
                    NSrequest.token = token;
                    NSrequest.zone = zoneName;
                    NSrequest.fqdn = domainName;
                    NSrequest.ttl = ttl;
                    NSrequest.rdata = new DynnetWSApi.RDataNS();
                    NSrequest.rdata.nsdname = value;

                    DynnetWSApi.CreateNSRecordResponseType NSresponse = clientProxy.CreateNSRecord(NSrequest);

                    Console.WriteLine(NSresponse.status);
                    break;
            }


            return true;
        }

        public bool DeleteZoneRaw(string token, string zoneName)
        {
            String getZoneUrl = string.Format("https://api2.dynect.net/REST/Zone/{0}/", zoneName);
            HttpWebRequest req = WebRequest.Create(getZoneUrl) as HttpWebRequest;

            req = WebRequest.Create(getZoneUrl) as HttpWebRequest;
            req.Method = "DELETE";
            req.ContentType = "application/json";
            req.Headers.Add("Auth-Token", token);

            using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(resp.GetResponseStream());
                string result = reader.ReadToEnd();
                Console.WriteLine(result);
            }

            return true;
        }

        public string Login()
        {
            DynnetWSApi.SessionLoginRequestType type = new DynnetWSApi.SessionLoginRequestType();
            type.customer_name = "modedge";
            type.user_name = "bgriffin";
            type.password = "modedge2013";

            DynnetWSApi.SessionLoginRequest request = new DynnetWSApi.SessionLoginRequest(type);
            DynnetWSApi.SessionLoginResponseType responseType = clientProxy.SessionLogin(type);

            Console.WriteLine(responseType.data.token);
            token = responseType.data.token;
            return responseType.data.token;
        }
    }
}
