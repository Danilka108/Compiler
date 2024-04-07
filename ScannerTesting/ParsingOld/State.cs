using System.Collections.Generic;
using Compiler.Parsing.Lexing;

namespace Compiler.Parsing;

// public readonly struct State : IState
// {
//     public LexemeType? Lexeme { get; init; }
//
//     public bool IsEnd()
//     {
//         return Lexeme == LexemeType.OperatorEnd;
//     }
//
//     public IEnumerable<IState> NextPossibleStates()
//     {
//         return this switch
//         {
//             { Lexeme: null } =>
//             [
//                 new State { Lexeme = LexemeType.ConstKeyword }
//             ],
//             { Lexeme: LexemeType.ConstKeyword } =>
//             [
//                 new State { Lexeme = LexemeType.Identifier }
//             ],
//             { Lexeme: LexemeType.Identifier } =>
//             [
//                 new State { Lexeme = LexemeType.Colon }
//             ],
//             { Lexeme: LexemeType.Colon } =>
//             [
//                 new State { Lexeme = LexemeType.Ampersand }
//             ],
//             { Lexeme: LexemeType.Ampersand } =>
//             [
//                 new State { Lexeme = LexemeType.StrKeyword }
//             ],
//             { Lexeme: LexemeType.StrKeyword } =>
//             [
//                 new State { Lexeme = LexemeType.AssignmentOperator }
//             ],
//             { Lexeme: LexemeType.AssignmentOperator } =>
//             [
//                 new State { Lexeme = LexemeType.StringLiteral }
//             ],
//             { Lexeme: LexemeType.StringLiteral } =>
//             [
//                 new State { Lexeme = LexemeType.OperatorEnd }
//             ],
//             _ => []
//         };
//     }
//
//     public bool IsBoundaryLexeme(LexemeType lexeme)
//     {
//         return false;
//         // switch (lexeme) {}
//     }
//
//     public bool IsInvalidLexeme(LexemeType lexeme)
//     {
//         return lexeme is LexemeType.UnexpectedSymbol or LexemeType.UnterminatedStringLiteral;
//     }
//
//
//     public bool IsIgnorableLexeme(LexemeType lexeme)
//     {
//         return lexeme is LexemeType.Separator;
//     }
// }
//
// // public class StateFoobar
// // {
// //     public bool IsIgnorableLexeme(LexemeType lexeme)
// //     {
// //         return lexeme is LexemeType.Separator;
// //     }
// //
// //     public bool IsInvalidLexeme(LexemeType lexeme)
// //     {
// //         return lexeme is LexemeType.UnexpectedSymbol or LexemeType.UnterminatedStringLiteral;
// //     }
// //
// //     public static State?
// //
// //     public static State? TryMoveNext(State state, LexemeType nextLexeme)
// //     {
// //         State? nextState = GetNextPossibleStates(state)
// //             .FirstOrDefault(possibleState => possibleState.Lexeme == nextLexeme);
// //
// //         if (nextState is not null && nextState.Value.Lexeme == nextLexeme)
// //         {
// //             return nextState.Value;
// //         }
// //
// //         return null;
// //     }
// //
// //     public static State MoveNext(State state)
// //     {
// //         State? nextState = GetNextPossibleStates(state).FirstOrDefault();
// //         return nextState ?? state;
// //     }
// //
// //     public static State[] GetNextPossibleStates(State state)
// //     {
// //         return state switch
// //         {
// //             { Lexeme: null } =>
// //             [
// //                 new State { Lexeme = LexemeType.ConstKeyword }
// //             ],
// //             { Lexeme: LexemeType.ConstKeyword } =>
// //             [
// //                 new State { Lexeme = LexemeType.Identifier }
// //             ],
// //             { Lexeme: LexemeType.Identifier } =>
// //             [
// //                 new State { Lexeme = LexemeType.Colon }
// //             ],
// //             { Lexeme: LexemeType.Colon } =>
// //             [
// //                 new State { Lexeme = LexemeType.Ampersand }
// //             ],
// //             { Lexeme: LexemeType.Ampersand } =>
// //             [
// //                 new State { Lexeme = LexemeType.StrKeyword }
// //             ],
// //             { Lexeme: LexemeType.StrKeyword } =>
// //             [
// //                 new State { Lexeme = LexemeType.AssignmentOperator }
// //             ],
// //             { Lexeme: LexemeType.AssignmentOperator } =>
// //             [
// //                 new State { Lexeme = LexemeType.StringLiteral }
// //             ],
// //             { Lexeme: LexemeType.StringLiteral } =>
// //             [
// //                 new State { Lexeme = LexemeType.OperatorEnd }
// //             ],
// //             _ => []
// //         };
// //     }
// //
// //     // public LexemeType[] GetBoundaryLexeme(State state)
// //     // {
// //     //     return state switch
// //     //     {
// //     //         { Lexeme: LexemeType.ConstKeyword } => [LexemeType.Identifier],
// //     //         { Lexeme: LexemeType.Identifier } => [LexemeType.]
// //     //         { Lexeme: LexemeType.Colon } =>
// //     //         { Lexeme: LexemeType.Ampersand } =>
// //     //         { Lexeme: LexemeType.StrKeyword } =>
// //     //         { Lexeme: LexemeType.AssignmentOperator } =>
// //     //         { Lexeme: LexemeType.StringLiteral } =>
// //     //         _ => []
// //     //     };
// //     // }
// // }