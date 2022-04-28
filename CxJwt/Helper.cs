using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Cx.Jwt
{
    //internal class JwtHelper
    //{
    //    public const string key_token = "logintoken";

    //    public static Task<string> SetToken(string loginkey, int expireDay)
    //    {
    //        var task1 = Task.Run(() =>
    //        {
    //            var claims = new[]
    //            {
    //                new Claim(ClaimTypes.Sid, loginkey),
    //             };

    //            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigExtensions.AppSettings["JWT:SecretKey"]));

    //            // 3. 选择加密算法
    //            var algorithm = SecurityAlgorithms.HmacSha256;

    //            // 4. 生成Credentials
    //            var signingCredentials = new SigningCredentials(secretKey, algorithm);

    //            // 6. 根据以上，生成token
    //            var token = new JwtSecurityToken(
    //                 ConfigExtensions.AppSettings["JWT:Issuer"],     //Issuer
    //                 ConfigExtensions.AppSettings["JWT:Audience"],   //Audience
    //                 claims,                          //Claims,
    //                 DateTime.Now,                    //notBefore
    //                 DateTime.Now.AddDays(expireDay),   //expires
    //                 signingCredentials               //Credentials
    //             );

    //            // 7. 将token变为string
    //            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
    //            //current.Response.Cookies.Append(LoginCookieKey, jwtToken);
    //            //current.Response.Headers.Append("Authentication", "Bearer " + jwtToken);
    //            return jwtToken;
    //        });
    //        return task1;
    //    }
    //}
}