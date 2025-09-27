using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractFlow.Domain.Models
{
    public abstract class Entity<TId>: IEquatable<Entity<TId>> where TId: notnull
    {
        public TId Id { get; protected set;}

        protected Entity(TId id)
        {
            Id = id;
        }

        public override bool Equals(object? obj) => obj is Entity<TId> entity && Id.Equals(entity.Id);

       

        public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);

        public static bool operator !=(Entity<TId> left, Entity<TId>? right) => !Equals(left, right);

        public bool Equals(Entity<TId>? other) => Equals((object?)other);
        
        public override int GetHashCode() => Id.GetHashCode();
        
    }
}
