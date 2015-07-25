using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApiTokenAuth.Utility
{
    public class BlobUploadUtility
    {
        static string azureStorageAccount;
        static string azureStorageKey;
        static string azureStorageURI;

        public static string CONTAINER_IMAGE = "images";
        public static string CONTAINER_CHATFILES = "chatfiles";


        static BlobUploadUtility()
        {
            azureStorageAccount = ConfigurationManager.AppSettings["storageAccount"];
            azureStorageKey = ConfigurationManager.AppSettings["storageKey"];
            azureStorageURI = ConfigurationManager.AppSettings["storageURI"];
        }

        /// <summary>
        /// to upload byte stream to blob 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        /// <returns> the absolute image path of the uploaded file</returns>
        public static string UploadFileStreamToBlob(byte[] data, string fileName, string containerName)
        {
            string absoluteFilePath = string.Empty;
            fileName = fileName.Replace(' ', '_').Replace(":", "_") + ".png";

            try
            {
                StorageCredentials credentials = new StorageCredentials(azureStorageAccount, azureStorageKey);
                CloudStorageAccount account = new CloudStorageAccount(credentials, false);

                CloudBlobClient blobClient = account.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                container.CreateIfNotExists();

                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

                var permissions = new BlobContainerPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);

                Stream stream = new MemoryStream(data);
                blob.UploadFromStream(stream);

                absoluteFilePath = azureStorageURI + "/" + containerName + "/" + fileName;
            }
            catch (Exception ex)
            {
                absoluteFilePath = string.Empty;
            }
            return absoluteFilePath;
        }

        public static void DeleteBlobFile(IEnumerable<string> fileNames, string containerName)
        {
            try
            {
                StorageCredentials credentials = new StorageCredentials(azureStorageAccount, azureStorageKey);
                CloudStorageAccount account = new CloudStorageAccount(credentials, false);

                CloudBlobClient blobClient = account.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                //container.CreateIfNotExists();
                foreach (var item in fileNames)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        CloudBlockBlob blob = container.GetBlockBlobReference(Path.GetFileName(item));
                        blob.DeleteIfExistsAsync();
                    }

                }
            }
            catch (Exception)
            {

            }

        }

        public static string Base64Encode(byte[] bytes)
        {
            if (bytes == null)
            {
                return "";
            }
            else
            {
                return System.Convert.ToBase64String(bytes);
            }
        }

        public static byte[] Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return null;
            }
            else
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return base64EncodedBytes;
            }
        }
    }
}