using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.ServiceModel;


namespace RestDemo
{
    [Cmdlet(VerbsCommon.Get, "Zone", SupportsShouldProcess = true)]
    public class GetZone : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the zone.
        /// </summary>
        /// <value>
        /// The name of the zone.
        /// </value>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Zone name to be retrived")]
        public string ZoneName { set; get; }


        /// <summary>
        /// Gets or sets the include deleted flag.
        /// </summary>
        /// <value>
        /// The include deleted flag.
        /// </value>
        [Parameter(Position = 1, Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Sets/Gets boolean for returning deleted records")]
        public bool? IncludeDeleted { set; get; }

        protected override void ProcessRecord()
        {
            DynApiWrapper wrapper = new DynApiWrapper();
            wrapper.Login();

            if (ZoneName.ToLower().CompareTo("all") == 0)
            {
                Task<DynnetWSApi.ZoneData[]> getdata =  wrapper.GetZoneAsync();
            }
            else
            {
                wrapper.GetZone(ZoneName);
            }
        }
    }
}
