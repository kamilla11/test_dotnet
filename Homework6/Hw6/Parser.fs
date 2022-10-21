module Hw6.Parser

open System
open Hw6.Calculator
open Hw6.MaybeBuilder
open System.Globalization

let isArgLengthSupported (args: string []) : Result<string [], Message> =
    match args.Length with
    | 3 -> Ok args
    | _ -> Error Message.WrongArgLength

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation, arg2) : Result<'a * CalculatorOperation * 'b, Message> =
    match operation with
    | Calculator.plus -> Ok(arg1, CalculatorOperation.Plus, arg2)
    | Calculator.minus -> Ok(arg1, CalculatorOperation.Minus, arg2)
    | Calculator.multiply -> Ok(arg1, CalculatorOperation.Multiply, arg2)
    | Calculator.divide -> Ok(arg1, CalculatorOperation.Divide, arg2)
    | _ -> Error Message.WrongArgFormatOperation

let doubleTryparse (value: string) =
    match Double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture) with
    | true, arg -> Ok arg
    | _ -> Error Message.WrongArgFormat

let parseArgs (args: string []) =
    let result =
        maybe {
            let! arg1 = doubleTryparse (args[0])
            let! arg2 = doubleTryparse (args[2])
            return (arg1, args[1], arg2)
        }

    result


[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isDividingByZero (arg1, operation, arg2) =
    match operation, arg2 with
    | CalculatorOperation.Divide, 0.0 -> Error Message.DivideByZero
    | _ -> Ok(arg1, operation, arg2)

let parseCalcArguments (args: string []) =
    let result =
        maybe {
        let! a = args |> isArgLengthSupported
        let! b = a |> parseArgs
        let! c = b |> isOperationSupported
        let! result = c |> isDividingByZero
        return result
    }
        
    match result with
    | Ok options -> Ok $"{options |||> calculate}"
    | Error message -> Error $"{message}"