using OrderingFood.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace OrderingFood.Services
{
    public class BufferedFileUploadLocalService : IBufferedFileUploadService
    {
        public  string GenerateSlug(string text)
        {
            // Remove diacritics
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            string regex = new string(normalizedString
                .Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());

            // Replace spaces with underscores
            string underscoredString = Regex.Replace(regex, @"\s", "_");

            return underscoredString;
        }
        public async Task<bool> UploadImageFile(IFormFile file, string direct, string nameOfFile)
        {
            string path = "";
            try
            {
               
                 
                if (file.Length > 0)
                {
                    //create the path in direct to file be located
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot/"+direct));

                    //create new direct if no exist
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //transder content file into direct
                    using (var fileStream = new FileStream(Path.Combine(path, nameOfFile), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }

    
}
