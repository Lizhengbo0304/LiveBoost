// 创建时间：2023-09-04-17:28
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Toolkit.Tools;

public static class AesHelper
{
    /// <summary>
    ///     AES加密
    /// </summary>
    /// <param name = "str" > 明文（待加密） </param>
    /// <param name = "key" > 密文 </param>
    /// <returns> 加密后的密文 </returns>
    public static string? AesEncrypt(this string str, string key)
    {
        if ( string.IsNullOrEmpty(str) )
        {
            return null;
        }

        try
        {
            // 将明文字符串转换为字节数组
            var toEncryptArray = Encoding.UTF8.GetBytes(str);

            // 创建AES加密算法实例
            var rm = Aes.Create();
            rm.Key = Encoding.UTF8.GetBytes(key);
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;

            // 创建加密转换器并对明文字节数组进行加密
            var cTransform = rm.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            // 将加密后的字节数组转换为Base64字符串
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        catch ( Exception e )
        {
            // 记录错误信息并返回空字符串
            e.LogError("加密失败：");
            return string.Empty;
        }
    }
}
