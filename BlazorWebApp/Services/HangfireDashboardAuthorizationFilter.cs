using System.Net;
using System.Security.Cryptography;
using System.Text;

using Hangfire.Dashboard;

namespace BlazorWebApp.Services
{
	public class HangfireDashboardAuthorizationFilter(string username, string password) : IDashboardAuthorizationFilter
	{
		private readonly string _username = username;
		private readonly string _password = password;

		public bool Authorize(DashboardContext context)
		{
			var httpContext = context.GetHttpContext();

			if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
			{
				httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				return false;
			}

			var header = httpContext.Request.Headers.Authorization.ToString();
			if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
			{
				Challenge(httpContext);
				return false;
			}

			var encodedCredentials = header["Basic ".Length..].Trim();
			string decodedCredentials;

			try
			{
				decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
			}
			catch
			{
				Challenge(httpContext);
				return false;
			}

			var separatorIndex = decodedCredentials.IndexOf(':');
			if (separatorIndex <= 0)
			{
				Challenge(httpContext);
				return false;
			}

			var requestUsername = decodedCredentials[..separatorIndex];
			var requestPassword = decodedCredentials[(separatorIndex + 1)..];

			var usernameMatches = FixedTimeEquals(requestUsername, _username);
			var passwordMatches = FixedTimeEquals(requestPassword, _password);

			if (!usernameMatches || !passwordMatches)
			{
				Challenge(httpContext);
				return false;
			}

			return true;
		}

		private static bool FixedTimeEquals(string left, string right)
		{
			var leftBytes = Encoding.UTF8.GetBytes(left);
			var rightBytes = Encoding.UTF8.GetBytes(right);

			return leftBytes.Length == rightBytes.Length
				&& CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
		}

		private static void Challenge(HttpContext httpContext)
		{
			httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
		}
	}
}