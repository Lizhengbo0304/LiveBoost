// 创建时间：2023-09-04-17:28
// 修改时间：2023-09-05-17:59

namespace LiveBoost.Tools;

public static class AesHelper
{
    /// <summary>
    ///     AES加密
    /// </summary>
    /// <param
    ///     name = "str" >
    ///     明文（待加密）
    /// </param>
    /// <param
    ///     name = "key" >
    ///     密文
    /// </param>
    /// <returns>
    ///     加密后的密文
    /// </returns>
    public static string? AesEncrypt(this string str, string key)
    {
        if ( string.IsNullOrEmpty(str) )
        {
            return null;
        }

        try
        {
            var toEncryptArray = Encoding.UTF8.GetBytes(str);

            var rm = Aes.Create();
            rm.Key = Encoding.UTF8.GetBytes(key);
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;

            var cTransform = rm.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch ( Exception e )
        {
            e.LogError("加密失败：");
            return string.Empty;
        }
    }
}
