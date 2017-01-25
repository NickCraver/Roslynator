// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DocumentRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context)
        {
            context.RegisterRefactoring(
                "Remove all comments",
                c => Remover.RemoveCommentsAsync(context.Document, CommentRemoveOptions.All, c));

            context.RegisterRefactoring(
                "Remove all comments (except documentation comments)",
                c => Remover.RemoveCommentsAsync(context.Document, CommentRemoveOptions.AllExceptDocumentation, c));

            context.RegisterRefactoring(
                "Remove all documentation comments",
                c => Remover.RemoveCommentsAsync(context.Document, CommentRemoveOptions.Documentation, c));

            context.RegisterRefactoring(
                "Remove all directives",
                c => Remover.RemoveDirectivesAsync(context.Document, DirectiveRemoveOptions.All, c));

            context.RegisterRefactoring(
                "Remove all directives (except region directives)",
                c => Remover.RemoveDirectivesAsync(context.Document, DirectiveRemoveOptions.AllExceptRegion, c));

            context.RegisterRefactoring(
                "Remove all region directives",
                c => Remover.RemoveDirectivesAsync(context.Document, DirectiveRemoveOptions.Region, c));

            context.RegisterRefactoring(
                "Format document",
                c =>
                {
                    var tcs = new TaskCompletionSource<Document>();

                    Document newDocument = context.Document.WithSyntaxRoot(context.Root.WithFormatterAnnotation());

                    tcs.SetResult(newDocument);

                    return tcs.Task;
                });
        }
    }
}