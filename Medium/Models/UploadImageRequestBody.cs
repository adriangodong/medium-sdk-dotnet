namespace Medium.Models
{
    public class UploadImageRequestBody
    {
        public string ContentType { get; set; }
        public byte[] ContentBytes { get; set; }
    }
}