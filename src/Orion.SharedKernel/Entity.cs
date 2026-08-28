namespace Orion.SharedKernel;

/// <summary>
/// Base class for DDD entities. Implementing <see cref="IEquatable{T}"/> lets the compiler use
/// <see cref="Equals(Entity{TId}?)"/> (the strongly-typed overload below) instead of falling back
/// to the default reference equality whenever two <c>Entity&lt;TId&gt;</c> instances are compared.
/// </summary>
/// <typeparam name="TId">Type of the identity value (e.g. <see cref="Guid"/>).</typeparam>
#pragma warning disable S4035
public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
{
    /// <c>protected set</c>: only this class and derived classes can assign the Id (e.g. in the constructor below); code outside the class hierarchy can only read it.
    public TId Id { get; protected set; } = default!;

    protected Entity()
    {
    }

    protected Entity(TId id) => Id = id;

    /// <summary>
    /// Typed equality from <see cref="IEquatable{T}"/>: two entities are equal when they are the
    /// same runtime type and share the same <see cref="Id"/>, regardless of their other property
    /// values or whether they are the same object in memory.
    /// </summary>
    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (GetType() != other.GetType()) return false;
        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Untyped override from <see cref="object"/>. `obj as Entity&lt;TId&gt;` returns null (instead of
    /// throwing) when <paramref name="obj"/> isn't an <c>Entity&lt;TId&gt;</c>, so this just delegates
    /// to the typed <see cref="Equals(Entity{TId}?)"/> above.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as Entity<TId>);

    /// <summary>
    /// Must stay consistent with <see cref="Equals(Entity{TId}?)"/>: equal entities are required to
    /// return the same hash code, otherwise lookups in <see cref="HashSet{T}"/>/<see cref="Dictionary{TKey,TValue}"/> break.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    /// <summary>Operator overload so <c>entityA == entityB</c> uses the same identity comparison as <see cref="Equals(Entity{TId}?)"/> instead of C#'s default reference comparison.</summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => left is null ? right is null : left.Equals(right);

    /// <summary>Complement of <see cref="operator =="/>; C# requires both operators to be defined together.</summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}
#pragma warning restore S4035