using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;




namespace StorySpoiler_exam_project
{
    [TestFixture]
    public class StorySpoilerTests
    {
        private RestClient client;
        private static string createdStoryId;
        private static string baseURL = "https://d3s5nxhwblsjbi.cloudfront.net";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("evka86", "123456");


            var options = new RestClientOptions(baseURL)
            {
                Authenticator = new JwtAuthenticator(token)
            };

            client = new RestClient(options);
        }


        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseURL);

            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { username, password });

            var response = loginClient.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            return json.GetProperty("accessToken").GetString();
        }
        [Order(1)]
        [Test]
        public void CreateStory_ShouldReturnCreated()
        {
            var story = new
            {
                title = "Story Title",
                description = "Story Description",
                url = ""
            };
            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(story);
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode, "Expected status code 201 Created");

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            createdStoryId = json.GetProperty("storyId").GetString()??String.Empty;
            Assert.IsNotNull(createdStoryId, "Story ID should not be null");


        }
        [Order(2)]
        [Test]
        public void EditTheCreatedStory_ShouldReturnOK()
        {
            var changedStory = new
            {
                title = "Changed Story Title",
                description = "Changed Story Description",
                url = ""
            };
            var request = new RestRequest($"/api/Story/Edit/{createdStoryId}", Method.Put);
            request.AddJsonBody(changedStory);
            var response = client.Execute(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected status code 200 OK");
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            var msg = json.GetProperty("msg").GetString();
            Assert.AreEqual("Successfully edited", msg, "Expected success message for story edit");
        }
        [Order(3)]
        [Test]
        public void GetAllStorySpoilers_ShouldReturnAll()
        {
            var request = new RestRequest("/api/Story/All", Method.Get);
            var response = client.Execute(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected status code 200 OK");
            var json = JsonSerializer.Deserialize<List<object>>(response.Content);
           Assert.That(json,Is.Not.Empty, "Expected non-empty list of story spoilers");
        }
        [Order(4)]
        [Test]
        public void DeleteTheLastCreatedStorySpoiler_ShouldReturnDeleted()
        { 
            var request = new RestRequest($"/api/Story/Delete/{createdStoryId}", Method.Delete);
            var response = client.Execute(request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Expected status code 200 OK");
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            var msg = json.GetProperty("msg").GetString();
            Assert.AreEqual("Deleted successfully!", msg, "Expected success message for story deletion");
        }
        [Order(5)]
        [Test]
        public void CreateStoryWithoutRequiredFields_ShouldReturnBadRequest()
        {
            var story = new
            {
                title = "",
                description = "",
                url = ""
            };
            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(story);
            var response = client.Execute(request);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected status code 400 Bad Request");
           
        }
        [Order(6)]
        [Test]
        public void EditNonExistingStory_ShouldReturnNotFound()
        { 
        string nonExistingStoryId = "non-existing-id";
            var changedStory = new
            {
                title = "Changed Story Title",
                description = "Changed Story Description",
                url = ""
            };
            var request = new RestRequest($"/api/Story/Edit/{nonExistingStoryId}", Method.Put);
            request.AddJsonBody(changedStory);
            var response = client.Execute(request);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode, "Expected status code 404 Not Found");
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            var msg = json.GetProperty("msg").GetString();
            Assert.AreEqual("No spoilers...", msg, "Expected error message for non-existing story edit");
        }
        [Order(7)]
        [Test]
        public void DeleteNonExistingStory_ShouldReturnBadRequest()
        {
            string nonExistingStoryId = "non-existing-id";
            var request = new RestRequest($"/api/Story/Delete/{nonExistingStoryId}", Method.Delete);
            var response = client.Execute(request);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Expected status code 400 Bad Request");
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            var msg = json.GetProperty("msg").GetString();
            Assert.AreEqual("Unable to delete this story spoiler!", msg, "Expected error message for non-existing story deletion");


        }
        [OneTimeTearDown]
        public void Cleanup()
        {
            client?.Dispose();

        }
    }
}
