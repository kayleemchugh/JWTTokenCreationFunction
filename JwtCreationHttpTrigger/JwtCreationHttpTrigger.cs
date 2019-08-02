using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Net;

namespace JwtCreationHttpTrigger
{
    public static class JwtCreationHttpTrigger
    {
        [FunctionName("JwtCreationHttpTrigger")]
        public static HttpResponseMessage Run(
             [HttpTrigger(AuthorizationLevel.Anonymous, new string[] { "POST" })] TokenRequest secretKeyInput)
        {
            Console.WriteLine("STARTING PROCESS");
            // Define const Key this should be private secret key  stored in some safe place
            string key = "ewyrRfLNd7Rd5aEdEHm6Xx2dmFcg08SrKJ52J2nEAoQ=";
            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new Microsoft
               .IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            //
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
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
            Console.WriteLine(tokenString);
            Console.WriteLine("Consume Token");

            // And finally when  you received token from client
            // you can  either validate it or try to  read
            var token = handler.ReadJwtToken(tokenString);
            Console.WriteLine(token.Payload.First().Value);
            Console.ReadLine();
            //return tokenString;
            Console.WriteLine("C# HTTP trigger function processed a request.");
            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Content = new StringContent(tokenString);
            return response;


            //return tokenString != null
            //    ? (ActionResult)new OkObjectResult($"Hello, {tokenString}")
            //    : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }

    public class TokenRequest
    {
        public string secretKey { get; set; }
    }
}
