using Microsoft.Extensions.Primitives;

namespace HotChocolate.AspNetCore;

/// <summary>
/// Representation of a single media type entry from the accept header.
/// </summary>
public readonly struct AcceptMediaType
{
    /// <summary>
    /// Initializes a new instance of <see cref="AcceptMediaType"/>.
    /// </summary>
    /// <param name="type">
    /// The type of the media type header entry.
    /// </param>
    /// <param name="subType">
    /// The subtype of the media type header entry.
    /// </param>
    /// <param name="quality">
    /// The value of the quality parameter `q`.
    /// </param>
    /// <param name="charset">
    /// The charset.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Type or subtype are empty.
    /// </exception>
    internal AcceptMediaType(
        StringSegment type,
        StringSegment subType,
        double? quality,
        StringSegment charset)
    {
        if (!type.HasValue)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (!subType.HasValue)
        {
            throw new ArgumentNullException(nameof(subType));
        }

        Type = type.Value;
        SubType = subType.Value;
        Quality = quality;
        Charset = charset.HasValue ? charset.Value : null;
        IsUtf8 = Charset?.Equals("utf-8", StringComparison.OrdinalIgnoreCase) ?? true;

        if (Type.Equals(ContentType.Types.All, StringComparison.Ordinal)
            && SubType.Equals(ContentType.Types.All, StringComparison.Ordinal))
        {
            Kind = AcceptMediaTypeKind.All;
        }
        else if (Type.Equals(ContentType.Types.Application, StringComparison.Ordinal))
        {
            if (SubType.Equals(ContentType.Types.All, StringComparison.Ordinal))
            {
                Kind = AcceptMediaTypeKind.AllApplication;
            }
            else if (SubType.Equals(ContentType.SubTypes.GraphQLResponse, StringComparison.Ordinal))
            {
                Kind = AcceptMediaTypeKind.ApplicationGraphQL;
            }
            else if (SubType.Equals(ContentType.SubTypes.GraphQLResponseStream, StringComparison.Ordinal))
            {
                Kind = AcceptMediaTypeKind.ApplicationGraphQLStream;
            }
            else if (SubType.Equals(ContentType.SubTypes.Json, StringComparison.Ordinal))
            {
                Kind = AcceptMediaTypeKind.ApplicationJson;
            }
            else if (SubType.Equals(ContentType.SubTypes.JsonLines, StringComparison.Ordinal))
            {
                Kind = AcceptMediaTypeKind.ApplicationJsonLines;
            }
        }
        else if (Type.Equals(ContentType.Types.MultiPart, StringComparison.Ordinal))
        {
            if (SubType.Equals(ContentType.Types.All, StringComparison.Ordinal))
            {
                Kind = AcceptMediaTypeKind.AllMultiPart;
            }
            else if (SubType.Equals(ContentType.SubTypes.Mixed, StringComparison.Ordinal))
            {
                Kind = AcceptMediaTypeKind.MultiPartMixed;
            }
        }
        else if (Type.Equals(ContentType.Types.Text, StringComparison.Ordinal)
            && SubType.Equals(ContentType.SubTypes.EventStream, StringComparison.Ordinal))
        {
            Kind = AcceptMediaTypeKind.EventStream;
        }
        else
        {
            Kind = AcceptMediaTypeKind.Unknown;
        }
    }

    /// <summary>
    /// Gets the media type kind which is an enum representing well-known media type.
    /// </summary>
    public AcceptMediaTypeKind Kind { get; }

    /// <summary>
    /// Gets the type of the <see cref="AcceptMediaType"/>.
    /// </summary>
    /// <example>
    /// For the media type <c>"application/json"</c>,
    /// the property gives the value <c>"application"</c>.
    /// </example>
    /// <remarks>
    /// See <see href="https://tools.ietf.org/html/rfc6838#section-4.2"/>
    /// for more details on the type.
    /// </remarks>
    public string Type { get; }

    /// <summary>
    /// Gets the subtype of the <see cref="AcceptMediaType"/>.
    /// </summary>
    /// <example>
    /// For the media type <c>"application/vnd.example+json"</c>, the property gives the value
    /// <c>"vnd.example+json"</c>.
    /// </example>
    /// <remarks>
    /// See <see href="https://tools.ietf.org/html/rfc6838#section-4.2"/>
    /// for more details on the subtype.
    /// </remarks>
    public string SubType { get; }

    /// <summary>
    /// Gets or sets the value of the quality parameter. Returns null
    /// if there is no quality.
    /// </summary>
    public double? Quality { get; }

    /// <summary>
    /// Gets or sets the value of the charset parameter.
    /// Returns <c>null</c> if there is no charset.
    /// </summary>
    public string? Charset { get; }

    /// <summary>
    /// Defines if the charset is UTF-8.
    /// </summary>
    public bool IsUtf8 { get; }
}
