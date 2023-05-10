using System.Text;

namespace ApcConverter
{
    internal class ApcReader
    {
        private byte[] _data;

        public void Load(string path)
        {
            _data = File.ReadAllBytes(path);
        }

        public string GetUserName()
        {
            var pattern = new byte[] { 0x08, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("username")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                throw new ParseException("Could not find Username");
            }

            return FindString(index);
        }

        public string GetPassword()
        {
            var pattern = new byte[] { 0x08, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("password")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                throw new ParseException("Could not find Password");
            }

            return FindString(index);
        }

        public string GetServerAddress()
        {
            var pattern = new byte[] { 0x0e, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("server_address")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                throw new ParseException("Could not find Server Address");
            }

            return FindString(index);
        }

        public int GetServerPort()
        {
            var pattern = new byte[] { 0x0b, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("server_port")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                throw new ParseException("Could not find Server Port");
            }

            return BitConverter.ToUInt16(_data, index - 4);
        }

        public string GetProtocol()
        {
            var pattern = new byte[] { 0x08, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("protocol")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                throw new ParseException("Could not find Protocol");
            }

            return FindString(index);
        }

        public string GetEncryption()
        {
            var pattern = new byte[] { 0x14, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("encryption_algorithm")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                throw new ParseException("Could not find Encryption Algorithm");
            }

            return FindString(index);
        }

        public string GetAuthentication()
        {
            var pattern = new byte[] { 0x18, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("authentication_algorithm")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                throw new ParseException("Could not find Authentication Algorithm");
            }

            return FindString(index);
        }

        public bool GetCompression()
        {
            var pattern = new byte[] { 0x0b, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes("compression")).ToArray();
            var index = _data.Find(pattern);

            if (index == -1)
            {
                return false;
            }

            return _data[index - 1] == 0x81;
        }

        public string GetCertificate()
        {
            var certificate = GetCertificate(0x0b, "certificate");

            if (certificate == null)
            {
                throw new ParseException("Could not find Certificate");
            }
            return ExtractCertificate(certificate);
        }

        public string GetCaCertificate()
        {
            var certificate = GetCertificate(0x07, "ca_cert");

            if (certificate == null)
            {
                throw new ParseException("Could not find CA Certificate");
            }
            return ExtractCertificate(certificate);
        }

        public string GetPrivateKey()
        {
            var certificate = GetCertificate(0x03, "key");

            if (certificate == null)
            {
                throw new ParseException("Could not find Private Key");
            }
            return ExtractCertificate(certificate);
        }

        private string GetCertificate(byte marker, string name)
        {
            var pattern = new byte[] { marker, 0x00, 0x00, 0x00 }.Concat(Encoding.ASCII.GetBytes(name)).ToArray();
            var index = _data.Find(pattern);
            if (index == -1)
            {
                throw new ParseException("");
            }

            var start = _data.FindBack(0x01, index - 1);

            if (start == -1)
            {
                throw new ParseException("Could not find Certificate");
            }

            var length = BitConverter.ToUInt16(_data, start + 1);

            if (start + 5 + length != index)
            {
                throw new ParseException("Certificate length does not match expected length");
            }

            return Encoding.ASCII.GetString(_data, start + 5, length);
        }

        private static string ExtractCertificate(string certificate)
        {
            var startPattern = "-----BEGIN ";
            
            var start = certificate.IndexOf(startPattern);
            if (start == -1)
            {
                throw new ParseException("Certificate does not have a start tag");
            }

            var nameEnd = certificate.IndexOf("-----", start + startPattern.Length);
            if (nameEnd == -1)
            {
                throw new ParseException("Certificate does not have a valid start tag");
            }

            var name = certificate.Substring(start + startPattern.Length, nameEnd - start - startPattern.Length);

            var stopPattern = $"-----END {name}-----";

            var end = certificate.IndexOf(stopPattern);

            if (end == -1)
            {
                throw new ParseException("Certificate does not have a valid end tag");
            }

            return certificate[start..(end + stopPattern.Length)];
        }

        private string FindString(int index)
        {
            var start = _data.FindBack(0x0a, index - 1);
            if (start == -1)
            {
                throw new ParseException("Can not find beginning of string");
            }

            var length = _data[start + 1];
            if (start + 2 + length == index)
            {
                return Encoding.ASCII.GetString(_data, start + 2, length);
            }

            throw new ParseException("String length does not match expected length");
        }
    }
}