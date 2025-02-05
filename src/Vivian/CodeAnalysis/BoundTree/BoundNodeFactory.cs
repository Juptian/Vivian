﻿using System.Collections.Immutable;
using System.Diagnostics;
using Vivian.CodeAnalysis.Symbols;
using Vivian.CodeAnalysis.Syntax;

namespace Vivian.CodeAnalysis.Binding
{
     internal static class BoundNodeFactory
        {
            public static BoundBlockStatement Block(SyntaxNode syntax, params BoundStatement[] statements) 
                => new(syntax, ImmutableArray.Create(statements));
            
            public static BoundVariableDeclaration VariableDeclaration(SyntaxNode syntax, VariableSymbol symbol, BoundExpression initializer)
                => new (syntax, symbol, initializer);
            
            public static BoundVariableDeclaration VariableDeclaration(SyntaxNode syntax, string name, BoundExpression initializer)
                => VariableDeclarationInternal(syntax, name, initializer, isReadOnly: false);
    
            public static BoundVariableDeclaration ConstantDeclaration(SyntaxNode syntax, string name, BoundExpression initializer)
                => VariableDeclarationInternal(syntax, name, initializer, isReadOnly: true);
    
            private static BoundVariableDeclaration VariableDeclarationInternal(SyntaxNode syntax, string name, BoundExpression initializer, bool isReadOnly)
            {
                var local = new LocalVariableSymbol(name, isReadOnly, initializer.Type, initializer.ConstantValue);
                return new BoundVariableDeclaration(syntax, local, initializer);
            }
    
            public static BoundWhileStatement While(SyntaxNode syntax, BoundExpression condition, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel) 
                => new(syntax, condition, body, breakLabel, continueLabel);
            
            public static BoundGotoStatement Goto(SyntaxNode syntax, BoundLabel label)
                => new(syntax, label);
            
    
            public static BoundConditionalGotoStatement GotoTrue(SyntaxNode syntax, BoundLabel label, BoundExpression condition)
                => new(syntax, label, condition, jumpIfTrue: true);
    
            public static BoundConditionalGotoStatement GotoFalse(SyntaxNode syntax, BoundLabel label, BoundExpression condition)
                => new(syntax, label, condition, jumpIfTrue: false);
    
            public static BoundLabelStatement Label(SyntaxNode syntax, BoundLabel label)
                => new(syntax, label);

            public static BoundNopStatement Nop(SyntaxNode syntax)
                => new(syntax);

            public static BoundAssignmentExpression Assignment(SyntaxNode syntax, VariableSymbol variable, BoundExpression expression)
                => new(syntax, variable, expression);
    
            public static BoundFieldAssignmentExpression Assignment(SyntaxNode syntax, BoundExpression structInstance, VariableSymbol structMember, BoundExpression expression)
                => new(syntax, structInstance, structMember, expression);
    
            public static BoundBinaryExpression Binary(SyntaxNode syntax, BoundExpression left, SyntaxKind kind, BoundExpression right)
            {
                var op = BoundBinaryOperator.Bind(kind, left.Type, right.Type)!;
                return Binary(syntax, left, op, right);
            }
    
            public static BoundBinaryExpression Binary(SyntaxNode syntax, BoundExpression left, BoundBinaryOperator op, BoundExpression right)
                => new(syntax, left, op, right);

            public static BoundBinaryExpression Add(SyntaxNode syntax, BoundExpression left, BoundExpression right)
                => Binary(syntax, left, SyntaxKind.PlusToken, right);
    
            public static BoundBinaryExpression LessOrEqual(SyntaxNode syntax, BoundExpression left, BoundExpression right)
                => Binary(syntax, left, SyntaxKind.LessOrEqualsToken, right);
    
            public static BoundExpressionStatement Increment(SyntaxNode syntax, BoundVariableExpression variable)
            {
                var increment = Add(syntax, variable, Literal(syntax, 1));
                var incrementAssign = new BoundAssignmentExpression(syntax, variable.Variable, increment);
                return new BoundExpressionStatement(syntax, incrementAssign);
            }
    
            public static BoundUnaryExpression Not(SyntaxNode syntax, BoundExpression condition)
            {
                Debug.Assert(condition.Type == TypeSymbol.Bool);
    
                var op = BoundUnaryOperator.Bind(SyntaxKind.BangToken, TypeSymbol.Bool);
                Debug.Assert(op != null);
                return new BoundUnaryExpression(syntax, op, condition);
            }
    
            public static BoundVariableExpression Variable(SyntaxNode syntax, BoundVariableDeclaration variable)
                => Variable(syntax, variable.Variable);

            public static BoundVariableExpression Variable(SyntaxNode syntax, VariableSymbol variable)
                => new(syntax, variable);
            
            public static BoundFieldAccessExpression Field(SyntaxNode syntax, BoundExpression structInstance, VariableSymbol structMember)
                => new(syntax, structInstance, structMember);
    
            public static BoundLiteralExpression Literal(SyntaxNode syntax, object literal)
            {
                Debug.Assert(literal is string || literal is bool || literal is int);
                return new BoundLiteralExpression(syntax, literal);
            }
        }
}