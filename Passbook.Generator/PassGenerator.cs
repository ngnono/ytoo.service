using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509.Store;

namespace Passbook.Generator
{
    public class PassGenerator
    {
        // Methods
        private static void CopyImageFiles(PassGeneratorRequest request, string tempPath)
        {
            if (request.Images.Count == 0)
            {
                return;
            }

            string destFileName = Path.Combine(tempPath, Path.GetFileName(request.IconFile));
            string str2 = Path.Combine(tempPath, Path.GetFileName(request.IconRetinaFile));
            File.Copy(request.IconFile, destFileName);
            File.Copy(request.IconRetinaFile, str2);
            string str3 = Path.Combine(tempPath, Path.GetFileName(request.LogoFile));
            string str4 = Path.Combine(tempPath, Path.GetFileName(request.LogoRetinaFile));
            File.Copy(request.LogoFile, str3);
            File.Copy(request.LogoRetinaFile, str4);
            string str5 = Path.Combine(tempPath, Path.GetFileName(request.BackgroundFile));
            string str6 = Path.Combine(tempPath, Path.GetFileName(request.BackgroundRetinaFile));
            File.Copy(request.BackgroundFile, str5);
            File.Copy(request.BackgroundRetinaFile, str6);
        }

        private string CreatePackage(PassGeneratorRequest request)
        {
            string path = Path.Combine(Path.GetTempPath() + Path.GetRandomFileName() + "/", "contents");
            Directory.CreateDirectory(path);
            CopyImageFiles(request, path);
            this.CreatePassFile(request, path);
            this.GenerateManifestFile(request, path);
            return path;
        }

        private void CreatePassFile(PassGeneratorRequest request, string tempPath)
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(tempPath, "pass.json")))
            {
                using (JsonWriter writer2 = new JsonTextWriter(writer))
                {
                    request.Write(writer2);
                }
            }
        }

        public Pass Generate(PassGeneratorRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            string pathToPackage = this.CreatePackage(request);
            return new Pass(this.ZipPackage(pathToPackage));
        }

        private void GenerateManifestFile(PassGeneratorRequest request, string tempPath)
        {
            string path = Path.Combine(tempPath, "manifest.json");
            string[] files = Directory.GetFiles(tempPath);
            using (StreamWriter writer = new StreamWriter(File.Open(path, FileMode.Create)))
            {
                using (JsonWriter writer2 = new JsonTextWriter(writer))
                {
                    writer2.Formatting = Formatting.Indented;
                    writer2.WriteStartObject();
                    foreach (string str2 in files)
                    {
                        string fileName = Path.GetFileName(str2);
                        string hashForFile = this.GetHashForFile(str2);
                        writer2.WritePropertyName(fileName);
                        writer2.WriteValue(hashForFile.ToLower());
                    }
                }
            }
            this.SignManigestFile(request, path);

        }

        private X509Certificate2 GetAppleCertificate()
        {
            return GetSpecifiedCertificate("‎‎0950b6cd3d2f37ea246a1aaa20dfaadbd6fe1f75", StoreName.CertificateAuthority, StoreLocation.LocalMachine);
        }

        public static X509Certificate2 GetCertificate(PassGeneratorRequest request)
        {
            return GetSpecifiedCertificate(request.CertThumbprint, StoreName.My, request.CertLocation);
        }

        private string GetHashForFile(string fileAndPath)
        {
            byte[] buffer;
            SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();
            using (FileStream stream = File.Open(fileAndPath, FileMode.Open))
            {
                buffer = provider.ComputeHash(stream);
            }
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        private static X509Certificate2 GetSpecifiedCertificate(string thumbPrint, StoreName storeName, StoreLocation storeLocation)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificates = store.Certificates;
            if (certificates.Count > 0)
            {
                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 certificate = certificates[i];
                    if (string.Compare(certificate.Thumbprint, thumbPrint, true) == 0)
                    {
                        return certificates[i];
                    }
                }
            }
            return null;
        }

        private void SignManigestFile(PassGeneratorRequest request, string manifestFileAndPath)
        {
            byte[] bytes = File.ReadAllBytes(manifestFileAndPath);
            X509Certificate2 certificate = GetCertificate(request);
            Org.BouncyCastle.X509.X509Certificate certificate2 = DotNetUtilities.FromX509Certificate(certificate);
            AsymmetricKeyParameter @private = DotNetUtilities.GetKeyPair(certificate.PrivateKey).Private;
            Org.BouncyCastle.X509.X509Certificate certificate4 = DotNetUtilities.FromX509Certificate(this.GetAppleCertificate());
            ArrayList collection = new ArrayList();
            collection.Add(certificate4);
            collection.Add(certificate2);
            X509CollectionStoreParameters parameters = new X509CollectionStoreParameters(collection);
            IX509Store certStore = X509StoreFactory.Create("CERTIFICATE/COLLECTION", parameters);
            CmsSignedDataGenerator generator = new CmsSignedDataGenerator();
            generator.AddSigner(@private, certificate2, CmsSignedGenerator.DigestSha1);
            generator.AddCertificates(certStore);
            CmsProcessable content = new CmsProcessableByteArray(bytes);
            File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(manifestFileAndPath), "signature"), generator.Generate(content, false).GetEncoded());
        }

        private string ZipPackage(string pathToPackage)
        {
            string zipFileName = pathToPackage + @"\..\pass.pkpass";
            new FastZip().CreateZip(zipFileName, pathToPackage, false, null);
            return zipFileName;
        }
    }
}