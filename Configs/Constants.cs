namespace LearnIndentityAndAuthorization.Configs;

public static class Constants
{
    public const string AUTH_COOKIE_NAME = "Jwt";
    public const string FORMAT_DATETIME = "dd/MM/yyyy HH:mm:ss";
    public const int EXPRIED_TIME_OF_ACCESS_TOKEN = 120; // 2 tiếng 
    public const int EXPRIED_TIME_OF_REFRESH_TOKEN = 1; // 1 tháng
    public const int MAX_SIZE_OF_FILE_DEFAULT = 100; // 100MB
    public const int MAX_COUNT_OF_FILE_DEFAULT = 1; // 1 file
    public const string ALLOWED_EXTENSIONS_DEFAULT = ".jpg,.jpeg,.png,.gif,.bmp,.pdf,.doc,.docx,.xls,.xlsx, .txt, .mp4, .mp3";
    public const string ALLOWED_EXTENSIONS_IMAGE = ".jpg,.jpeg,.png,.gif,.bmp";
    public const string ALLOWED_EXTENSIONS_DOCUMENT = ".pdf,.doc,.docx,.xls,.xlsx, .txt";
    public const string ALLOWED_EXTENSIONS_VIDEO = ".mp4, .avi, .flv, .wmv, .mov";
}