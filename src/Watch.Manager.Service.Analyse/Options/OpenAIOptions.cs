namespace Watch.Manager.Service.Analyse.Options;

/// <summary>
///     Represents configuration options for interacting with the OpenAI API.
/// </summary>
/// <remarks>
///     This class provides settings for specifying the behavior and parameters used when communicating with
///     the OpenAI API. For example, the <see cref="ChatModel" /> property determines which chat model is used for generating responses.
/// </remarks>
public sealed class OpenAIOptions
{
    /// <summary>The name of the chat model to use.</summary>
    /// <remarks>When using Azure OpenAI, this should be the "Deployment name" of the chat model.</remarks>
    public string ChatModel { get; set; } = "gpt-4o-mini";
}
