using Azure.Core;
using GraphAttachmentDownloaderSdk.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Serilog;

namespace GraphAttachmentDownloaderSdk.Services
{
    public class AttachmentProcessor
    {
        private readonly GraphServiceClient _graphClient;
        private readonly string _searchUser;

        public AttachmentProcessor(GraphServiceClient graphClient, IConfiguration config)
        {
            _graphClient = graphClient;
            _searchUser = config["Graph:SearchUser"];
        }

        public async Task<AttachmentMetadata> DownloadAttachmentsAsync()
        {
            var metadata = new AttachmentMetadata();

            var messagesResponse = await _graphClient.Users[_searchUser].Messages
                .GetAsync(requestConfig =>
                {
                    requestConfig.QueryParameters.Filter = "receivedDateTime ge 2025-07-17T00:00:00Z and hasAttachments eq true";
                    requestConfig.QueryParameters.Orderby = new[] { "receivedDateTime desc" };
                    requestConfig.QueryParameters.Top = 10;
                });

            foreach (var message in messagesResponse.Value)
            {
                var attachmentsResponse = await _graphClient.Users[_searchUser].Messages[message.Id].Attachments
                    .GetAsync();

                foreach (var attachment in attachmentsResponse.Value)
                {
                    var meta = new AttachmentMetadata.Entry
                    {
                        Name = attachment.Name,
                        ContentType = attachment.ContentType,
                        Size = attachment.Size ?? 0,
                        ODataType = attachment.OdataType,
                        IsInline = attachment.IsInline ?? false,
                        MessageSubject = message.Subject,
                        MessageId = message.Id,
                        AttachmentId = attachment.Id
                    };

                    if (attachment is FileAttachment file)
                    {
                        File.WriteAllBytes(file.Name, file.ContentBytes);
                        Log.Information($"Saved file: {file.Name}");
                    }
                    else if (attachment is ItemAttachment item)
                    {
                        var emlName = $"{message.Subject}.eml".Replace("/", "_");
                        var requestUrl = $"https://graph.microsoft.com/v1.0/users/{_searchUser}/messages/{message.Id}/attachments/{item.Id}/$value";

                        var tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
                        var token = await GraphClientFactory.Credential.GetTokenAsync(tokenRequestContext, default);

                        using var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);

                        var response = await httpClient.GetAsync(requestUrl);
                        response.EnsureSuccessStatusCode();

                        using var stream = await response.Content.ReadAsStreamAsync();
                        using var fs = new FileStream(emlName, FileMode.Create);
                        await stream.CopyToAsync(fs);

                        meta.MimeSavedAs = emlName;
                        Log.Information($"Saved embedded message: {emlName}");
                    }




                    metadata.TopLevelAttachments.Add(meta);
                }
            }

            return metadata;
        }
    }
}
