using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApcConverter
{
    internal sealed class ApcDocument
    {
        private string _certificate;
        private string _caCertificate;
        private string _privateKey;
        private readonly HashSet<string> _serverAddresses = new();

        [JsonPropertyName("key")]
        public string PrivateKey
        {
            get => _privateKey;
            set
            {
                _privateKey = FormatCertificate(value);
            }
        }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("server_port")]
        public int ServerPort { get; set; }

        [JsonPropertyName("certificate")]
        public string Certificate
        {
            get => _certificate;
            set
            {
                _certificate = FormatCertificate(value);
            }
        }

        [JsonPropertyName("ca_cert")]
        public string CaCertificate
        {
            get => _caCertificate;
            set
            {
                _caCertificate = FormatCertificate(value);
            }
        }


        [JsonPropertyName("server_dn")]
        public string ServerDN { get; set; } = "";

        [JsonPropertyName("server_address")]
        public IEnumerable<string> ServerAddress { get => _serverAddresses; }

        [JsonPropertyName("authentication_algorithm")]
        public string AuthenticationAlgorithm { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("encryption_algorithm")]
        public string EncryptionAlgorithm { get; set; }

        [JsonPropertyName("compression")]
        [JsonConverter(typeof(BoolJsonConverter))]
        public bool Compression { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        public void ClearServerAddresses()
        {
            _serverAddresses.Clear();
        }

        public void AddServerAddress(string address)
        {
            _serverAddresses.Add(address);
        }

        private static string FormatCertificate(string source)
        {
            source = source.Trim();

            var startPattern = "-----BEGIN ";

            if (!source.StartsWith(startPattern))
            {
                throw new ArgumentException("Certificate does not have a start tag", nameof(source));
            }

            var nameEnd = source.IndexOf("-----", 11);
            if (nameEnd == -1)
            {
                throw new ArgumentException("Certificate does not have a valid start tag", nameof(source));
            }

            var name = source[startPattern.Length..nameEnd];

            startPattern = $"-----BEGIN {name}-----";
            var endPattern = $"-----END {name}-----";

            if (!source.EndsWith(endPattern))
            {
                throw new ArgumentException("Certificate does not have a valid end tag", nameof(source));
            }

            source = source.Replace("\n", "").Replace("\r", "");

            source = source.Replace(startPattern, startPattern + "\n");
            source = source.Replace(endPattern, "\n" + endPattern);

            return source;
        }
    }

    internal sealed class BoolJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (int.TryParse(reader.GetString()!, out int result))
            {
                return result != 0;
            }
            return false;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value ? "1" : "0");
        }
    }
}