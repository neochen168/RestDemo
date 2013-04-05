using System.Management.Automation;

namespace RestDemo
{

    [Cmdlet(VerbsCommon.Add, "Record", SupportsShouldProcess = true)]
    public class AddRecord : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the zone.
        /// </summary>
        /// <value>
        /// The name of the zone.
        /// </value>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Zone name to be retrived")]
        public string ZoneName { set; get; }

        [Parameter(Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Zone name to be retrived")]
        public string DomainName { set; get; }

        [Parameter(Position = 2, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Zone name to be retrived")]
        public string RecordType { set; get; }

        [Parameter(Position = 3, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Zone name to be retrived")]
        public int TTL { set; get; }

        [Parameter(Position = 4, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Zone name to be retrived")]
        public string Value { set; get; }

        protected override void ProcessRecord()
        {
            DynApiWrapper wrapper = new DynApiWrapper();
            wrapper.Login();

            wrapper.AddRecord(RecordType, ZoneName, DomainName, TTL, Value);
            wrapper.PublishZone(ZoneName);
        }
    }
}
