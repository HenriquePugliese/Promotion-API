namespace Acropolis.Application.Base.Models
{
    public abstract class Entity
    {
        protected Entity(Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
        }

        public Guid Id { get; private set; }
    }
}