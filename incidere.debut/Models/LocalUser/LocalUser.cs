using System;
using System.Collections.Generic;

namespace incidere.debut.Models.LocalUser
{
    public class LocalUser : Entity
    {
        public LocalUser()
        {
            ReferenceNo = Guid.NewGuid().ToString();
        }

        public string ReferenceNo { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Location { get; set; }
        public List<ExternalUser> ExternalUsers { get; } = new List<ExternalUser>();
        public List<string> Roles { get; } = new List<string>();

        public override string ToString()
        {
            return "LocalUsers:" + ReferenceNo;
        }

        public override void DoWork()
        {
            ChangedBy = string.Empty;
            ChangedDate = DateTime.Now;
            CreatedBy = string.Empty;
            CreatedDate = DateTime.Now;
            Id = Guid.NewGuid().ToString();
            WebId = Guid.NewGuid().ToString();
        }
    }
}