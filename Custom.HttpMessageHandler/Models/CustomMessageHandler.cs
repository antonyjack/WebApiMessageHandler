using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Custom.HttpMessageHandler.Models
{
    public class CustomMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue AuthValue = request.Headers.Authorization;
            if (AuthValue != null)
            {
                string username, password;
                ParseAuthorizationHeader(AuthValue.Parameter, out username, out password);
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    response.ReasonPhrase = "Invalid user credentials.";
                    var tsc = new TaskCompletionSource<HttpResponseMessage>();
                    tsc.SetResult(response);
                    return tsc.Task;
                }
                else
                {
                    if (username != "admin" || password != "admin")
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        response.ReasonPhrase = "Invalid user credentials.";
                        var tsc = new TaskCompletionSource<HttpResponseMessage>();
                        tsc.SetResult(response);
                        return tsc.Task;
                    }
                    else
                    {
                        HttpContext.Current.User = GeneratePrincipal(username);
                        Thread.CurrentPrincipal = HttpContext.Current.User;
                    }
                        
                }
            }
            else
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.ReasonPhrase = "Authorization header missing.";
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
            }
            return base.SendAsync(request, cancellationToken);
        }

        private void ParseAuthorizationHeader(string authHeader, out string userName, out string password)
        {

            userName = string.Empty;
            password = string.Empty;

            //Split back out and convert from Base64 string to get username and password supplied:
            string[] credentials = Encoding.UTF8.GetString(Convert
                                                            .FromBase64String(authHeader))
                                                            .Split(
                                                            new[] { ':' });

            //If *both* username and password are not defined, or if either is empty, return without further definition
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0])
                || string.IsNullOrEmpty(credentials[1]))
            {
                return;
            }

            //Define the output parameters
            userName = credentials[0];
            password = credentials[1];
        }

        private ClaimsPrincipal GeneratePrincipal(string username)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Name, username) }, "User"));
        }
    }
}