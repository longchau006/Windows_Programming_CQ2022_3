﻿using System.Collections.Generic;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Linq;
using Windows_Programming.Model.GeminiService;
using Windows_Programming.Database;
using Windows_Programming.View;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using Windows.Storage;
//=======================================================================================
public class GeminiService
{
    private readonly HttpClient _httpClient;
    private const string API_ENDPOINT = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

    //====================================================Function Setup

    private readonly List<RequestGemini.FunctionDefinition> _functionDefinitions = new()
    {
         new RequestGemini.FunctionDefinition
        {
            Name = "GeneralConversation",
            Description = "Handle general conversation and messages that don't match other functions",
            Parameters = new Dictionary<string, RequestGemini.ParameterDefinition>
            {
                { "message", new RequestGemini.ParameterDefinition { Type = "string", Description = "The user's message" } }
            }
        },
         new RequestGemini.FunctionDefinition
         {
             Name = "ChangeFullname",
             Description = "Change user fullname",
             Parameters = new Dictionary<string, RequestGemini.ParameterDefinition>
             {
                 {"fullname", new RequestGemini.ParameterDefinition {Type = "string", Description = "User's fullname" } }
             }
         },
         new RequestGemini.FunctionDefinition
         {
             Name = "ChangeAddress",
             Description = "Change user address",
             Parameters = new Dictionary<string, RequestGemini.ParameterDefinition>
             {
                 {"address", new RequestGemini.ParameterDefinition {Type = "string", Description = "User's address" } }
             }
         },
         new RequestGemini.FunctionDefinition
         {
             Name = "AddBlog",
             Description = "Add a blog to the database",
             Parameters = new Dictionary<string, RequestGemini.ParameterDefinition>
             {
                 {"title", new RequestGemini.ParameterDefinition {Type = "string", Description = "Title of the blog" } },
                 {"content", new RequestGemini.ParameterDefinition {Type = "string", Description = "Automatically generate content with the number of words entered. Otherwise, the content is based on what the user enters" } },
                 {"image", new RequestGemini.ParameterDefinition {Type = "string", Description = "Image of the blog" } }
             }
         }
    };

    

    //================Init Gemini Services
    public GeminiService(string apiKey)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
    }


    //============================Send Message
    public async Task<string> ProcessPrompt(string prompt)
    {
        System.Diagnostics.Debug.WriteLine("Vao ham Process 1");
        var functionsJson = JsonSerializer.Serialize(_functionDefinitions);
        System.Diagnostics.Debug.WriteLine("Vao ham Process 2");
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = $@"
                            You are an AI assistant that handles both specific functions and general queries.
                            Available functions: {functionsJson}

                            User request: {prompt}

                            If the request matches any defined function in the available functions list, use that function.
                            For all other requests, provide a detailed, informative response using GeneralConversation.

                            Respond ONLY with a JSON object containing:
                            {{
                                ""function"": ""function_name"",
                                ""parameters"": {{parameter_values}}
                            }}

                            For general queries, ensure the response is comprehensive and informative."
                        }
                    }
                }
            }
        };
        System.Diagnostics.Debug.WriteLine("Vao ham Process 3");
        var response = await _httpClient.PostAsJsonAsync(API_ENDPOINT, requestBody);
        var responseContent = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine($"--------------->API Response: {responseContent}");

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var geminiResponse = JsonSerializer.Deserialize<ResponseGemini.GeminiResponse>(responseContent, options);
            var functionCallJson = geminiResponse.Candidates[0].Content.Parts[0].Text;

            System.Diagnostics.Debug.WriteLine($"--------------->Function Call JSON: {functionCallJson}");

            var functionCall = JsonSerializer.Deserialize<FunctionCallResponse>(functionCallJson, options);

            System.Diagnostics.Debug.WriteLine($"--------------->Parsed Function: {functionCall.Function}");
            System.Diagnostics.Debug.WriteLine($"--------------->Parameters: {string.Join(", ", functionCall.Parameters.Select(p => $"{p.Key}={p.Value}"))}");

            return await ExecuteFunction(functionCall);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"--------------->Error: {ex.Message}");
            return $"Error processing response: {ex.Message}";
        }
    }

    //Detach function name and parameter of function
    private class FunctionCallResponse
    {
        [JsonPropertyName("function")]
        public string Function { get; set; }

        [JsonPropertyName("parameters")]
        public Dictionary<string, string> Parameters { get; set; }
    }



    //Do function and return result to MainWindow.cs
    private async Task<string> ExecuteFunction(FunctionCallResponse functionCall)
    {
        switch (functionCall.Function)
        {
            case "ChangeFullname":
                return await ChangeFullname(functionCall.Parameters["fullname"]);
            case "ChangeAddress":
                return await ChangeAddress(functionCall.Parameters["address"]);
            case "GeneralConversation":
                return HandleGeneralConversation(functionCall.Parameters["message"]);
            case "AddBlog":
                return await AddBlog(functionCall.Parameters["title"], functionCall.Parameters["content"], functionCall.Parameters["image"]);
            default:
                return HandleGeneralConversation(functionCall.Parameters["message"]);
        }
    }



    //All Function Demo
    private async Task<string> ChangeFullname(string fullname)
    {
        if (string.IsNullOrWhiteSpace(fullname))
        {
            return "Fullname cannot be empty";
        }
        IDao dao = FirebaseServicesDAO.Instance;
        await dao.UpdateFullName(fullname, MainWindow.MyAccount.Id);
        return "Change fullname successfully";
    }

    private async Task<string> ChangeAddress(string address)
    {
        if(string.IsNullOrWhiteSpace(address))
        {
            return "Address cannot be empty";
        }
        IDao dao = FirebaseServicesDAO.Instance;
        await dao.UpdateAddress(address, MainWindow.MyAccount.Id);
        return "Change address successfully";
    }

    private async Task<string> AddBlog(string title, string content, string image)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(image))
        {
            return "Title, content and image cannot be empty";
        }
        Blog blog = new Blog();
        blog.Title = title;
        blog.Content = content;
        blog.PublishDate = DateTime.Now;
        blog.Author = MainWindow.MyAccount.Id;

        var httpClient = new System.Net.Http.HttpClient();
        byte[] imageBytes = await httpClient.GetByteArrayAsync(image);// get folder Assets of current app
        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;

        StorageFile storageFile = await storageFolder.CreateFileAsync("image.jpg", CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteBytesAsync(storageFile, imageBytes);
        blog.Image = storageFile.Path;

        BlogViewModel blogViewModel = new BlogViewModel();
        await blogViewModel.AddBlog(blog);
        return "Add blog successfully";
    }

    private void PrintABC()
    {
        System.Diagnostics.Debug.WriteLine("--------------->Print ABCD");
    }
    private string HandleGeneralConversation(string message)
    {
        Console.WriteLine($"=====================Genneral user: {message}");
        System.Diagnostics.Debug.WriteLine($"===================---->General user: {message}");
        return message;
    }

}