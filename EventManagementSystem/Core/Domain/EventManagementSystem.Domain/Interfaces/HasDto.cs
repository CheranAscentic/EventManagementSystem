namespace EventManagementSystem.Domain.Interfaces
{
    /// <summary>
    /// Interface for domain models that can be converted to a DTO.
    /// </summary>
    public interface HasDto
    {
        /// <summary>
        /// Converts the domain model to its corresponding DTO.
        /// </summary>
        /// <returns>A DTO object representing the domain model.</returns>
        object ToDto();
    }
}
