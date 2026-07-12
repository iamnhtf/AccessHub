using Amazon.S3;
using Amazon.S3.Model;

namespace AccessHub.API.Services;

public class S3Service
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _configuration;

    public S3Service(IAmazonS3 s3, IConfiguration configuration)
    {
        _s3 = s3;
        _configuration = configuration;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var bucketName = _configuration["AWS:BucketName"];

        var key = $"attachments/{Guid.NewGuid()}_{file.FileName}";

        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
        };

        await _s3.PutObjectAsync(request);

        return key;
    }

    public string GeneratePresignedUrl(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _configuration["AWS:BucketName"],
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(30),
        };

        return _s3.GetPreSignedURL(request);
    }
}
