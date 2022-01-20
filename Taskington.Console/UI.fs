namespace PPBackup.Console

open System

module UI =
    let public clear() =
        Console.Clear()

    let public read() =
        Console.ReadLine()

    let public line() = 
        printfn "========================================================"

    let public margin() =
        printfn ""

    let public wideline() = 
        margin()
        line()
        margin()

    let public head (title : string) =
        margin()
        line()
        printfn "  %s" title
        line()
        margin()

    let public emptyline count =
        for i = 1 to count do
            printfn "%s" (new String(' ', Console.BufferWidth))

    let public getCursorPos() =
        (Console.CursorLeft, Console.CursorTop)

    let public setCursorPos pos =
        Console.SetCursorPosition((fst pos), (snd pos))

    let private menuloop (title : string) (entries : seq<string * (unit -> unit)>) =
        let i = ref 0
        let entriesmap =
            entries
                |> Seq.map(fun e ->
                    incr i
                    let text, callback = e
                    (i.contents.ToString(), (text, callback))
                    )
                |> Map.ofSeq
        
        head title
        entriesmap |> Map.iter (fun key value ->
            printfn "[%s] %s" key (fst value))
        margin()
        printfn "[Enter] Exit menu"
        printf "? "
        let menukey = read()
        let stayInMenu =
            match menukey with
            | "" -> false
            | _ ->
                let foundMenuItem = entriesmap |> Map.tryFind (menukey.ToUpper())
                margin()
                match foundMenuItem with
                | Some e -> 
                    (snd e)()
                | None ->
                    printfn "!!! Wrong input"
                margin()
                true
        stayInMenu

    let public menu title entries =
        while menuloop title entries do ()