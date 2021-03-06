﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SplitVariableDeclarationRefactoring
    {
        public static bool CanRefactor(VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration == null)
                throw new ArgumentNullException(nameof(variableDeclaration));

            switch (variableDeclaration.Parent?.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return variableDeclaration.Variables.Count > 1;
            }

            return false;
        }

        public static string GetTitle(VariableDeclarationSyntax variableDeclaration)
        {
            if (variableDeclaration == null)
                throw new ArgumentNullException(nameof(variableDeclaration));

            return $"Split {GetName(variableDeclaration)} declaration";
        }

        private static string GetName(VariableDeclarationSyntax variableDeclaration)
        {
            switch (variableDeclaration.Parent?.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    return "local";
                case SyntaxKind.FieldDeclaration:
                    return "field";
                case SyntaxKind.EventFieldDeclaration:
                    return "event";
            }

            return "variable";
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            VariableDeclarationSyntax variableDeclaration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (variableDeclaration.Parent?.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    return await SplitLocalDeclarationAsync(document, (LocalDeclarationStatementSyntax)variableDeclaration.Parent, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.FieldDeclaration:
                    return await SplitFieldDeclarationAsync(document, (FieldDeclarationSyntax)variableDeclaration.Parent, cancellationToken).ConfigureAwait(false);
                case SyntaxKind.EventFieldDeclaration:
                    return await SplitEventFieldDeclarationAsync(document, (EventFieldDeclarationSyntax)variableDeclaration.Parent, cancellationToken).ConfigureAwait(false);
                default:
                    return document;
            }
        }

        private static async Task<Document> SplitLocalDeclarationAsync(
            Document document,
            LocalDeclarationStatementSyntax statement,
            CancellationToken cancellationToken)
        {
            var block = (BlockSyntax)statement.Parent;

            SyntaxList<StatementSyntax> newStatements = block.Statements.ReplaceRange(
                statement,
                SplitLocalDeclaration(statement));

            BlockSyntax newBlock = block.WithStatements(newStatements);

            return await document.ReplaceNodeAsync(block, newBlock, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> SplitFieldDeclarationAsync(
            Document document,
            FieldDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            var containingMember = (MemberDeclarationSyntax)declaration.Parent;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceRange(
                declaration,
                SplitFieldDeclaration(declaration));

            MemberDeclarationSyntax newNode = containingMember.SetMembers(newMembers);

            return await document.ReplaceNodeAsync(containingMember, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> SplitEventFieldDeclarationAsync(
            Document document,
            EventFieldDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
            var containingMember = (MemberDeclarationSyntax)declaration.Parent;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.ReplaceRange(
                declaration,
                SplitEventFieldDeclaration(declaration));

            MemberDeclarationSyntax newNode = containingMember.SetMembers(newMembers);

            return await document.ReplaceNodeAsync(containingMember, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static IEnumerable<LocalDeclarationStatementSyntax> SplitLocalDeclaration(LocalDeclarationStatementSyntax statement)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = statement.Declaration.Variables;

            LocalDeclarationStatementSyntax statement2 = statement.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                LocalDeclarationStatementSyntax newStatement = LocalDeclarationStatement(
                    statement2.Modifiers,
                    VariableDeclaration(
                        statement2.Declaration.Type,
                        SingletonSeparatedList(variables[i])));

                if (i == 0)
                    newStatement = newStatement.WithLeadingTrivia(statement.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newStatement = newStatement.WithTrailingTrivia(statement.GetTrailingTrivia());

                yield return newStatement.WithFormatterAnnotation();
            }
        }

        private static IEnumerable<FieldDeclarationSyntax> SplitFieldDeclaration(FieldDeclarationSyntax declaration)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Declaration.Variables;

            FieldDeclarationSyntax declaration2 = declaration.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                FieldDeclarationSyntax newDeclaration = FieldDeclaration(
                    declaration2.AttributeLists,
                    declaration2.Modifiers,
                    VariableDeclaration(
                        declaration2.Declaration.Type,
                        SingletonSeparatedList(variables[i])));

                if (i == 0)
                    newDeclaration = newDeclaration.WithLeadingTrivia(declaration.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newDeclaration = newDeclaration.WithTrailingTrivia(declaration.GetTrailingTrivia());

                yield return newDeclaration.WithFormatterAnnotation();
            }
        }

        private static IEnumerable<EventFieldDeclarationSyntax> SplitEventFieldDeclaration(EventFieldDeclarationSyntax fieldDeclaration)
        {
            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = fieldDeclaration.Declaration.Variables;

            EventFieldDeclarationSyntax fieldDeclaration2 = fieldDeclaration.WithoutTrivia();

            for (int i = 0; i < variables.Count; i++)
            {
                EventFieldDeclarationSyntax newDeclaration = EventFieldDeclaration(
                    fieldDeclaration2.AttributeLists,
                    fieldDeclaration2.Modifiers,
                    VariableDeclaration(
                        fieldDeclaration2.Declaration.Type,
                        SingletonSeparatedList(variables[i])));

                if (i == 0)
                    newDeclaration = newDeclaration.WithLeadingTrivia(fieldDeclaration.GetLeadingTrivia());

                if (i == variables.Count - 1)
                    newDeclaration = newDeclaration.WithTrailingTrivia(fieldDeclaration.GetTrailingTrivia());

                yield return newDeclaration.WithFormatterAnnotation();
            }
        }
    }
}
