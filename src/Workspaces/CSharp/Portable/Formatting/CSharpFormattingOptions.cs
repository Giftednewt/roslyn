﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis.Options;

namespace Microsoft.CodeAnalysis.CSharp.Formatting
{
    public static partial class CSharpFormattingOptions
    {
        public static Option<bool> SpacingAfterMethodDeclarationName { get; } = CSharpFormattingOptions2.SpacingAfterMethodDeclarationName;

        public static Option<bool> SpaceWithinMethodDeclarationParenthesis { get; } = CSharpFormattingOptions2.SpaceWithinMethodDeclarationParenthesis;

        public static Option<bool> SpaceBetweenEmptyMethodDeclarationParentheses { get; } = CSharpFormattingOptions2.SpaceBetweenEmptyMethodDeclarationParentheses;

        public static Option<bool> SpaceAfterMethodCallName { get; } = CSharpFormattingOptions2.SpaceAfterMethodCallName;

        public static Option<bool> SpaceWithinMethodCallParentheses { get; } = CSharpFormattingOptions2.SpaceWithinMethodCallParentheses;

        public static Option<bool> SpaceBetweenEmptyMethodCallParentheses { get; } = CSharpFormattingOptions2.SpaceBetweenEmptyMethodCallParentheses;

        public static Option<bool> SpaceAfterControlFlowStatementKeyword { get; } = CSharpFormattingOptions2.SpaceAfterControlFlowStatementKeyword;

        public static Option<bool> SpaceWithinExpressionParentheses { get; } = CSharpFormattingOptions2.SpaceWithinExpressionParentheses;

        public static Option<bool> SpaceWithinCastParentheses { get; } = CSharpFormattingOptions2.SpaceWithinCastParentheses;

        public static Option<bool> SpaceWithinOtherParentheses { get; } = CSharpFormattingOptions2.SpaceWithinOtherParentheses;

        public static Option<bool> SpaceAfterCast { get; } = CSharpFormattingOptions2.SpaceAfterCast;

        public static Option<bool> SpacesIgnoreAroundVariableDeclaration { get; } = CSharpFormattingOptions2.SpacesIgnoreAroundVariableDeclaration;

        public static Option<bool> SpaceBeforeOpenSquareBracket { get; } = CSharpFormattingOptions2.SpaceBeforeOpenSquareBracket;

        public static Option<bool> SpaceBetweenEmptySquareBrackets { get; } = CSharpFormattingOptions2.SpaceBetweenEmptySquareBrackets;

        public static Option<bool> SpaceWithinSquareBrackets { get; } = CSharpFormattingOptions2.SpaceWithinSquareBrackets;

        public static Option<bool> SpaceAfterColonInBaseTypeDeclaration { get; } = CSharpFormattingOptions2.SpaceAfterColonInBaseTypeDeclaration;

        public static Option<bool> SpaceAfterComma { get; } = CSharpFormattingOptions2.SpaceAfterComma;

        public static Option<bool> SpaceAfterDot { get; } = CSharpFormattingOptions2.SpaceAfterDot;

        public static Option<bool> SpaceAfterSemicolonsInForStatement { get; } = CSharpFormattingOptions2.SpaceAfterSemicolonsInForStatement;

        public static Option<bool> SpaceBeforeColonInBaseTypeDeclaration { get; } = CSharpFormattingOptions2.SpaceBeforeColonInBaseTypeDeclaration;

        public static Option<bool> SpaceBeforeComma { get; } = CSharpFormattingOptions2.SpaceBeforeComma;

        public static Option<bool> SpaceBeforeDot { get; } = CSharpFormattingOptions2.SpaceBeforeDot;

        public static Option<bool> SpaceBeforeSemicolonsInForStatement { get; } = CSharpFormattingOptions2.SpaceBeforeSemicolonsInForStatement;

        public static Option<BinaryOperatorSpacingOptions> SpacingAroundBinaryOperator { get; } = CSharpFormattingOptions2.SpacingAroundBinaryOperator;

        public static Option<bool> IndentBraces { get; } = CSharpFormattingOptions2.IndentBraces;

        public static Option<bool> IndentBlock { get; } = CSharpFormattingOptions2.IndentBlock;

        public static Option<bool> IndentSwitchSection { get; } = CSharpFormattingOptions2.IndentSwitchSection;

        public static Option<bool> IndentSwitchCaseSection { get; } = CSharpFormattingOptions2.IndentSwitchCaseSection;

        public static Option<bool> IndentSwitchCaseSectionWhenBlock { get; } = CSharpFormattingOptions2.IndentSwitchCaseSectionWhenBlock;

        public static Option<LabelPositionOptions> LabelPositioning { get; } = CSharpFormattingOptions2.LabelPositioning;

        public static Option<bool> WrappingPreserveSingleLine { get; } = CSharpFormattingOptions2.WrappingPreserveSingleLine;

        public static Option<bool> WrappingKeepStatementsOnSingleLine { get; } = CSharpFormattingOptions2.WrappingKeepStatementsOnSingleLine;

        public static Option<bool> NewLinesForBracesInTypes { get; } = CSharpFormattingOptions2.NewLinesForBracesInTypes;

        public static Option<bool> NewLinesForBracesInMethods { get; } = CSharpFormattingOptions2.NewLinesForBracesInMethods;

        public static Option<bool> NewLinesForBracesInProperties { get; } = CSharpFormattingOptions2.NewLinesForBracesInProperties;

        public static Option<bool> NewLinesForBracesInAccessors { get; } = CSharpFormattingOptions2.NewLinesForBracesInAccessors;

        public static Option<bool> NewLinesForBracesInAnonymousMethods { get; } = CSharpFormattingOptions2.NewLinesForBracesInAnonymousMethods;

        public static Option<bool> NewLinesForBracesInControlBlocks { get; } = CSharpFormattingOptions2.NewLinesForBracesInControlBlocks;

        public static Option<bool> NewLinesForBracesInAnonymousTypes { get; } = CSharpFormattingOptions2.NewLinesForBracesInAnonymousTypes;

        public static Option<bool> NewLinesForBracesInObjectCollectionArrayInitializers { get; } = CSharpFormattingOptions2.NewLinesForBracesInObjectCollectionArrayInitializers;

        public static Option<bool> NewLinesForBracesInLambdaExpressionBody { get; } = CSharpFormattingOptions2.NewLinesForBracesInLambdaExpressionBody;

        public static Option<bool> NewLineForElse { get; } = CSharpFormattingOptions2.NewLineForElse;

        public static Option<bool> NewLineForCatch { get; } = CSharpFormattingOptions2.NewLineForCatch;

        public static Option<bool> NewLineForFinally { get; } = CSharpFormattingOptions2.NewLineForFinally;

        public static Option<bool> NewLineForMembersInObjectInit { get; } = CSharpFormattingOptions2.NewLineForMembersInObjectInit;

        public static Option<bool> NewLineForMembersInAnonymousTypes { get; } = CSharpFormattingOptions2.NewLineForMembersInAnonymousTypes;

        public static Option<bool> NewLineForClausesInQuery { get; } = CSharpFormattingOptions2.NewLineForClausesInQuery;
    }
}
