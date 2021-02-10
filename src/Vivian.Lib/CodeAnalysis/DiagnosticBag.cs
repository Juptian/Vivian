﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Vivian.CodeAnalysis.Symbols;
using Vivian.CodeAnalysis.Syntax;
using Vivian.CodeAnalysis.Text;

namespace Vivian.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new();
        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }
        
        public void AddRange(IEnumerable<Diagnostic> diagnostics)
        {
            _diagnostics.AddRange(diagnostics);
        }

        private void Report(TextLocation location, string message)
        {
            var diagnostic = new Diagnostic(location, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type)
        {
            var message = $"The number '{text}' isn't valid '{type}'.";
            Report(location, message);
        }

        public void ReportBadCharacter(TextLocation location, char character)
        {
            var message = $"Bad character input: '{character}'.";
            Report(location, message);
        }
        
        public void ReportUnexpectedToken(TextLocation location, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            var message = $"Unexpected token: '{actualKind}', expected: '{expectedKind}'.";
            Report(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string operatorText, TypeSymbol operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'.";
            Report(location, message);
        }

        public void ReportUndefinedBinaryOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for type '{leftType}' and '{rightType}'.";
            Report(location, message);
        }
        
        public void ReportCannotConvert(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert type '{fromType}' to '{toType}'.";
            Report(location, message);
        }

        public void ReportCannotAssign(TextLocation location, string name)
        {
            var message = $"Variable '{name}' is read-only and cannot be assigned to.";
            Report(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            const string message = "Unterminated string literal.";
            Report(location, message);
        }

        public void ReportUndefinedFunction(TextLocation location, string name)
        {
            var message = $"Function '{name}' does not exist in the current context.";
            Report(location, message);
        }

        public void ReportWrongArgumentCount(TextLocation location, string name, int expectedCount, int actualCount)
        {
            var message = $"Function '{name}' requires '{expectedCount}' arguments but was given '{actualCount}'.";
            Report(location, message);
        }

        public void ReportExpressionMustHaveValue(TextLocation location)
        {
            const string message = "Expression must have a value.";
            Report(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            var message = $"'{name}' is already declared.";
            Report(location, message);
        }

        public void ReportUndefinedType(TextLocation location, string name)
        {
            var message = $"Type '{name}' does not exist.";
            Report(location, message);
        }

        public void ReportCannotConvertImplicitly(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert type '{fromType}' to '{toType}'. An explicit conversion exists (are you missing a cast?)";
            Report(location, message);
        }

        public void ReportParameterAlreadyDeclared(TextLocation location, string parameterName)
        {
            var message = $"A parameter with the name '{parameterName}' already exists.";
            Report(location, message);             
        }

        public void ReportInvalidBreakOrContinue(TextLocation location, string text)
        {
            var message = $"The keyword {text} can only exist inside of a loop.";
            Report(location, message);
        }
        
        public void ReportInvalidReturnExpression(TextLocation location, string functionName)
        {
            var message = $"Since the function '{functionName}' does not return a value the 'return' keyword cannot be followed by an expression.";
            Report(location, message);        
        }
        
        public void ReportMissingReturnExpression(TextLocation location, TypeSymbol returnType)
        {
            var message = $"An expression of type '{returnType}' expected.";
            Report(location, message);        
        }

        public void ReportAllPathsMustReturn(TextLocation location)
        {
            const string message = "Not all code paths return a value.";
            Report(location, message);   
        }

        public void ReportUndefinedVariable(TextLocation location, string name)
        {
            var message = $"Variable '{name}' does not exist in the current context.";
            Report(location, message);
        }

        public void ReportNotAVariable(TextLocation location, string name)
        {
            var message = $"'{name}' is not a variable.";
            Report(location, message);
        }

        public void ReportNotAFunction(TextLocation location, string name)
        {
            var message = $"'{name}' is not a function.";
            Report(location, message);
        }

        public void ReportInvalidExpressionStatement(TextLocation location)
        {
            const string message = "Only assignment and call expressions can be used as a statement";
            Report(location, message);        
        }
        
        public void ReportCannotMixMainAndGlobalStatements(TextLocation location)
        {
            const string message = "Cannot declare main function when global statements are used.";
            Report(location, message);
        }

        public void ReportMainMustHaveCorrectSignature(TextLocation location)
        {
            const string message = "Main must not take arguments and cannot return anything.";
            Report(location, message);
        }

        public void ReportOnlyOneFileCanHaveGlobalStatements(TextLocation location)
        {
            const string message = "At most one file can contain global statements.";
            Report(location, message);
        }
        
        public void ReportInvalidReturnWithValueInGlobalStatements(TextLocation location)
        {
            const string message = "The 'return' keyword cannot be followed by an expression in global statements.";
            Report(location, message);
        }

        public void ReportInvalidReference(string path)
        {
            var message = $"The reference is not a valid .NET assembly: '{path}'";
            Report(default, message);
        }

        public void ReportRequiredTypeNotFound(string vivianName, string metadataName)
        {
            var message = vivianName == null
                ? $"The required type '{metadataName}' cannot be resolved among the given references."
                : $"The required type '{vivianName}' ('{metadataName}') cannot be resolved among the given references.";
            Report(default, message);
        }

        public void ReportRequiredTypeAmbiguous(string vivianName, string metadataName, TypeDefinition[] foundTypes)
        {
            var assemblyNames = foundTypes.Select(t => t.Module.Assembly.Name.Name);
            var assemblyNameList = string.Join(", ", assemblyNames);
            
            var message = vivianName == null
                ? $"The required type '{metadataName}' was found in multiple references: {assemblyNameList}"
                : $"The required type '{vivianName}' ('{metadataName}') was found in multiple references: {assemblyNameList}.";
            Report(default, message);
        }

        public void ReportRequiredMethodNotFound(string typeName, string methodName, string[] parameterTypeNames)
        {
            var parameterTypeNameList = string.Join(", ", parameterTypeNames);

            var message = $"The required method '{typeName}.{methodName}'({parameterTypeNameList}) cannot be resolved among the given references.";
            Report(default, message);
        }

        public void ReportUnterminatedMultiLineComment(TextLocation location)
        {
            const string message = "Unterminated multi-line comment.";
            Report(default, message);
        }
    }
}