﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class CSharpFactory
    {
        internal static SyntaxTokenList TokenList(params SyntaxKind[] tokenKinds)
        {
            return SyntaxFactory.TokenList(
                tokenKinds.Select(f => Token(f)));
        }

        public static AssignmentExpressionSyntax SimpleAssignmentExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            return AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, right);
        }

        public static AttributeListSyntax AttributeList(AttributeSyntax attribute)
        {
            return SyntaxFactory.AttributeList(SingletonSeparatedList(attribute));
        }

        public static FieldDeclarationSyntax FieldDeclaration(TypeSyntax type, VariableDeclaratorSyntax variableDeclarator)
        {
            return SyntaxFactory.FieldDeclaration(VariableDeclaration(type, variableDeclarator));
        }

        public static FieldDeclarationSyntax FieldDeclaration(TypeSyntax type, string identifier, ExpressionSyntax initializerValue = null)
        {
            return SyntaxFactory.FieldDeclaration(
                VariableDeclaration(
                    type,
                    VariableDeclarator(
                        Identifier(identifier),
                        null,
                        EqualsValueClause(
                            initializerValue))));
        }

        public static ArgumentListSyntax ArgumentList(params ArgumentSyntax[] arguments)
        {
            return SyntaxFactory.ArgumentList(SeparatedList(arguments));
        }

        public static ArgumentSyntax Argument(string identifierName)
        {
            return SyntaxFactory.Argument(IdentifierName(identifierName));
        }

        public static AttributeSyntax Attribute(string identifierName)
        {
            return SyntaxFactory.Attribute(IdentifierName(identifierName));
        }

        public static UsingDirectiveSyntax UsingDirective(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            if (names.Length == 0)
                throw new ArgumentException($"'{names}' cannot be empty.", nameof(names));

            if (names.Length == 1)
                return SyntaxFactory.UsingDirective(IdentifierName(names[0]));

            NameSyntax name = QualifiedName(IdentifierName(names[0]), IdentifierName(names[1]));

            for (int i = 2; i < names.Length; i++)
                name = QualifiedName(name, IdentifierName(names[i]));

            return SyntaxFactory.UsingDirective(name);
        }

        public static NamespaceDeclarationSyntax NamespaceDeclaration(string identifierName)
        {
            return SyntaxFactory.NamespaceDeclaration(IdentifierName(identifierName));
        }

        public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
        }

        public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, SyntaxToken operatorToken, SimpleNameSyntax name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, operatorToken, name);
        }

        public static MemberAccessExpressionSyntax SimpleMemberAccessExpression(string identifierName, string name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(identifierName), IdentifierName(name));
        }

        public static InvocationExpressionSyntax InvocationExpression(string name)
        {
            return SyntaxFactory.InvocationExpression(IdentifierName(name));
        }

        public static InvocationExpressionSyntax InvocationExpression(string name, ArgumentSyntax argument)
        {
            return SyntaxFactory.InvocationExpression(
                IdentifierName(name),
                SyntaxFactory.ArgumentList(SingletonSeparatedList(argument)));
        }

        public static InvocationExpressionSyntax InvocationExpression(string name, params ArgumentSyntax[] arguments)
        {
            return SyntaxFactory.InvocationExpression(
                IdentifierName(name),
                SyntaxFactory.ArgumentList(SeparatedList(arguments)));
        }

        public static AccessorDeclarationSyntax Getter()
        {
            return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax AutoGetter()
        {
            return Getter().WithSemicolonToken();
        }

        public static AccessorDeclarationSyntax Setter()
        {
            return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax AutoSetter()
        {
            return Setter().WithSemicolonToken();
        }

        public static VariableDeclarationSyntax VariableDeclaration(TypeSyntax type, VariableDeclaratorSyntax variableDeclarator)
        {
            return SyntaxFactory.VariableDeclaration(type, SingletonSeparatedList(variableDeclarator));
        }

        public static SyntaxToken SemicolonToken()
        {
            return Token(SyntaxKind.SemicolonToken);
        }

        public static SyntaxToken CommaToken()
        {
            return Token(SyntaxKind.CommaToken);
        }

        private static SyntaxToken Token(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.Token(syntaxKind);
        }

        public static PredefinedTypeSyntax StringType()
        {
            return PredefinedType((SyntaxKind.StringKeyword));
        }

        public static PredefinedTypeSyntax IntType()
        {
            return PredefinedType((SyntaxKind.IntKeyword));
        }

        public static PredefinedTypeSyntax BoolType()
        {
            return PredefinedType((SyntaxKind.BoolKeyword));
        }

        public static PredefinedTypeSyntax VoidType()
        {
            return PredefinedType((SyntaxKind.VoidKeyword));
        }

        private static PredefinedTypeSyntax PredefinedType(SyntaxKind syntaxKind)
        {
            return SyntaxFactory.PredefinedType(Token(syntaxKind));
        }

        public static LiteralExpressionSyntax StringLiteralExpression(string value)
        {
            return LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax NumericLiteralExpression(int value)
        {
            return LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                Literal(value));
        }

        public static LiteralExpressionSyntax TrueLiteralExpression()
        {
            return LiteralExpression(SyntaxKind.TrueLiteralExpression);
        }

        public static LiteralExpressionSyntax FalseLiteralExpression()
        {
            return LiteralExpression(SyntaxKind.FalseLiteralExpression);
        }

        public static SyntaxTrivia IndentTrivia { get; } = Whitespace("    ");

        public static SyntaxTrivia EmptyTrivia { get; } = SyntaxTrivia(SyntaxKind.WhitespaceTrivia, string.Empty);

        public static SyntaxTrivia NewLine { get; } = CreateNewLine();

        private static SyntaxTrivia CreateNewLine()
        {
            switch (Environment.NewLine)
            {
                case "\r":
                    return CarriageReturn;
                case "\n":
                    return LineFeed;
                default:
                    return CarriageReturnLineFeed;
            }
        }

        public static InvocationExpressionSyntax NameOf(string identifier)
        {
            return InvocationExpression(
                "nameof",
                Argument(identifier));
        }
    }
}