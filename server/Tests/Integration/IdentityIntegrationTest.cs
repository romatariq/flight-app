using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using App.Public.DTO.v1.Identity;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Integration;


public class IdentityIntegrationTest : IClassFixture<CustomWebAppFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebAppFactory<Program> _factory;
    
    private readonly JsonSerializerOptions _camelCaseJsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    

    public IdentityIntegrationTest(CustomWebAppFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    
    [Fact(DisplayName = "POST - register new user")]
    public async Task RegisterNewUserTest()
    {
        // Arrange
        const string url = "/api/v1/identity/account/register";
        const string email = "register@test.ee";
        const string firstname = "TestFirst";
        const string lastname = "TestLast";
        const string password = "Foo.bar1";

        var registerData = new
        {
            Email = email,
            Password = password,
            Firstname = firstname,
            Lastname = lastname,
        };
        var data = JsonContent.Create(registerData);

        // Act
        var response = await _client.PostAsync(url, data);

        // Assert
        await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode);
    }
    

    [Fact(DisplayName = "POST - login user")]
    public async Task LoginUserTest()
    {
        const string email = "login@test.ee";
        const string firstname = "TestFirst";
        const string lastname = "TestLast";
        const string password = "Foo.bar1";
        const int expiresInSeconds = 1;

        // Arrange
        await RegisterNewUser(email, password, firstname, lastname);
        
        var url = $"/api/v1/identity/account/login?expiresInSeconds={expiresInSeconds}";

        var loginData = new
        {
            Email = email,
            Password = password,
        };

        var data = JsonContent.Create(loginData);

        // Act
        var response = await _client.PostAsync(url, data);

        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        VerifyJwtContent(responseContent, email, firstname, lastname,
            DateTime.Now.AddSeconds(expiresInSeconds + 1).ToUniversalTime());
    }

    [Fact(DisplayName = "POST - JWT expired")]
    public async Task JWTExpiredTest()
    {
        const string email = "expired@test.ee";
        const string firstname = "TestFirst";
        const string lastname = "TestLast";
        const string password = "Foo.bar1";
        const int expiresInSeconds = 3;

        const string url = "/api/v1/userFlights/getAll";

        // Arrange
        var jwt = await RegisterNewUserAndGetJwt(email, password, firstname, lastname, expiresInSeconds);
        var jwtResponse = JsonSerializer.Deserialize<JWTResponse>(jwt, _camelCaseJsonSerializerOptions);
        
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponse!.JWT);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        // Arrange
        await Task.Delay((expiresInSeconds + 2) * 1000);

        var request2 = new HttpRequestMessage(HttpMethod.Get, url);
        request2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponse.JWT);

        // Act
        var response2 = await _client.SendAsync(request2);

        // Assert
        Assert.False(response2.IsSuccessStatusCode);
    }


    [Fact(DisplayName = "POST - JWT renewal")]
    public async Task JWTRenewalTest()
    {
        const string email = "renewal@test.ee";
        const string firstname = "TestFirst";
        const string lastname = "TestLast";
        const string password = "Foo.bar1";
        const int expiresInSeconds = 3;

        const string url = "/api/v1/userFlights/getAll";

        // Arrange
        var jwt = await RegisterNewUserAndGetJwt(email, password, firstname, lastname, expiresInSeconds);
        var jwtResponse = JsonSerializer.Deserialize<JWTResponse>(jwt, _camelCaseJsonSerializerOptions);
        
        // let the jwt expire
        await Task.Delay((expiresInSeconds + 2) * 1000);

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponse!.JWT);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.False(response.IsSuccessStatusCode);

        // Arrange
        var refreshUrl = $"/api/v1/identity/account/refreshToken?expiresInSeconds={expiresInSeconds}";
        var refreshData = new
        {
            jwtResponse.JWT,
            jwtResponse.RefreshToken,
        };

        var data =  JsonContent.Create(refreshData);
        
        var response2 = await _client.PostAsync(refreshUrl, data);
        var responseContent2 = await response2.Content.ReadAsStringAsync();
        
        Assert.True(response2.IsSuccessStatusCode);
        
        jwtResponse = JsonSerializer.Deserialize<JWTResponse>(responseContent2, _camelCaseJsonSerializerOptions);

        var request3 = new HttpRequestMessage(HttpMethod.Get, url);
        request3.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtResponse!.JWT);

        // Act
        var response3 = await _client.SendAsync(request3);
        // Assert
        Assert.True(response3.IsSuccessStatusCode);
    }


    // HELPERS
    private async Task<string> RegisterNewUserAndGetJwt(string email, string password, string firstname, string lastname,
        int expiresInSeconds = 1)
    {
        await RegisterNewUser(email, password, firstname, lastname);
        return await LoginUserAndGetJwtContent(email, password, expiresInSeconds);
    }
    
    private async Task RegisterNewUser(string email, string password, string firstname, string lastname)
    {
        const string url = "/api/v1/identity/account/register";
        
        var registerData = new Register()
        {
            Email = email,
            Password = password,
            FirstName = firstname,
            LastName = lastname,
        };

        var data = JsonContent.Create(registerData);
        // Act
        var response = await _client.PostAsync(url, data);
        Assert.True(response.IsSuccessStatusCode);

        var adminJwt = await LoginAsAdminAndGetJwt();
        await VerifyUserAsAdmin(email, adminJwt);
    }
    
    private async Task<string> LoginAsAdminAndGetJwt()
    {
        var jwtContent = await LoginUserAndGetJwtContent("admin@app.com", "Foo.bar1", 10);
        var jwtResponse = JsonSerializer.Deserialize<JWTResponse>(jwtContent, _camelCaseJsonSerializerOptions);
        return jwtResponse!.JWT;
    }
    
    private async Task<string> LoginUserAndGetJwtContent(string email, string password, int expiresInSeconds = 1)
    {
        var url = $"/api/v1/identity/account/login?expiresInSeconds={expiresInSeconds}";
        var loginData = new Login()
        {
            Email = email,
            Password = password
        };
        var data = JsonContent.Create(loginData);
        var response = await _client.PostAsync(url, data);
        
        return await response.Content.ReadAsStringAsync();
    }
    
    private async Task VerifyUserAsAdmin(string email, string adminJwt)
    {
        const string url = "/api/v1/admin/userVerification/set";
        var verifyData = new VerifyUser()
        {
            Email = email,
            IsVerified = true
        };
        var data = JsonContent.Create(verifyData);
        
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);
        request.Content = data;
        var res = await _client.SendAsync(request);
        Assert.True(res.IsSuccessStatusCode);
    }
    

    private void VerifyJwtContent(string jwt, string email, string firstname, string lastname,
        DateTime validToIsSmallerThan)
    {
        var jwtResponse = JsonSerializer.Deserialize<JWTResponse>(jwt, _camelCaseJsonSerializerOptions);

        Assert.NotNull(jwtResponse);
        Assert.NotNull(jwtResponse.RefreshToken);
        Assert.NotNull(jwtResponse.JWT);

        // verify the actual JWT
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtResponse.JWT);
        Assert.Equal(email, jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);
        Assert.Equal(firstname, jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value);
        Assert.Equal(lastname, jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value);
        Assert.True(jwtToken.ValidTo < validToIsSmallerThan);
    }


}