using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Please provide a GitHub username.");
            return;
        }

        string username = args[0];
        await FetchGitHubActivity(username);
    }

    static async Task FetchGitHubActivity(string username)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // GitHub API requires a user-agent header
                client.DefaultRequestHeaders.Add("User-Agent", "GitHubActivityCLI");

                // Make the API request
                string apiUrl = $"https://api.github.com/users/{username}/events";
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                // Handle invalid username or API failure
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to fetch activity for user '{username}'. Status code: {response.StatusCode}");
                    return;
                }

                // Parse the response JSON
                string content = await response.Content.ReadAsStringAsync();
                JArray activities = JArray.Parse(content);

                // Display the user's recent activity
                if (activities.Count == 0)
                {
                    Console.WriteLine($"No activity found for user '{username}'.");
                    return;
                }

                Console.WriteLine($"Recent activity for user '{username}':\n");
                foreach (var activity in activities)
                {
                    string type = activity["type"]?.ToString();
                    string repoName = activity["repo"]?["name"]?.ToString();

                    if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(repoName))
                    {
                        Console.WriteLine($"- {type} in repository {repoName}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
