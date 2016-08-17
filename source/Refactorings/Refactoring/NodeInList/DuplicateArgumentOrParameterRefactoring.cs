﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.NodeInList
{
    internal abstract class DuplicateArgumentOrParameterRefactoring<TSyntax, TListSyntax> : NodeInListRefactoring<TSyntax, TListSyntax>
        where TSyntax : SyntaxNode
        where TListSyntax : SyntaxNode
    {
        public DuplicateArgumentOrParameterRefactoring(TListSyntax listSyntax, SeparatedSyntaxList<TSyntax> list)
            : base(listSyntax, list)
        {
        }

        protected abstract string GetTitle(params string[] args);

        public void ComputeRefactoring(RefactoringContext context, TListSyntax listSyntax)
        {
            int index = FindNode(context.Span);

            if (index > 0
                && !List[index - 1].IsMissing)
            {
                context.RegisterRefactoring(
                    GetTitle(),
                    cancellationToken => RefactorAsync(context.Document, index, cancellationToken));
            }
        }

        protected async Task<Document> RefactorAsync(
            Document document,
            int nodeIndex,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var info = new RewriterInfo<TSyntax>(
                List[nodeIndex],
                List[nodeIndex - 1],
                GetTokenBefore(nodeIndex),
                GetTokenAfter(nodeIndex));

            SyntaxNode newRoot = root.ReplaceNode(ListSyntax, Rewrite(info));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}