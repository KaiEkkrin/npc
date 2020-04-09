namespace npcblas2.Data
{
    /// <summary>
    /// Defines the extra things we add to our users.
    /// </summary>
    public interface IApplicationUser
    {
        /// <summary>
        /// This handle should be unique per user and is shown publically.
        /// </summary>
        string Handle { get; set; }

        /// <summary>
        /// If true, this user has the Admin permission.
        /// </summary>
        bool? IsAdmin { get; set; }
    }
}