using System.Collections;
using HotChocolate.Language;
using NetTopologySuite.Geometries;
using static HotChocolate.Types.Spatial.ThrowHelper;
using static HotChocolate.Types.Spatial.WellKnownFields;

namespace HotChocolate.Types.Spatial.Serialization;

internal abstract class GeoJsonInputObjectSerializer<T>
    : GeoJsonSerializerBase<T>
    where T : Geometry
{
    private readonly GeoJsonGeometryType _geometryType;

    protected GeoJsonInputObjectSerializer(GeoJsonGeometryType geometryType)
    {
        _geometryType = geometryType;
    }

    public abstract T CreateGeometry(IType type, object? coordinates, int? crs);

    public override bool TrySerialize(
        IType type,
        object? runtimeValue,
        out object? resultValue)
    {
        ArgumentNullException.ThrowIfNull(type);

        try
        {
            if (runtimeValue is null)
            {
                resultValue = null;
                return true;
            }

            if (runtimeValue is IReadOnlyDictionary<string, object>
                || runtimeValue is IDictionary<string, object>)
            {
                resultValue = runtimeValue;
                return true;
            }

            if (runtimeValue is Geometry g
                && TrySerializeCoordinates(type, g, out var coordinate)
                && coordinate is { })
            {
                resultValue = new Dictionary<string, object>
                    {
                        { CoordinatesFieldName, coordinate },
                        {
                            TypeFieldName,
                            GeoJsonTypeSerializer.Default.Serialize(type, _geometryType) ??
                            throw Serializer_CouldNotSerialize(type)
                        },
                        { CrsFieldName, g.SRID }
                    };

                return true;
            }

            resultValue = null;
            return false;
        }
        catch
        {
            resultValue = null;
            return false;
        }
    }

    public override bool IsInstanceOfType(IType type, IValueNode literal)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(literal);

        return literal is ObjectValueNode
            || literal is NullValueNode;
    }

    public override bool IsInstanceOfType(IType type, object? runtimeValue)
    {
        ArgumentNullException.ThrowIfNull(type);

        return runtimeValue is T t
            && GeoJsonTypeSerializer.Default.TryParseString(
                t.GeometryType,
                out var g)
            && g == _geometryType;
    }

    public override object? ParseLiteral(IType type, IValueNode valueSyntax)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (valueSyntax is NullValueNode)
        {
            return null;
        }

        if (valueSyntax is ObjectValueNode obj)
        {
            (var geometryType, var coordinates, var crs) =
                ParseFields(type, obj);

            if (geometryType != _geometryType)
            {
                throw Serializer_Parse_TypeIsInvalid(type);
            }

            return CreateGeometry(type, coordinates, crs);
        }

        throw Serializer_Parse_ValueKindInvalid(type, valueSyntax.Kind);
    }

    public override IValueNode ParseResult(IType type, object? resultValue)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (resultValue is null)
        {
            return NullValueNode.Default;
        }

        if (resultValue is IReadOnlyDictionary<string, object> dict)
        {
            var list = new List<ObjectFieldNode>
                {
                    new ObjectFieldNode(
                        TypeFieldName,
                        GeoJsonTypeSerializer.Default.ParseResult(type, _geometryType))
                };

            if (dict.TryGetValue(CoordinatesFieldName, out var value)
                && value is IList coordinates)
            {
                list.Add(
                    new ObjectFieldNode(
                        CoordinatesFieldName,
                        ParseCoordinateResult(type, coordinates)));
            }

            if (dict.TryGetValue(CrsFieldName, out value) && value is int crs)
            {
                list.Add(
                    new ObjectFieldNode(
                        CrsFieldName,
                        new IntValueNode(crs)));
            }

            return new ObjectValueNode(list);
        }

        if (resultValue is T)
        {
            return ParseValue(type, resultValue);
        }

        throw Serializer_CouldNotParseValue(type);
    }

    public override IValueNode ParseValue(IType type, object? runtimeValue)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (runtimeValue is null)
        {
            return NullValueNode.Default;
        }

        if (runtimeValue is IReadOnlyDictionary<string, object> dict)
        {
            return ParseResult(type, dict);
        }

        if (runtimeValue is T geometry)
        {
            var list = new List<ObjectFieldNode>
                {
                    new ObjectFieldNode(
                        TypeFieldName,
                        GeoJsonTypeSerializer.Default.ParseResult(type, _geometryType)),
                    new ObjectFieldNode(
                        CoordinatesFieldName,
                        ParseCoordinateValue(type, geometry)),
                    new ObjectFieldNode(
                        CrsFieldName,
                        new IntValueNode(geometry.SRID))
                };

            return new ObjectValueNode(list);
        }

        throw Serializer_CouldNotParseValue(type);
    }

    public override bool TryDeserialize(
        IType type,
        object? resultValue,
        out object? runtimeValue)
    {
        ArgumentNullException.ThrowIfNull(type);

        try
        {
            switch (resultValue)
            {
                case null:
                    runtimeValue = null;
                    return true;

                case IReadOnlyDictionary<string, object> dict:
                    (var geometryType, var coordinates, var crs) =
                        ParseFields(type, dict);

                    if (geometryType != _geometryType)
                    {
                        runtimeValue = null;
                        return false;
                    }

                    runtimeValue = CreateGeometry(type, coordinates, crs);
                    return true;

                case T:
                    runtimeValue = resultValue;
                    return true;

                default:
                    runtimeValue = null;
                    return false;
            }
        }
        catch
        {
            runtimeValue = null;
            return false;
        }
    }
}
