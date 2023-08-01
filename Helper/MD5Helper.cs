
namespace android_backend.Helper
{
    public static class MD5Helper
    {
        public static string hash(this string str)
        {
            using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(str);
                var hash = cryptoMD5.ComputeHash(bytes);
                var md5 = BitConverter.ToString(hash)
                .Replace("-", String.Empty)
                .ToUpper();
                return md5;
            }
        }
    }

}