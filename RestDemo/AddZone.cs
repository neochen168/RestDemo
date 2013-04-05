using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace RestDemo
{
    [Cmdlet(VerbsCommon.Add, "Zone", SupportsShouldProcess = true)]
    public class AddZone : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the zone.
        /// </summary>
        /// <value>
        /// The name of the zone.
        /// </value>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Zone name to be retrived")]
        public string ZoneName { set; get; }


        protected override void ProcessRecord()
        {
            DynApiWrapper wrapper = new DynApiWrapper();
            wrapper.Login();

            if (ZoneName != string.Empty)
            {
                wrapper.AddZone(ZoneName);
                wrapper.PublishZone(ZoneName);
            }
        }
    }
}
