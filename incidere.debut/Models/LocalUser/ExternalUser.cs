using System;
using System.Collections.Generic;

namespace incidere.debut.Models.LocalUser
{
    public class ExternalUser : EntityCollection
    {
        public ExternalUser()
        {
        }

        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public List<string> Roles { get; } = new List<string>();

        public override void DoWork()
        {
            WebId = Guid.NewGuid().ToString();
        }
    }
}