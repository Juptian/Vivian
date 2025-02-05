﻿using System.Collections.Immutable;
using Vivian.CodeAnalysis.Symbols;

namespace Vivian.CodeAnalysis.Binding
{
    internal sealed class BoundProgram
    {
        public BoundProgram(BoundProgram? previous,
                            ImmutableArray<Diagnostic> diagnostics,
                            FunctionSymbol? mainFunction,
                            FunctionSymbol? scriptFunction,
                            ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functions,
                            ImmutableDictionary<ClassSymbol, BoundBlockStatement> structs)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
            Structs = structs;
        }

        public BoundProgram? Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public FunctionSymbol? MainFunction { get; }
        public FunctionSymbol? ScriptFunction { get; }
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> Functions { get; }
        public ImmutableDictionary<ClassSymbol, BoundBlockStatement> Structs { get; }
    }
}