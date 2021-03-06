﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseStringLengthInsteadOfComparisonWithEmptyStringRefactoring
    {
        public static void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
        {
            var equalsExpression = (BinaryExpressionSyntax)context.Node;

            ExpressionSyntax left = equalsExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = equalsExpression.Right;

                if (right?.IsMissing == false)
                {
                    SemanticModel semanticModel = context.SemanticModel;
                    CancellationToken cancellationToken = context.CancellationToken;

                    if (CSharpAnalysis.IsEmptyString(left, semanticModel, cancellationToken))
                    {
                        if (IsString(right, semanticModel, cancellationToken))
                            ReportDiagnostic(context, equalsExpression);
                    }
                    else if (CSharpAnalysis.IsEmptyString(right, semanticModel, cancellationToken))
                    {
                        if (IsString(left, semanticModel, cancellationToken))
                            ReportDiagnostic(context, equalsExpression);
                    }
                }
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxNode node)
        {
            if (!node.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.UseStringLengthInsteadOfComparisonWithEmptyString,
                    node);
            }
        }

        private static bool IsString(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return expression?.IsMissing == false
                && semanticModel.GetTypeInfo(expression, cancellationToken).ConvertedType?.IsString() == true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax left = binaryExpression.Left;

            ExpressionSyntax right = binaryExpression.Right;

            BinaryExpressionSyntax newNode = binaryExpression;

            if (CSharpAnalysis.IsEmptyString(left, semanticModel, cancellationToken))
            {
                newNode = binaryExpression
                    .WithLeft(ZeroLiteralExpression())
                    .WithRight(CreateConditionalAccess(right));

            }
            else if (CSharpAnalysis.IsEmptyString(right, semanticModel, cancellationToken))
            {
                newNode = binaryExpression
                    .WithLeft(CreateConditionalAccess(left))
                    .WithRight(ZeroLiteralExpression());
            }
            else
            {
                Debug.Assert(false, binaryExpression.ToString());
                return document;
            }

            newNode = newNode.WithTriviaFrom(binaryExpression).WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static ConditionalAccessExpressionSyntax CreateConditionalAccess(ExpressionSyntax right)
        {
            return ConditionalAccessExpression(right, MemberBindingExpression(IdentifierName("Length")));
        }
    }
}
