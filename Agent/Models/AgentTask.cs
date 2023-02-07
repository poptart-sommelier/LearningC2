using System.Runtime.Serialization;

namespace Agent.Models
{
    [DataContract]
    public class AgentTask
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "command")]
        public string Command { get; set; }

        [DataMember(Name = "arguments")]
        public string[] Arguments { get; set; }

        [DataMember(Name = "file")]
        public byte[] File { get; set; }

    }
}
