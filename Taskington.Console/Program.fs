open System
open PPBackup.Console
open Taskington.Base
open Taskington.Base.Plans

[<EntryPoint>]
let main argv =
    let application = Application()
    application.Start()

    let executablePlans = application.ServiceProvider.Get<seq<ExecutablePlan>>()

    let mutable cursorPos = UI.getCursorPos()
    let mutable planName = ""
    let mutable progress = 0
    let mutable hasErrors = false
    let mutable statusText = ""
    let mutable isRunning = false

    let initProgress() =
        progress <- 0
        hasErrors <- false
        statusText <- ""

    let updateProgress() =
        if isRunning then
            UI.setCursorPos cursorPos
            UI.emptyline 4
            UI.setCursorPos cursorPos
            printfn "[%d%%] %s" progress planName
            if hasErrors then
                printfn "ERROR: %s" statusText
            else
                printfn "%s" statusText

    for plan in executablePlans do
        plan.Events.IsRunning.AddHandler (fun o e ->
            if e.IsRunning then
                initProgress()
                planName <- e.Plan.Name
            isRunning <- e.IsRunning)
        plan.Events.Progress.AddHandler (fun o e ->
            progress <- e.Progress
            updateProgress())
        plan.Events.HasErrors.AddHandler (fun o e ->
            hasErrors <- e.HasErrors
            statusText <- e.StatusText
            updateProgress())
        plan.Events.StatusText.AddHandler (fun o e ->
            statusText <- e.StatusText
            updateProgress())

    UI.menu "PPBackup" (executablePlans
        |> Seq.map(fun plan ->
            (plan.Plan.Name, fun() ->
                cursorPos <- UI.getCursorPos()
                plan.Execution.ExecuteAsync().Wait()
            )))

    0
