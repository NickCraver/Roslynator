﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddIdentifierToLocalDeclarationRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            TypeSyntax type = declaration?.Type;

            if (type?.IsVar == false)
            {
                VariableDeclaratorSyntax declarator = declaration.Variables.FirstOrDefault();

                if (declarator != null
                    && context.Span.Start >= type.Span.Start)
                {
                    SyntaxTriviaList triviaList = type.GetTrailingTrivia();

                    if (triviaList.Any())
                    {
                        SyntaxTrivia trivia = triviaList
                            .SkipWhile(f => f.IsKind(SyntaxKind.WhitespaceTrivia))
                            .FirstOrDefault();

                        if (trivia.IsKind(SyntaxKind.EndOfLineTrivia)
                            && context.Span.End <= trivia.Span.Start)
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                            if (typeSymbol?.IsErrorType() == false)
                            {
                                string name = Identifier.CreateName(typeSymbol, firstCharToLower: true);
                                name = Identifier.EnsureUniqueLocalName(name, declarator.SpanStart, semanticModel, context.CancellationToken);

                                if (!string.IsNullOrEmpty(name))
                                {
                                    context.RegisterRefactoring(
                                        $"Add identifier '{name}'",
                                        c => RefactorAsync(context.Document, type, name, c));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionStatementSyntax expressionStatement)
        {
            var expression = expressionStatement.Expression as TypeSyntax;

            if (expression != null)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol?.IsErrorType() == false)
                {
                    string name = Identifier.CreateName(typeSymbol, firstCharToLower: true);
                    name = Identifier.EnsureUniqueLocalName(name, expression.SpanStart, semanticModel, context.CancellationToken);

                    if (!string.IsNullOrEmpty(name))
                    {
                        context.RegisterRefactoring(
                            $"Add identifier '{name}'",
                            c => RefactorAsync(context.Document, expressionStatement, name, c));
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            TypeSyntax type,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTrivia endOfLine = type.GetTrailingTrivia()
                .SkipWhile(f => f.IsKind(SyntaxKind.WhitespaceTrivia))
                .First();

            TextSpan span = TextSpan.FromBounds(type.Span.End, endOfLine.Span.Start);

            var textChange = new TextChange(span, " " + name);

            return await document.WithTextChangeAsync(textChange, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax expressionStatement,
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LocalDeclarationStatementSyntax newNode = LocalDeclarationStatement(
                VariableDeclaration(
                    (TypeSyntax)expressionStatement.Expression,
                    VariableDeclarator(name)));

            if (expressionStatement.SemicolonToken.IsMissing)
            {
                newNode = newNode
                    .WithSemicolonToken(expressionStatement.SemicolonToken)
                    .WithTriviaFrom(expressionStatement.Expression);
            }
            else
            {
                newNode = newNode.WithTriviaFrom(expressionStatement);
            }

            return await document.ReplaceNodeAsync(expressionStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
