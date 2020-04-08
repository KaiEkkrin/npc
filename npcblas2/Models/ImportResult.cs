namespace npcblas2.Models
{
    /// <summary>
    /// Describes the result of importing characters.
    /// </summary>
    public class ImportResult
    {
        public int NumberAdded { get; set; }

        public int NumberUpdated { get; set; }

        public int NumberRejected { get; set; }
    }
}