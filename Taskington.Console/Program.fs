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

    PlanRunningUpdateMessage.Subscribe (fun message ->
        if message.IsRunning then
            initProgress()
            planName <- message.Plan.Name
        isRunningState <- message.IsRunning)
    PlanProgressUpdateMessage.Subscribe (fun message ->
        progressState <- message.Progress
        updateProgress())
    PlanErrorUpdateMessage.Subscribe (fun message ->
        hasErrorsState <- message.HasErrors
        statusTextState <- message.ValidationText
        updateProgress())
    PlanStatusTextUpdateMessage.Subscribe (fun message ->
        statusTextState <- message.StatusText
        updateProgress())

    let plans = GetPlansMessage().RequestMany()
    UI.menu "Taskington" (plans
        |> Seq.map(fun plan ->
            (plan.Name, fun() ->
                cursorPos <- UI.getCursorPos()
                ExecutePlanMessage(plan).Publish()
            )))

    0
