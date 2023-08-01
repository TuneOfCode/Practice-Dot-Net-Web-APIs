using System.Net;
using LearnIndentityAndAuthorization.Configs;

namespace LearnIndentityAndAuthorization.Helpers;

public static class Helper
{
    /// <summary>
    /// Up ảnh lên server
    /// </summary>
    /// <param name="files"></param>
    /// <param name="maxCount"></param>
    /// <param name="allowedExtensions"></param>
    /// <param name="maxSize"></param>
    /// <returns></returns>
    public static List<string> UploadFiles
    (
        List<IFormFile> files,
        int maxCount = Constants.MAX_COUNT_OF_FILE_DEFAULT,
        string allowedExtensions = Constants.ALLOWED_EXTENSIONS_DEFAULT,
        long maxSize = Constants.MAX_SIZE_OF_FILE_DEFAULT
    )
    {
        List<string> fileUrls = new List<string>();

        // kiểm tra file có tồn tại hay không
        if (files is null || files.Count == 0)
        {
            throw new Exception("File không tồn tại");
        }

        // kiểm tra số lượng file có vượt quá giới hạn cho phép hay không
        if (files.Count > maxCount)
        {
            throw new Exception($"Số lượng file vượt quá {maxCount} file");
        }

        // kiểm tra dung lượng file có vượt quá giới hạn cho phép hay không
        var totalSize = files.Sum(f => f.Length);
        var maxSizeInMb = maxSize * 1024 * 1024;
        if (totalSize > maxSizeInMb)
        {
            throw new Exception($"Dung lượng file vượt quá {maxSize}MB");
        }

        // tạo thư mục lưu file
        string folderName = null!; // Path.Combine("Resources", "Images")

        // lấy đường dẫn truy cập file
        // localhost:{port}/resources/images/fileName
        string fileUrl = null!; // /resources/images/fileName

        // upload ảnh với từng file
        foreach (var file in files)
        {
            // kiểm tra định dạng file có hợp lệ hay không
            var extensionFile = Path.GetExtension(file.FileName);
            if (!allowedExtensions.Contains(extensionFile.ToLower()))
            {
                throw new Exception($"Định dạng file không hợp lệ. Định dạng file hợp lệ: {allowedExtensions}");
            }

            // tạo mới tên file để tránh trùng lặp
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            // tạo thư mục lưu file theo định dạng file
            if (Constants.ALLOWED_EXTENSIONS_IMAGE.Contains(extensionFile.ToLower()))
            {
                folderName = Path.Combine("Resources", "Images");
                fileUrl = $"/resources/images/{fileName}";
            }
            else if (Constants.ALLOWED_EXTENSIONS_DOCUMENT.Contains(extensionFile.ToLower()))
            {
                folderName = Path.Combine("Resources", "Documents");
                fileUrl = $"/resources/documents/{fileName}";
            }
            else if (Constants.ALLOWED_EXTENSIONS_VIDEO.Contains(extensionFile.ToLower()))
            {
                folderName = Path.Combine("Resources", "Videos");
                fileUrl = $"/resources/videos/{fileName}";
            }
            else
            {
                folderName = Path.Combine("Resources", "Others");
                fileUrl = $"/resources/others/{fileName}";
            }


            // tạo đường dẫn lưu file
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            // kiểm tra thư mục lưu file đã tồn tại hay chưa
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            // tạo đường dẫn truy cập và lưu file
            var fileSaved = Path.Combine(pathToSave, fileName);

            // lưu file vào thư mục
            using (var stream = new FileStream(fileSaved, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // cho phép truy cập file từ bên ngoài
            fileName = WebUtility.UrlEncode(fileName);

            // thêm đường dẫn truy cập file vào danh sách
            if (fileUrl is null)
            {
                throw new Exception("Đường dẫn truy cập file không hợp lệ");
            }

            fileUrls.Add(fileUrl);
        }


        return fileUrls;
    }
}