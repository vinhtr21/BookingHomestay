namespace Go2Hotel.Payment
{
    public static class Helper
    {
        public static string GetIpAddress(this HttpContext httpContext)
        {
            string ipAddress = httpContext.Connection.RemoteIpAddress.ToString();

            if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var values))
            {
                ipAddress = values.FirstOrDefault();
            }

            if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown" || ipAddress.Length > 45)
            {
                ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
            }

            return ipAddress;
        }
    }
}
