open PPBackup.Console
open Taskington.Base
open Taskington.Base.Plans
open Taskington.Base.Config
open Taskington.Base.TinyBus

[<EntryPoint>]
let main argv =
    let application = Application()
    application.Load()
    InitializeConfigurationMessage().Publish();

    let mutable cursorPos = UI.getCursorPos()
    let mutable planName = ""
    let mutable progressState = 0
    let mutable hasErrorsState = false
    let mutable statusTextState = ""
    let mutable isRunningState = false

    let initProgress() =
        progressState <- 0
        hasErrorsState <- false
        statusTextState <- ""

    let updateProgress() =
        if isRunningState then
            UI.setCursorPos cursorPos
            UI.emptyline 4
            UI.setCursorPos cursorPos
            printfn "[%d%%] %s" progressState planName
            if hasErrorsState then
                printfn "ERROR: %s" statusTextState
            else
                printfn "%s" statusTextState

    PlanMessages.PlanIsRunningUpdated.Subscribe (fun plan isRunning ->
        if isRunning then
            initProgress()
            planName <- plan.Name
        isRunningState <- isRunning)
    PlanMessages.PlanProgressUpdated.Subscribe (fun plan progress ->
        progressState <- progress
        updateProgress())
    PlanMessages.PlanHasErrorsUpdated.Subscribe (fun plan hasErrors errorText ->
        hasErrorsState <- hasErrors
        statusTextState <- errorText
        updateProgress())
    PlanMessages.PlanStatusTextUpdated.Subscribe (fun plan statusText ->
        statusTextState <- statusText
        updateProgress())

    let plans = GetPlansMessage().RequestMany()
    UI.menu "Taskington" (plans
        |> Seq.map(fun plan ->
            (plan.Name, fun() ->
                cursorPos <- UI.getCursorPos()
                PlanMessages.ExecutePlan.Push(plan)
            )))

    0
