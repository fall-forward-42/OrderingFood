namespace OrderingFood.Interfaces
{
    public interface IBufferedFileUploadService
    {
        //interface to upload and save img file
        Task<bool> UploadImageFile(IFormFile file, string direct,string nameOfFile);
        string GenerateSlug(string text);
    }
}
