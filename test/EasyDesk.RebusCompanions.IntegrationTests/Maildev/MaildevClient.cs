using EasyDesk.CleanArchitecture.Application.Json;
using EasyDesk.Commons.Collections;
using EasyDesk.Commons.Options;
using System.Text.Json;

namespace EasyDesk.RebusCompanions.IntegrationTests.Maildev;

public class MaildevClient
{
    private readonly List<MaildevMail> _readEmails = [];

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public MaildevClient(Uri rootUri, JsonOptionsConfigurator jsonConfigurator)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = rootUri,
        };
        _jsonOptions = jsonConfigurator.CreateOptions();
    }

    public async Task<IEnumerable<MaildevMail>> GetEmails()
    {
        var emails = await RetrieveNewEmails();

        _readEmails.AddRange(emails);

        return _readEmails;
    }

    private async Task<IEnumerable<MaildevMail>> RetrieveNewEmails()
    {
        using var response = await _httpClient.GetAsync("email");
        using var content = await response.Content.ReadAsStreamAsync();
        var emails = JsonSerializer.Deserialize<IEnumerable<MaildevMailDto>>(content, _jsonOptions) ?? throw new Exception("Maildev returned null response");
        var result = new List<MaildevMail>();

        foreach (var email in emails.Skip(_readEmails.Count))
        {
            result.Add(await ConvertToMailObject(email));
        }
        return result;
    }

    private async Task<MaildevMail> ConvertToMailObject(MaildevMailDto dto)
    {
        var convertedAttachments = new List<MaildevAttachment>();
        foreach (var attachment in dto.Attachments.Flatten())
        {
            convertedAttachments.Add(await ConvertToAttachmentObject(attachment, dto.Id));
        }
        return new(
            From: dto.From.Select(x => x.Address),
            To: dto.To.Select(x => x.Address),
            Subject: dto.Subject,
            Html: dto.Html,
            Text: dto.Text,
            Attachments: convertedAttachments);
    }

    private async Task<MaildevAttachment> ConvertToAttachmentObject(MaildevAttachmentDto dto, string emailId)
    {
        using var response = await _httpClient.GetAsync($"email/{emailId}/attachment/{dto.FileName}");
        var content = await response.Content.ReadAsStringAsync();
        return new(
            ContentType: dto.ContentType,
            FileName: dto.FileName,
            Content: content);
    }

    public async Task DeleteAllEmails()
    {
        await _httpClient.DeleteAsync("email/all");
        _readEmails.Clear();
    }

    private record MaildevMailDto(
        string Id,
        IEnumerable<MaildevAccountDto> From,
        IEnumerable<MaildevAccountDto> To,
        string Subject,
        string Html,
        string Text,
        Option<IEnumerable<MaildevAttachmentDto>> Attachments);

    private record MaildevAccountDto(string Address);

    private record MaildevAttachmentDto(string ContentType, string FileName);
}

public record MaildevMail(
    IEnumerable<string> From,
    IEnumerable<string> To,
    string Subject,
    string Html,
    string Text,
    IEnumerable<MaildevAttachment> Attachments);

public record MaildevAttachment(
    string ContentType,
    string FileName,
    string Content);
