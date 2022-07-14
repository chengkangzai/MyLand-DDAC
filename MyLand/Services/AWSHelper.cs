using Amazon.S3;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace MyLand.Services
{
    public class AWSHelper
    {
        public static string getBucketName()
        {
            return "myland-images";
        }

        public static string getBucketUrl()
        {
            return "https://myland-images.s3.us-east-1.amazonaws.com/";
        }

        public static AmazonS3Client getAmazonS3Client()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration configuration = builder.Build();

            var accessKey = configuration["AWSCredential:key1"];
            var secretKey = configuration["AWSCredential:key2"];
            var tokenKey = configuration["AWSCredential:key3"];


            return new AmazonS3Client(accessKey, secretKey, tokenKey, Amazon.RegionEndpoint.USEast1);
        }
    }
}
