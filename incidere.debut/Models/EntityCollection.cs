namespace incidere.debut.Models
{
    public abstract class EntityCollection
    {
        public EntityCollection()
        {
            DoWork();
        }

        public string WebId { get; set; }

        public abstract void DoWork();
    }
}