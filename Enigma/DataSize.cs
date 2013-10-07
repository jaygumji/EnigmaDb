namespace Enigma
{
    /// <summary>
    /// Used to set data sizes in a more user friendly manner
    /// </summary>
    public struct DataSize
    {

        /// <summary>
        /// Represents 1 KB
        /// </summary>
        public const long KB = 1024;

        /// <summary>
        /// Represents 1 MB
        /// </summary>
        public const long MB = KB * 1024;

        /// <summary>
        /// Represents 1 GB
        /// </summary>
        public const long GB = MB * 1024;

        /// <summary>
        /// Represents 1 TB
        /// </summary>
        public const long TB = GB * 1024;

        /// <summary>
        /// The value of the data size in bytes
        /// </summary>
        public readonly long Value;

        /// <summary>
        /// Represents an empty data size
        /// </summary>
        public static readonly DataSize Empty = new DataSize(-long.MaxValue);

        /// <summary>
        /// Whether this instance is empty
        /// </summary>
        public bool IsEmpty { get { return Value == -long.MaxValue; } }

        /// <summary>
        /// Creates a new instance of <see cref="DataSize"/>
        /// </summary>
        /// <param name="value">The value of the data size in bytes</param>
        public DataSize(long value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DataSize)
                return Value.Equals(((DataSize)obj).Value);

            if (obj is long)
                return Value.Equals((long)obj);

            return false;
        }

        public override string ToString()
        {
            var value = (double) Value;

            if (Value >= TB)
                return (value / TB).ToString("### ### ### ### ##0.00").TrimStart() + " TB";
            else if (Value >= GB)
                return (value / GB).ToString("# ##0.00").TrimStart() + " GB";
            else if (Value >= MB)
                return (value / MB).ToString("# ##0.00").TrimStart() + " MB";
            else if (Value >= KB)
                return (value / KB).ToString("# ##0.00").TrimStart() + " KB";

            return Value.ToString("# ##0").TrimStart() + " B";
        }

        /// <summary>
        /// Creates a datasize from a double value as KB
        /// </summary>
        /// <param name="value">The value in KB</param>
        /// <returns>A datasize representing the value</returns>
        public static DataSize FromKB(double value)
        {
            return new DataSize((long)(value * KB));
        }

        /// <summary>
        /// Creates a datasize from a double value as MB
        /// </summary>
        /// <param name="value">The value in MB</param>
        /// <returns>A datasize representing the value</returns>
        public static DataSize FromMB(double value)
        {
            return new DataSize((long) (value * MB));
        }

        /// <summary>
        /// Creates a datasize from a double value as GB
        /// </summary>
        /// <param name="value">The value in GB</param>
        /// <returns>A datasize representing the value</returns>
        public static DataSize FromGB(double value)
        {
            return new DataSize((long)(value * GB));
        }

        /// <summary>
        /// Creates a datasize from a double value as TB
        /// </summary>
        /// <param name="value">The value in TB</param>
        /// <returns>A datasize representing the value</returns>
        public static DataSize FromTB(double value)
        {
            return new DataSize((long)(value * TB));
        }

        /// <summary>
        /// Creates a datasize from a byte count value
        /// </summary>
        /// <param name="value">The value as byte count</param>
        /// <returns>A datasize representing the value</returns>
        public static DataSize FromBytes(long value)
        {
            return new DataSize(value);
        }

        /// <summary>
        /// Creates a datasize from the length of a binary array
        /// </summary>
        /// <param name="value">The binary array</param>
        /// <returns>A datasize representing the length of the binary array</returns>
        public static DataSize FromBytes(byte[] value)
        {
            return new DataSize(value.LongLength);
        }

    }
}
