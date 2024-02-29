namespace DMG.Examples.Testing.Domain.Models
{
    public record User : ModelBase
    {
        public required string Name { get; init; }
        public required int Age { get; init; }
    }
}
