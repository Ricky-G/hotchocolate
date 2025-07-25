using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using HotChocolate.Configuration;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Internal;
using HotChocolate.Language;
using HotChocolate.Language.Visitors;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.Data.Filters.Spatial;

public abstract class QueryableSpatialMethodHandler
    : FilterFieldHandler<QueryableFilterContext, Expression>
{
    private readonly IExtendedType _runtimeType;

    protected QueryableSpatialMethodHandler(
        IFilterConvention convention,
        ITypeInspector inspector,
        InputParser inputParser,
        MethodInfo method)
    {
        _runtimeType = inspector.GetReturnType(method);
        GeometryFieldName = convention.GetOperationName(SpatialFilterOperations.Geometry);
        BufferFieldName = convention.GetOperationName(SpatialFilterOperations.Buffer);
        InputParser = inputParser;
    }

    protected abstract int Operation { get; }

    protected string GeometryFieldName { get; }

    protected string BufferFieldName { get; }

    protected InputParser InputParser { get; }

    public override bool CanHandle(
        ITypeCompletionContext context,
        IFilterInputTypeConfiguration typeConfiguration,
        IFilterFieldConfiguration fieldConfiguration) =>
        fieldConfiguration is FilterOperationFieldConfiguration op
        && op.Id == Operation;

    public override bool TryHandleEnter(
        QueryableFilterContext context,
        IFilterField field,
        ObjectFieldNode node,
        [NotNullWhen(true)] out ISyntaxVisitorAction? action)
    {
        if (field is IFilterOperationField filterOperationField)
        {
            if (node.Value.IsNull())
            {
                context.ReportError(
                    ErrorHelper.CreateNonNullError(field, node.Value, context));
                action = SyntaxVisitor.Skip;
                return true;
            }

            if (!TryHandleOperation(
                context,
                filterOperationField,
                node,
                out var nestedProperty))
            {
                context.ReportError(
                    ErrorHelper.CouldNotCreateFilterForOperation(field, node.Value, context));
                action = SyntaxVisitor.Skip;
                return true;
            }

            context.RuntimeTypes.Push(_runtimeType);
            context.PushInstance(nestedProperty);
            action = SyntaxVisitor.Continue;
        }
        else
        {
            action = SyntaxVisitor.Break;
        }

        return true;
    }

    protected abstract bool TryHandleOperation(
        QueryableFilterContext context,
        IFilterOperationField field,
        ObjectFieldNode node,
        [NotNullWhen(true)] out Expression? result);

    public override bool TryHandleLeave(
        QueryableFilterContext context,
        IFilterField field,
        ObjectFieldNode node,
        [NotNullWhen(true)] out ISyntaxVisitorAction? action)
    {
        // Dequeue last
        var condition = context.GetLevel().Dequeue();

        context.PopInstance();
        context.RuntimeTypes.Pop();

        if (context.InMemory)
        {
            condition = FilterExpressionBuilder.NotNullAndAlso(
                context.GetInstance(),
                condition);
        }

        context.GetLevel().Enqueue(condition);
        action = SyntaxVisitor.Continue;
        return true;
    }

    protected bool TryGetParameter<T>(
        IFilterField parentField,
        IValueNode node,
        string fieldName,
        [NotNullWhen(true)] out T fieldNode)
        => SpatialOperationHandlerHelper
            .TryGetParameter(parentField, node, fieldName, InputParser, out fieldNode);
}
