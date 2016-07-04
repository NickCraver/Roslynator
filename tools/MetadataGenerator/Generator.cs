﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Pihrtsoft.CodeAnalysis.CSharp;
using Pihrtsoft.CodeAnalysis.Metadata;

namespace MetadataGenerator
{
    internal class Generator
    {
        public Collection<AnalyzerInfo> Analyzers { get; } = new Collection<AnalyzerInfo>();
        public Collection<RefactoringInfo> Refactorings { get; } = new Collection<RefactoringInfo>();

        public string CreateRefactoringsExtensionDescription()
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    xw.WriteStartElement("html");
                    WriteRefactoringsExtensionDescription(xw);
                }

                return File.ReadAllText(@"..\text\CSharpRefactoringsDescription.txt", Encoding.UTF8)
                    + RemoveRootHtmlElement(sw.ToString());
            }
        }

        public string CreateAnalyzersExtensionDescription()
        {
            using (var sw = new StringWriter())
            {
                using (XmlWriter xw = XmlWriter.Create(sw, CreateXmlWriterSettings()))
                {
                    xw.WriteStartElement("html");
                    WriteAnalyzersExtensionDescription(xw);
                    WriteRefactoringsExtensionDescription(xw);
                }

                return File.ReadAllText(@"..\text\CSharpAnalyzersAndRefactoringsDescription.txt", Encoding.UTF8)
                    + RemoveRootHtmlElement(sw.ToString());
            }
        }

        private void WriteRefactoringsExtensionDescription(XmlWriter xw)
        {
            xw.WriteElementString("h3", "List of Refactoring");
            xw.WriteStartElement("ul");

            foreach (RefactoringInfo info in Refactorings
                .OrderBy(f => f.Title, StringComparer.InvariantCulture))
            {
                string href = "http://github.com/JosefPihrt/Pihrtsoft.CodeAnalysis/blob/master/Refactorings.md#" + info.GetGitHubHref();
                xw.WriteStartElement("li");
                xw.WriteStartElement("a");
                xw.WriteAttributeString("href", href);
                xw.WriteString(info.Title);
                xw.WriteEndElement();
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
        }

        private void WriteAnalyzersExtensionDescription(XmlWriter xw)
        {
            xw.WriteElementString("h3", "List of Analyzers");

            xw.WriteStartElement("ul");
            foreach (AnalyzerInfo analyzer in Analyzers
                .OrderBy(f => f.Id, StringComparer.InvariantCulture))
            {
                xw.WriteElementString("li", $"{analyzer.Id} - {analyzer.Title}");
            }

            xw.WriteEndElement();
        }

        public string CreateGitHubMarkDown()
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine(File.ReadAllText(@"..\text\ReadMe.txt", Encoding.UTF8));
                sw.WriteLine("### List of Analyzers");
                sw.WriteLine();
                foreach (AnalyzerInfo info in Analyzers
                    .OrderBy(f => f.Id, StringComparer.InvariantCulture))
                {
                    sw.WriteLine("* " + info.Id + " - " + info.Title.TrimEnd('.'));
                }

                sw.WriteLine();
                sw.WriteLine("### List of Refactorings");
                sw.WriteLine();

                foreach (RefactoringInfo info in Refactorings
                    .OrderBy(f => f.Title, StringComparer.InvariantCulture))
                {
                    sw.WriteLine("* [" + info.Title.TrimEnd('.') + "](Refactorings.md#" + info.GetGitHubHref() + ")");
                }

                return sw.ToString();
            }
        }

        public string CreateRefactoringsMarkDown()
        {
            using (var sw = new StringWriter())
            {
                sw.WriteLine("## " + "C# Refactorings");

                foreach (RefactoringInfo info in Refactorings
                    .OrderBy(f => f.Title, StringComparer.InvariantCulture))
                {
                    sw.WriteLine("");
                    sw.WriteLine("#### " + info.Title);
                    sw.WriteLine("");
                    sw.WriteLine("* **Syntax**: " + string.Join(", ", info.Syntaxes.Select(f => f.Name)));

                    if (!string.IsNullOrEmpty(info.Scope))
                        sw.WriteLine("* **Scope**: " + info.Scope);

                    sw.WriteLine("");

                    if (info.Images.Count > 0)
                    {
                        foreach (ImageInfo image in info.Images)
                            sw.WriteLine(CreateImageMarkDown(info, image.Name));
                    }
                    else
                    {
                        sw.WriteLine(CreateImageMarkDown(info, info.Identifier));
                    }
                }

                return sw.ToString();
            }
        }

        private static string CreateImageMarkDown(RefactoringInfo info, string fileName)
        {
            string url = "/images/refactorings/" + fileName + ".png";

            CheckImageExist(@"..\" + url.Replace("/", @"\"));

            return "![" + info.Title + "](" + url + ")";
        }

        public string CreateAnalyzersXml()
        {
            FieldInfo[] fieldInfos = typeof(DiagnosticDescriptors).GetFields(BindingFlags.Public | BindingFlags.Static);

            var doc = new XDocument();

            var root = new XElement("Analyzers");

            foreach (FieldInfo fieldInfo in fieldInfos.OrderBy(f => ((DiagnosticDescriptor)f.GetValue(null)).Id))
            {
                if (fieldInfo.Name.EndsWith("FadeOut"))
                    continue;

                var descriptor = (DiagnosticDescriptor)fieldInfo.GetValue(null);

                root.Add(new XElement(
                    "Analyzer",
                    new XAttribute("Identifier", fieldInfo.Name),
                    new XAttribute("ExtensionVersion", "0.1.0"),
                    new XAttribute("NuGetVersion", "0.1.0"),
                    new XElement("Id", descriptor.Id),
                    new XElement("Title", descriptor.Title),
                    new XElement("Category", descriptor.Category),
                    new XElement("Severity", descriptor.DefaultSeverity),
                    new XElement("IsEnabledByDefault", descriptor.IsEnabledByDefault),
                    new XElement("SupportsFadeOut", descriptor.CustomTags.Contains(WellKnownDiagnosticTags.Unnecessary)),
                    new XElement("SupportsFadeOutAnalyzer", fieldInfos.Any(f => f.Name == fieldInfo.Name + "FadeOut"))
                ));
            }

            doc.Add(root);

            using (var sw = new Utf8StringWriter())
            {
                doc.Save(sw);

                return sw.ToString();
            }
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        private static XmlWriterSettings CreateXmlWriterSettings()
        {
            var settings = new XmlWriterSettings();

            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = "\r\n";
            settings.IndentChars = "    ";
            settings.Indent = true;

            return settings;
        }

        private static string RemoveRootHtmlElement(string value)
        {
            return Regex.Replace(value, @"^\s*<html>|</html>\s*$", "");
        }

        private static void CheckImageExist(string path)
        {
            if (!File.Exists(path))
                throw new IOException($"file not found '{path}'");
        }
    }
}