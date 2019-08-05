using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Net;

namespace JwtCreationHttpTrigger
{
    public class JwtCreationHttpTrigger
    {
        [FunctionName("JwtCreationHttpTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            string secretKey = req.Query["secretKey"];

            Console.WriteLine("STARTING PROCESS");

            // Define const Key this should be private secret key  stored in some safe place           
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
         
            var credentials = new SigningCredentials
                              (securityKey, SecurityAlgorithms.HmacSha256Signature);

            //  Finally create a Token
            var header = new JwtHeader(credentials);

            //Some PayLoad that contain information about the  customer
            var payload = new JwtPayload
           {
               {"iat", DateTime.Now.ToString("yyyyMMdd") },
               {"exp", DateTime.Now.AddDays(90).ToString("yyyyMMdd") }
           };
            //
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            string tokenString = handler.WriteToken(secToken);
            Console.WriteLine("Security token" + tokenString);

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Content = new StringContent(tokenString);
            return response;

        }

    }

}
