open System
open PPBackup.Console
open Taskington.Base
open Taskington.Base.Plans
open Taskington.Base.Config
open Taskington.Base.TinyBus

[<EntryPoint>]
let main argv =
    let application = Application()
    application.Load()
    ConfigurationEvents.InitializeConfiguration.Push();

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

    PlanEvents.PlanIsRunningUpdated.Subscribe (fun plan isRunning ->
        if isRunning then
            initProgress()
            planName <- plan.Name
        isRunningState <- isRunning)
    PlanEvents.PlanProgressUpdated.Subscribe (fun plan progress ->
        progressState <- progress
        updateProgress())
    PlanEvents.PlanHasErrorsUpdated.Subscribe (fun plan hasErrors errorText ->
        hasErrorsState <- hasErrors
        statusTextState <- errorText
        updateProgress())
    PlanEvents.PlanStatusTextUpdated.Subscribe (fun plan statusText ->
        statusTextState <- statusText
        updateProgress())

    let plans = ConfigurationEvents.GetPlans.RequestMany()
    UI.menu "Taskington" (plans
        |> Seq.map(fun plan ->
            (plan.Name, fun() ->
                cursorPos <- UI.getCursorPos()
                PlanEvents.ExecutePlan.Push(plan)
            )))

    0
