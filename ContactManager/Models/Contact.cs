using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Models
{
    public class Contact
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string ProfileImage { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] ProfileImageBytes { get; set; }

        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string WorkPhone { get; set; }
        public string HomePhone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Notes { get; set; }

    }

    public class UploadCustomerImageModel
    {
        public string Description { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] ImageData { get; set; }
    }

}