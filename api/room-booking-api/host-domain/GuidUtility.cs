using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace host_domain
{
    public static class GuidUtility
    {
        public static Guid Create(string name) => Create(DnsNamespace, name, 5);

        public static Guid Create(Guid namespaceId, string name, int version)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            return Create(namespaceId, Encoding.UTF8.GetBytes(name), version);
        }

        [SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "Per spec.")]
        [SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Per spec.")]
        public static Guid Create(Guid namespaceId, byte[] nameBytes, int version)
        {
            if (version != 3 && version != 5)
                throw new ArgumentOutOfRangeException(nameof(version), "version must be either 3 or 5.");

            // convert the namespace UUID to network order (step 3)
            var namespaceBytes = namespaceId.ToByteArray();
            SwapByteOrder(namespaceBytes);

            // compute the hash of the namespace ID concatenated with the name (step 4)
            var data = namespaceBytes.Concat(nameBytes).ToArray();
            byte[] hash;
            using (var algorithm = version == 3 ? (HashAlgorithm)MD5.Create() : SHA1.Create())
                hash = algorithm.ComputeHash(data);

            // most bytes from the hash are copied straight to the bytes of the new GUID (steps 5-7, 9, 11-12)
            var newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            // set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
            newGuid[6] = (byte)((newGuid[6] & 0x0F) | (version << 4));

            // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
            newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

            // convert the resulting UUID to local byte order (step 13)
            SwapByteOrder(newGuid);
            return new Guid(newGuid);
        }

        public static readonly Guid DnsNamespace = new Guid("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

        internal static void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        private static void SwapBytes(byte[] guid, int left, int right)
        {
            var temp = guid[left];
            guid[left] = guid[right];
            guid[right] = temp;
        }
    }
}