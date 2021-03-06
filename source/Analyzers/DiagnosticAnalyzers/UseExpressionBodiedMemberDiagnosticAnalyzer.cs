﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Refactorings;
using Roslynator.Extensions;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseExpressionBodiedMemberDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseExpressionBodiedMember,
                    DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut);
            }
        }

        private static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut; }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.GetAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            if (method.ExpressionBody == null)
            {
                BlockSyntax body = method.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (OperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private void AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConstructorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
            {
                BlockSyntax body = declaration.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private void AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (DestructorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
            {
                BlockSyntax body = declaration.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private void AnalyzeAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessor = (AccessorDeclarationSyntax)context.Node;

            if (accessor.ExpressionBody == null
                && !accessor.AttributeLists.Any())
            {
                BlockSyntax body = accessor.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetExpression(body);

                if (expression != null
                    && expression.IsSingleLine())
                {
                    var accessorList = accessor.Parent as AccessorListSyntax;

                    if (accessorList != null)
                    {
                        SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

                        if (accessors.Count == 1
                            && accessors.First().IsKind(SyntaxKind.GetAccessorDeclaration))
                        {
                            if (accessorList.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                            {
                                ReportDiagnostic(context, accessorList, expression);
                                context.ReportToken(FadeOutDescriptor, accessor.Keyword);
                                context.ReportBraces(FadeOutDescriptor, body);
                            }

                            return;
                        }
                    }

                    if (accessor.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                        ReportDiagnostic(context, body, expression);
                }
            }
        }

        private static void AnalyzeBody(SyntaxNodeAnalysisContext context, BlockSyntax body)
        {
            ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetReturnExpression(body);

            if (expression != null)
                AnalyzeExpression(context, body, expression);
        }

        private static void AnalyzeExpression(SyntaxNodeAnalysisContext context, BlockSyntax block, ExpressionSyntax expression)
        {
            if (block.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && expression.IsSingleLine())
            {
                ReportDiagnostic(context, block, expression);
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, BlockSyntax block, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseExpressionBodiedMember,
                block);

            SyntaxNode parent = expression.Parent;

            if (parent.IsKind(SyntaxKind.ReturnStatement))
                context.ReportToken(FadeOutDescriptor, ((ReturnStatementSyntax)parent).ReturnKeyword);

            context.ReportBraces(FadeOutDescriptor, block);
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseExpressionBodiedMember,
                accessorList);

            SyntaxNode parent = expression.Parent;

            if (parent.IsKind(SyntaxKind.ReturnStatement))
                context.ReportToken(FadeOutDescriptor, ((ReturnStatementSyntax)parent).ReturnKeyword);

            context.ReportBraces(FadeOutDescriptor, accessorList);
        }
    }
}
