# How to implement messagehandler in asp.net webapi?

> There are many ways to secure your rest service. In this post, I would tell how to secure your rest service using basic authentication. To secure your service you should know basic things about HttpMessageHandler. In my previous post, I have explained the HttpMessageHandler and its usage. 

> The following example shows how to secure your WEB API request.

>We can handle by authenticating the request based on the authorization header. Authorization header must contain username and password in encoded string format.
> Authorization header format is : "Basic encodedvalue".

## DelegatingHandler:
```C#
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Custom.HttpMessageHandler.Models
{
    public class CustomMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthenticationHeaderValue AuthValue = request.Headers.Authorization;
            if (AuthValue != null)
            {
                //Authentication logic here
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
    }
}
```

> If the user does not provide authorization header within the request, you should return Unauthorized(401) response. If the user passes authorization header with the request, you can extract user credentials from the authorization header and do some authentication mechanism.

> The following code shows how to get the user credentials from authorization header.
```C#
protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
{
     AuthenticationHeaderValue AuthValue = request.Headers.Authorization;
     if (AuthValue != null)
     {
          string username, password;         
          string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(AuthValue.Parameter)).Split(new[] { ':' });
          userName = credentials[0];
	  password = credentials[1];
         
          // Authentication related code logic
     }
}
```


> If the user credentials are successfully authenticated then the request send it to service layer. 

> The following code shows how to pass the request from handler to the service layer.

```C#
protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
{
     AuthenticationHeaderValue AuthValue = request.Headers.Authorization;
     if (AuthValue != null)
     {
          string username, password;         
          string[] credentials = Encoding.UTF8.GetString(Convert.FromBase64String(AuthValue.Parameter)).Split(new[] { ':' });
          userName = credentials[0];
	  password = credentials[1];
          //If the username and password is not 'admin' the message handler throws unauthorized user response.
          if(username != "admin" && password != "admin")
          {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.ReasonPhrase = "Invalid user credentials.";
                var tsc = new TaskCompletionSource<HttpResponseMessage>();
                tsc.SetResult(response);
                return tsc.Task;
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
     return base.SendAsync(request, cancellationToken); //This code passes the request to service layer
}
```

## How to create claims identity inside message handler?
> After successfully authenticated, you can create custom claims principal and bind over the current principal. 

> The following code shows how to bind claims identity to current principal.

```C#
   HttpContext.Current.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() { new Claim(ClaimTypes.Name, "admin") }, "User"));
   Thread.CurrentPrincipal = HttpContext.Current.User;
```

> With the help of claims principal, we can achieve claimsauthorization mechanism in WEBAPI. 
