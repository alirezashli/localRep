using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Helper.Models
{
    [DataContract]
    public class CapthcaKeySpec
    {
        [DataMember(Name = "privateKey")]
        public string PrivateKey { get; set; }
        [DataMember(Name = "publicKey")]
        public string PublicKey { get; set; }
    }
}
