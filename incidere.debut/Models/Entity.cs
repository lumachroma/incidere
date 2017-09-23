using System;

namespace incidere.debut.Models
{
    public abstract class Entity
    {
        public Entity()
        {
            DoWork();
        }

        public string ChangedBy { get; set; }
        public DateTime ChangedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Id { get; set; }
        public string WebId { get; set; }
        public string FirebaseKey { get; set; }

        public abstract void DoWork();
    }
}