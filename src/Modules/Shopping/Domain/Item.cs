namespace Shopping.Domain;

public class Item
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Url { get; private set; }

    public Item(string name, string description, string? url)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome não pode ser vazio", nameof(name));

        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? null;
        Url = url ?? null;
    }
}
