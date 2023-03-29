namespace _1640WebApp.Services
{
    public interface IFileService
    {
        Tuple<int, string> SaveImage(IFormFile imageFile);

        public bool DeleteImage(String imageFileName);
    }
}