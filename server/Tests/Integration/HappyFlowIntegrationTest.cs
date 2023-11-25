using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using App.Public.DTO.v1;
using App.Public.DTO.v1.Identity;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.Integration;


// register
// login
// go to airports page
// go to airport page
// go to airport reviews page
// post review
// edit review
// get review
// delete review
// logout
public class HappyFlowIntegrationTest : IClassFixture<CustomWebAppFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebAppFactory<Program> _factory;

    private readonly JsonSerializerOptions _camelCaseJsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    

    public HappyFlowIntegrationTest(CustomWebAppFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    
    
    [Fact(DisplayName = "Test main happy flow - all steps one by one")]
    public async Task MainHappyFlowTest()
    {
        // Arrange
        const string url = "/swagger/index.html";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        
        await RegisterUser();
    }


    /// <summary>
    /// Register new user.
    /// </summary>
    private async Task RegisterUser()
    {
        // Arrange
        const string url = "/api/v1/identity/account/register";
        const string email = "happy@flow.ee";
        const string firstname = "Happy";
        const string lastname = "Flow";
        const string password = "Foo.bar1";

        var registerData = new Register
        {
            Email = email,
            Password = password,
            FirstName = firstname,
            LastName = lastname,
        };
        var data = JsonContent.Create(registerData);

        // Act
        var response = await _client.PostAsync(url, data);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        await VerifyRegisteredUser(email);
    }

    
    /// <summary>
    /// Log in as admin.
    /// </summary>
    /// <returns>Admin's jwt.</returns>
    private async Task<string> LogInAsAdmin()
    {
        // Arrange
        const string url = "/api/v1/identity/account/login";
        var loginData = new Login()
        {
            Email = "admin@app.com",
            Password = "Foo.bar1"
        };
        var data = JsonContent.Create(loginData);
        
        // Act
        var response = await _client.PostAsync(url, data);
        
        var responseData = await response.Content.ReadAsStringAsync();
        var jwtResponse = JsonSerializer.Deserialize<JWTResponse>(responseData, _camelCaseJsonSerializerOptions);
        var jwt = jwtResponse?.JWT;
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(jwtResponse);
        Assert.NotNull(jwt);

        return jwt;
    }


    /// <summary>
    /// Admin verifies registered user.
    /// </summary>
    /// <param name="email">E-mail of the user to verify.</param>
    private async Task VerifyRegisteredUser(string email)
    {
        // Arrange
        const string url = "/api/v1/admin/userVerification/set";
        var adminJwt = await LogInAsAdmin();

        var verifyData = new VerifyUser()
        {
            Email = email,
            IsVerified = true
        };
        var data = JsonContent.Create(verifyData);
        
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminJwt);
        request.Content = data;
        
        // Act
        var res = await _client.SendAsync(request);
        
        // Assert
        Assert.True(res.IsSuccessStatusCode);

        await Login();
    }
    
    
    /// <summary>
    /// Log in as newly registered user.
    /// </summary>
    private async Task Login()
    {
        // Arrange
        const string url = "/api/v1/identity/account/login";

        var loginData = new Login()
        {
            Email = "happy@flow.ee",
            Password = "Foo.bar1",
        };

        var data = JsonContent.Create(loginData);

        // Act
        var response = await _client.PostAsync(url, data);

        var responseContent = await response.Content.ReadAsStringAsync();
        var jwtResponse = JsonSerializer.Deserialize<JWTResponse>(responseContent, _camelCaseJsonSerializerOptions);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(jwtResponse);
        Assert.NotNull(jwtResponse.JWT);
        Assert.NotNull(jwtResponse.RefreshToken);
        
        await GetAirports(jwtResponse);
    }
    
    
    /// <summary>
    /// Get airports data.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    private async Task GetAirports(JWTResponse jwt)
    {
        // Arrange
        const string url = "/api/v1/airports/airports?filter=tll";
        
        // Act
        var response = await _client.GetAsync(url);

        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Tallinn Airport", responseContent);

        await GetAirport(jwt);
    }
    
    
    /// <summary>
    /// Get airport data.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    private async Task GetAirport(JWTResponse jwt)
    {
        // Arrange
        const string url = "/api/v1/airports/airport?airportIata=tll";
        
        // Act
        var response = await _client.GetAsync(url);

        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Contains("Tallinn Airport", responseContent);
        Assert.Contains("\"displayFlights\":true", responseContent);

        await GetAirportReviews(jwt);
    }
    
    
    /// <summary>
    /// Get random review category id from all categories.
    /// </summary>
    /// <returns>Random review category id.</returns>
    private async Task<string> GetRandomReviewCategoryId()
    {
        // Arrange
        const string url = "/api/v1/reviews/categories";
        
        // Act
        var response = await _client.GetAsync(url);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<List<RecommendationCategory>>(responseContent, _camelCaseJsonSerializerOptions);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData);

        return responseData[0].Id.ToString();
    }
    
    
    /// <summary>
    /// Get reviews of airport.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    private async Task GetAirportReviews(JWTResponse jwt)
    {
        // Arrange
        var categoryId = await GetRandomReviewCategoryId();
        var url = $"/api/v1/reviews/getAllAuthorized?airportIata=tll&categoryId={categoryId}";
        
        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.JWT);
        var response = await _client.SendAsync(request);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<Recommendations>(responseContent, _camelCaseJsonSerializerOptions);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseData);
        Assert.Empty(responseData.Data);
        
        await AddAirportReview(jwt, categoryId);
    }
    
    
    /// <summary>
    /// Create new review.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    /// <param name="categoryId">Random category id.</param>
    private async Task AddAirportReview(JWTResponse jwt, string categoryId)
    {
        // Arrange
        const string url = "/api/v1/reviews/add";
        var addData = new
        {
            AirportIata = "TLL",
            Category = new
            {
                Id = categoryId,
                Category = ""
            },
            Rating = 5,
            Text = "Test review",
            AuthorName = ""
        };
        var data = JsonContent.Create(addData);

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.JWT);
        request.Content = data;
        
        // Act
        var response = await _client.SendAsync(request);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<Guid>(responseContent, _camelCaseJsonSerializerOptions);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotEqual(Guid.Empty, responseData);

        await EditAirportReview(jwt, categoryId, responseData.ToString());
    }
    
    
    /// <summary>
    /// Edit airport review.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    /// <param name="categoryId">Random category id.</param>
    /// <param name="reviewId"></param>
    private async Task EditAirportReview(JWTResponse jwt, string categoryId, string reviewId)
    {
        // Arrange
        const string url = "/api/v1/reviews/update";
        var updateData = new
        {
            Id = reviewId,
            AirportIata = "TLL",
            Category = new
            {
                Id = categoryId,
                Category = ""
            },
            Rating = 3,
            Text = "Test review, edited",
            AuthorName = ""
        };
        var data = JsonContent.Create(updateData);

        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.JWT);
        request.Content = data;
        
        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        
        await GetAirportReview(jwt, reviewId);
    }
    
    
    /// <summary>
    /// Get edited review.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    /// <param name="reviewId">Id of review to edit.</param>
    private async Task GetAirportReview(JWTResponse jwt, string reviewId)
    {
        // Arrange
        var url = $"/api/v1/reviews/get?reviewId={reviewId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.JWT);
        
        // Act
        var response = await _client.SendAsync(request);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<Recommendation>(responseContent, _camelCaseJsonSerializerOptions);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseData);
        Assert.Equal("Test review, edited", responseData.Text);
        Assert.Equal(3, responseData.Rating);
        Assert.Equal(reviewId, responseData.Id.ToString());

        await DeleteAirportReview(jwt, reviewId);
    }
    
    
    /// <summary>
    /// Delete review.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    /// <param name="reviewId">Id of review to delete.</param>
    private async Task DeleteAirportReview(JWTResponse jwt, string reviewId)
    {
        // Arrange
        var url = $"/api/v1/reviews/delete?id={reviewId}";

        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.JWT);
        
        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        
        await LogOut(jwt);
    }
    
    
    /// <summary>
    /// Log user out.
    /// </summary>
    /// <param name="jwt">User's jwt.</param>
    private async Task LogOut(JWTResponse jwt)
    {
        // Arrange
        const string url = "/api/v1/identity/account/logout";

        var logoutData = new Logout
        {
            RefreshToken = jwt.RefreshToken
        };
        var data = JsonContent.Create(logoutData);

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt.JWT);
        request.Content = data;
        
        // Act
        var response = await _client.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<LogoutResponse>(responseContent, _camelCaseJsonSerializerOptions);
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(responseData);
        Assert.Equal(1, responseData.TokenDeleteCount);
    }
}