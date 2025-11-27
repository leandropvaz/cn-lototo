using Blazored.LocalStorage;
using CN.Lototo.Client.Dto;
using System.Net.Http.Headers;

namespace CN.Lototo.Client.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            var user = await _localStorage.GetItemAsync<LoginResponseDto>("user");

            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (user != null)
                request.Headers.Add("X-Tenant-Id", user.PlantaId.ToString());

            return await base.SendAsync(request, cancellationToken);
        }

    }
}
