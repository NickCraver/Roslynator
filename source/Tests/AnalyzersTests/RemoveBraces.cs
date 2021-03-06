﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveBraces
    {
        private static void Foo(object x, object y)
        {
            bool f = false;
            var items = new List<object>();

            if (f)
            {
                Foo(x, y);
            }

            if (f)
            {
                Foo(x, y);
            }
            else
            {
                Foo(x, y);
            }

            foreach (object item in items)
            {
                Foo(x, y);
            }

            foreach ((string, string) item in Tuple.Values)
            {
                Foo(x, y);
            }

            for (int i = 0; i < items.Count; i++)
            {
                Foo(x, y);
            }

            using ((IDisposable)null)
            {
                Foo(x, y);
            }

            while (f)
            {
                Foo(x, y);
            }

            do
            {
                Foo(x, y);
            }
            while (f);

            lock (null)
            {
                Foo(x, y);
            }

            unsafe
            {
                fixed ()
                {
                    Foo(x, y);
                }
            }
        }
    }
}
