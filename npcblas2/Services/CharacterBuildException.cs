using System;
using System.Runtime.Serialization;

namespace npcblas2.Services
{
    /// <summary>
    /// This exception type won't cause error entries in the log when thrown during
    /// character build.
    /// </summary>
    [Serializable]
    public class CharacterBuildException : Exception
    {
        public CharacterBuildException() : base()
        {
        }

        public CharacterBuildException(string message) : base(message)
        {
        }

        public CharacterBuildException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CharacterBuildException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}