using System;

namespace incidere.debut.Models.Internals.Settings
{
    public class Setting : Entity
    {
        public Setting()
        {
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return "Setting:" + Key;
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