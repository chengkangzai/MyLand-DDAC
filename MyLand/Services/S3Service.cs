using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MyLand.Services
{
    public class S3Service
    {
        /**
         * @throws AmazonS3Exception
         */
        public async static Task<PutObjectResponse> UploadImages(string fileName, Stream stream)
        {
            var s3Client = AWSHelper.getAmazonS3Client();
            var request = new PutObjectRequest
            {
                InputStream = stream,//source file
                BucketName = AWSHelper.getBucketName() + "/images",// bucket name and path for folder
                Key = fileName,
                CannedACL = S3CannedACL.PublicRead// public can read it
            };
            return await s3Client.PutObjectAsync(request);
        }
    }
}
