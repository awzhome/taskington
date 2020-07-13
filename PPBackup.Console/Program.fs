open System
open PPBackup.Console
open PPBackup.Base
open PPBackup.Base.Executors
open System.ComponentModel

[<EntryPoint>]
let main argv =
    let application = Application()
    application.Start()

    let planExecutions = application.Services.Get<seq<IPlanExecution>>()

    let mutable cursorPos = UI.getCursorPos()

    for exec in planExecutions do
        exec.Status.PropertyChanged.AddHandler (PropertyChangedEventHandler(fun o e ->
            UI.setCursorPos cursorPos
            UI.emptyline 4
            UI.setCursorPos cursorPos
            printfn "[%d%%] %s" exec.Status.Progress exec.BackupPlan.Name
            printfn "%s" exec.Status.StateText
        ))

    UI.menu "PPBackup" (planExecutions
        |> Seq.map(fun exec ->
            (exec.BackupPlan.Name, fun() ->
                cursorPos <- UI.getCursorPos()
                exec.Execute()
            )))

    0
