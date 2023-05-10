using System.Text.Encodings.Web;
using System.Text.Json;

namespace ApcConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine($"Usage: {System.Diagnostics.Process.GetCurrentProcess().ProcessName} input output");
                return;
            }
            var reader = new ApcReader();
            reader.Load(args[0]);

            var apcDocument = new ApcDocument();
            try
            {
                apcDocument.AddServerAddress(reader.GetServerAddress());
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Server Address: {e.Message}");
                return;
            }

            try
            {
                apcDocument.ServerPort = reader.GetServerPort();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Server Port: {e.Message}");
                return;
            }
            
            try
            {
                apcDocument.Protocol = reader.GetProtocol();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Protocol: {e.Message}");
                return;
            }

            try
            {
                apcDocument.AuthenticationAlgorithm = reader.GetAuthentication();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Authentication Algorithm: {e.Message}");
                return;
            }

            try
            {
                apcDocument.EncryptionAlgorithm = reader.GetEncryption();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Encryption Algorithm: {e.Message}");
                return;
            }

            try
            {
                apcDocument.Certificate = reader.GetCertificate();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Certificate: {e.Message}");
                return;
            }

            try
            {
            apcDocument.CaCertificate = reader.GetCaCertificate();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading CA Certificate: {e.Message}");
                return;
            }

            try
            {
            apcDocument.PrivateKey = reader.GetPrivateKey();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Private Key: {e.Message}");
                return;
            }

            try
            {
            apcDocument.Username = reader.GetUserName();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Username: {e.Message}");
                return;
            }

            try
            {
            apcDocument.Password = reader.GetPassword();
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Error reading Password: {e.Message}");
                return;
            }

            apcDocument.Compression = reader.GetCompression();

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonString = JsonSerializer.Serialize(apcDocument, options);

            File.WriteAllText(args[1], jsonString, System.Text.Encoding.ASCII);
        }
    }
}