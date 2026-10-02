using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RandoFile.Core;
using RandoFile.Core.Services;

namespace RandoFile.App.Services;

public enum UpdateStatus
{
    /// <summary>A newer release exists on GitHub.</summary>
    Available,

    /// <summary>The installed build is the newest one.</summary>
    UpToDate,

    /// <summary>The repository has no published release yet.</summary>
    NoRelease,

    /// <summary>GitHub could not be reached, or answered with an unexpected status.</summary>
    Failed,
}

/// <summary>Outcome of an update check.</summary>
/// <param name="Status">What the check found.</param>
/// <param name="Version">Version of the release that was found, without the leading "v".</param>
/// <param name="ReleaseUrl">Browser URL of that release.</param>
public sealed record UpdateCheckResult(UpdateStatus Status, string? Version = null, string? ReleaseUrl = null);

/// <summary>
/// Reads the latest release of the project repository from the public GitHub API.
/// The application only notifies; downloading and installing is left to the browser today,
/// which keeps the door open for a full auto-update later without changing the UI contract.
/// </summary>
public sealed class UpdateService
{
    private static readonly HttpClient Client = CreateClient();

    public async Task<UpdateCheckResult> CheckAsync(
        string owner,
        string repository,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(repository))
        {
            return new UpdateCheckResult(UpdateStatus.Failed);
        }

        try
        {
            var url = $"https://api.github.com/repos/{owner}/{repository}/releases/latest";
            using var response = await Client.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new UpdateCheckResult(UpdateStatus.NoRelease);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new UpdateCheckResult(UpdateStatus.Failed);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            var tag = document.RootElement.TryGetProperty("tag_name", out var tagElement)
                ? tagElement.GetString()
                : null;

            var releaseUrl = document.RootElement.TryGetProperty("html_url", out var urlElement)
                ? urlElement.GetString()
                : null;

            var version = VersionComparer.Normalize(tag);
            if (version is null)
            {
                return new UpdateCheckResult(UpdateStatus.NoRelease, ReleaseUrl: releaseUrl);
            }

            var status = VersionComparer.IsNewer(version, AppInfo.Version)
                ? UpdateStatus.Available
                : UpdateStatus.UpToDate;

            return new UpdateCheckResult(status, version, releaseUrl);
        }
        catch (OperationCanceledException)
        {
            return new UpdateCheckResult(UpdateStatus.Failed);
        }
        catch (Exception)
        {
            return new UpdateCheckResult(UpdateStatus.Failed);
        }
    }

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RandoFile", AppInfo.Version));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        return client;
    }
}
