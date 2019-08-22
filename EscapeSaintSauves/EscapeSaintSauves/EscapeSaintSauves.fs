// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace EscapeSaintSauves

open System.Diagnostics
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms
open System

module Clue =
    type AnswerInputType =
    | TextAnswer
    | NumberAnswer

    type Clue = {
        buttonText: string
        inputType: AnswerInputType
        questionText: string
        correctAnswer: string
        textClue: string option
        imageClue: ImageSource option
        soundClue: string option
        videoClue: string option
    }

    let clues =
        [
            {
                buttonText="Lastname"
                inputType=AnswerInputType.TextAnswer
                questionText="Give me a lastname!!!"
                correctAnswer="Toto"
                textClue=Some "Here is my clue for you!"
                imageClue=None
                soundClue=None
                videoClue=None
            };
            {
                buttonText="Firstname"
                inputType=AnswerInputType.TextAnswer
                questionText="Give me a firstname!!!"
                correctAnswer="Jc"
                textClue=None
                imageClue=Some (ImageSource.FromResource("EscapeSaintSauves.XamarinLogo.png"))
                soundClue=None
                videoClue=None
            };
            {
                buttonText="Date"
                inputType=AnswerInputType.NumberAnswer
                questionText="Give me a date!!!"
                correctAnswer="12"
                textClue=None
                imageClue=None
                soundClue=None
                videoClue=Some "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ForBiggerBlazes.mp4"
            }
        ]

module App =
    open MediaManager
    open MediaManager.Video
    open System.Reflection

    type PageId =
    | HomePageId
    | InputPageId
    | ResultPageId
    | ErrorPageId

    type Model = {
        Page: PageId
        Clue: Clue.Clue option
        Proposition: string option
    }

    type Msg =
        | GoTo of PageId
        | GoToInput of Clue.Clue
        | PropositionChanged of string
        | CheckSolution

    let initModel = {
        Page = HomePageId
        Clue = None
        Proposition = None
    }

    let init () = initModel, Cmd.none

    let rec update msg model =
        match msg with
        | GoTo pageId ->
            let prop = if model.Page = InputPageId then model.Proposition else None
            { model with Proposition = prop ; Page = pageId }, Cmd.none
        | GoToInput clue ->
            update (GoTo InputPageId) { model with Clue = Some clue }
        | PropositionChanged proposition ->
            { model with Proposition = Some proposition }, Cmd.none
        | CheckSolution ->
            let page = if model.Proposition.IsSome && model.Proposition.Value = model.Clue.Value.correctAnswer then ResultPageId else ErrorPageId
            update (GoTo page) model

    let view (model: Model) dispatch =
        let header =
            View.StackLayout(
                orientation = StackOrientation.Horizontal,
                padding = 20.0,
                verticalOptions = LayoutOptions.Start,
                children = [
                    yield View.Button(
                        text = "ApplicationName",
                        command = (fun () -> dispatch (GoTo HomePageId)),
                        horizontalOptions = LayoutOptions.Center,
                        backgroundColor = Color.Blue
                    )
                    if model.Page <> PageId.HomePageId then
                        yield View.ImageButton(
                            source = ImageSource.FromResource("EscapeSaintSauves.icons8-delete-64.png"),
                            clicked = (fun (_) -> dispatch (GoTo HomePageId)),
                            horizontalOptions = LayoutOptions.EndAndExpand,
                            backgroundColor = Color.Yellow
                        )
                ],
                backgroundColor = Color.Orange
            )
        let body =
            match model.Page with
            | HomePageId ->
                View.StackLayout(
                    padding = 20.0,
                    verticalOptions = LayoutOptions.CenterAndExpand,
                    children = [
                        for clue in Clue.clues do
                            yield View.Button(
                                text = clue.buttonText,
                                command = (fun () -> dispatch (GoToInput clue)),
                                horizontalOptions = LayoutOptions.Center
                            )
                    ],
                    backgroundColor = Color.Red
                )
            | InputPageId ->
                let inputRef = ViewRef<Entry>()
                let entry = 
                        View.Entry(
                            placeholder = model.Clue.Value.questionText,
                            textChanged=(fun event -> dispatch (PropositionChanged event.NewTextValue)),
                            horizontalOptions = LayoutOptions.Center,
                            ref = inputRef,
                            keyboard =
                                match model.Clue.Value.inputType with
                                | Clue.AnswerInputType.NumberAnswer -> Keyboard.Numeric
                                | Clue.AnswerInputType.TextAnswer -> Keyboard.Text
                        )
                //inputRef.Value.Focus() |> ignore
                View.StackLayout(
                    padding = 20.0,
                    verticalOptions = LayoutOptions.Center,
                    children = [
                        entry
                        View.Button(
                            text = "Propose solution",
                            command = (fun () -> dispatch CheckSolution),
                            horizontalOptions = LayoutOptions.Center
                        )
                    ]
                )
            | ResultPageId ->
                View.StackLayout(
                    padding = 20.0,
                    verticalOptions = LayoutOptions.Center,
                    children = [
                        yield View.Label(
                            text = "Correct answer",
                            horizontalOptions = LayoutOptions.Center
                        )
                        if model.Clue.Value.textClue.IsSome then
                            yield View.Label(
                                text = model.Clue.Value.textClue.Value,
                                horizontalOptions = LayoutOptions.Center
                            )
                        if model.Clue.Value.imageClue.IsSome then
                            yield View.Image(
                                source = model.Clue.Value.imageClue.Value,
                                horizontalOptions = LayoutOptions.Center
                            )
                        if model.Clue.Value.videoClue.IsSome then
                            //yield View.VideoView(
                            //        source = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
                            //        showControls = false,
                            //        heightRequest = 500.,
                            //        widthRequest = 200.
                            //        )
                            yield View.VideoView(
                                source = model.Clue.Value.videoClue.Value,
                                showControls = true,
                                heightRequest = 500.,
                                widthRequest = 200.,
                                minimumHeightRequest = 1000.,
                                minimumWidthRequest = 2000.
                            )
                        yield View.Button(
                            text = "Ok",
                            command = (fun () -> dispatch (GoTo HomePageId)),
                            horizontalOptions = LayoutOptions.Center
                        )
                    ]
                )
            | ErrorPageId ->
                View.StackLayout(
                    children = [
                        View.Label(
                            text = "Wrong answer",
                            horizontalOptions = LayoutOptions.Center,
                            horizontalTextAlignment=TextAlignment.Center
                        )
                        View.Button(
                            text = "Go to home",
                            command = (fun () -> dispatch (GoTo HomePageId)),
                            horizontalOptions = LayoutOptions.Center
                        )
                    ],
                    verticalOptions = LayoutOptions.CenterAndExpand
                )

        View.ContentPage(
            content = View.StackLayout(
                padding = 20.0,
                verticalOptions = LayoutOptions.FillAndExpand,
                children = [
                    header
                    body
                ],
                backgroundColor = Color.Green
            )
        )

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App () as app = 
    inherit Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    // Uncomment this line to enable live update in debug mode. 
    // See https://fsprojects.github.io/Fabulous/tools.html for further  instructions.
    //
    //do runner.EnableLiveUpdate()
#endif    

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/models.html for further  instructions.
#if APPSAVE
    let modelId = "model"
    override __.OnSleep() = 

        let json = Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)
        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() = 
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try 
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) -> 

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)
                let model = Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel (model, Cmd.none)

            | _ -> ()
        with ex -> 
            App.program.onError("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = 
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif


