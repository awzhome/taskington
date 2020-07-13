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

    for exec in planExecutions do
        exec.Status.PropertyChanged.AddHandler (PropertyChangedEventHandler(fun o e ->
            printfn "[%s %d%%] %s" exec.BackupPlan.Name exec.Status.Progress exec.Status.StateText
        ))

    UI.menu "PPBackup" (planExecutions
        |> Seq.map(fun exec ->
            (exec.BackupPlan.Name, fun() -> exec.Execute())))

    0
