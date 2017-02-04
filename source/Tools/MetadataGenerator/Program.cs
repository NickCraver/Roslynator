﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Roslynator.Metadata;

namespace MetadataGenerator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
#if DEBUG
                args = new string[] { @"..\..\..\.." };
#else
                args = new string[] { Environment.CurrentDirectory };
#endif
            }

            string dirPath = args[0];

            SortRefactoringsAndAddMissingIds(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"));

            var generator = new Generator();

            foreach (RefactoringInfo refactoring in RefactoringInfo
                .LoadFromFile(Path.Combine(dirPath, @"Refactorings\Refactorings.xml"))
                .OrderBy(f => f.Identifier, StringComparer.InvariantCulture))
            {
                generator.Refactorings.Add(refactoring);
            }

            Console.WriteLine($"number of refactorings: {generator.Refactorings.Count}");

            foreach (AnalyzerInfo analyzer in AnalyzerInfo
                .LoadFromFile(Path.Combine(dirPath, @"Analyzers\Analyzers.xml"))
                .OrderBy(f => f.Id, StringComparer.InvariantCulture))
            {
                generator.Analyzers.Add(analyzer);
            }

            Console.WriteLine($"number of analyzers: {generator.Analyzers.Count}");

            var writer = new CodeFileWriter();

            writer.SaveCode(
                Path.Combine(dirPath, @"Analyzers\Analyzers.xml"),
                generator.CreateAnalyzersXml());

            writer.SaveCode(
                Path.Combine(dirPath, @"VisualStudio\description.txt"),
                generator.CreateAnalyzersExtensionDescription());

            writer.SaveCode(
                Path.Combine(dirPath, @"VisualStudio.Refactorings\description.txt"),
                generator.CreateRefactoringsExtensionDescription());

            writer.SaveCode(
                 Path.Combine(Path.GetDirectoryName(dirPath), @"README.md"),
                generator.CreateReadMeMarkDown());

            foreach (string imagePath in generator.FindMissingImages(Path.Combine(Path.GetDirectoryName(dirPath), @"images\refactorings")))
                Console.WriteLine($"missing image: {imagePath}");

            writer.SaveCode(
                Path.Combine(dirPath, @"Refactorings\Refactorings.md"),
                generator.CreateRefactoringsMarkDown());

            writer.SaveCode(
                Path.Combine(dirPath, @"Refactorings\README.md"),
                generator.CreateRefactoringsReadMe());

            writer.SaveCode(
                Path.Combine(dirPath, @"Refactorings\RoslynatorConfig.xml"),
                generator.CreateDefaultConfigFile());

            writer.SaveCode(
                Path.Combine(dirPath, @"Analyzers\README.md"),
                generator.CreateAnalyzersReadMe());

            writer.SaveCode(
                Path.Combine(dirPath, @"Analyzers\AnalyzersByCategory.md"),
                generator.CreateAnalyzersByCategoryMarkDown());

#if DEBUG
            Console.WriteLine("DONE");
            Console.ReadKey();
#endif
        }

        public static void SortRefactoringsAndAddMissingIds(string filePath)
        {
            XDocument doc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            XElement root = doc.Root;

            IEnumerable<XElement> newElements = root
                .Elements()
                .OrderBy(f => f.Attribute("Identifier").Value);

            if (newElements.Any(f => f.Attribute("Id") == null))
            {
                int maxValue = newElements.Where(f => f.Attribute("Id") != null)
                    .Select(f => int.Parse(f.Attribute("Id").Value.Substring(2)))
                    .DefaultIfEmpty()
                    .Max();

                int idNumber = maxValue + 1;

                newElements = newElements.Select(f =>
                {
                    if (f.Attribute("Id") != null)
                    {
                        return f;
                    }
                    else
                    {
                        string id = $"RR{idNumber.ToString().PadLeft(4, '0')}";
                        f.ReplaceAttributes(new XAttribute("Id", id), f.Attributes());
                        idNumber++;
                        return f;
                    }
                });
            }

            root.ReplaceAll(newElements);

            doc.Save(filePath);
        }
    }
}
