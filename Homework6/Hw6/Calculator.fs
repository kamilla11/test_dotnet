module Hw6.Calculator

open System

type CalculatorOperation =
    | Plus = 0
    | Minus = 1
    | Multiply = 2
    | Divide = 3

[<Literal>]
//let plus = "+"
let plus = "Plus"

[<Literal>]
// let minus = "-"
let minus = "Minus"

[<Literal>]
// let multiply = "*"
let multiply = "Multiply"

[<Literal>]
// let divide = "/"
let divide = "Divide"

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline calculate value1 operation value2 : 'a =
    match operation with
    | CalculatorOperation.Plus -> value1 + value2
    | CalculatorOperation.Minus -> value1 - value2
    | CalculatorOperation.Divide -> value1 / value2
    | CalculatorOperation.Multiply -> value1 * value2