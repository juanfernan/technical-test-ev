namespace TestEv.Domain.Entities
{
    public class Project
    {
        public string Id { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string OwnerId { get; private set; } = string.Empty;
        public ProjectStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Project() { }

        public static Project Create(string name, string description, string ownerId, ProjectStatus status = ProjectStatus.Active)
        {
            var now = DateTime.UtcNow;
            return new Project
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                OwnerId = ownerId,
                Status = status,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public void Update(string name, string description, string ownerId, ProjectStatus status)
        {
            Name = name;
            Description = description;
            OwnerId = ownerId;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public static Project Hydrate(
            string id,
            string name,
            string description,
            string ownerId,
            ProjectStatus status,
            DateTime createdAt,
            DateTime updatedAt)
        {
            return new Project
            {
                Id = id,
                Name = name,
                Description = description,
                OwnerId = ownerId,
                Status = status,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };
        }
    }
}
