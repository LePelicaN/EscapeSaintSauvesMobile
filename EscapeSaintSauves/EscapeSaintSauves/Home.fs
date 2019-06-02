namespace Home

open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms

module Types = 
    type Msg = None
    type Model = int

module State =
    let init (s : Types.Model) = s, Cmd.none
    let update msg model = model, Cmd.none

module View =
    open Types

    let root (model: Model) dispatch =
        View.ContentPage(
            content = View.StackLayout(padding = 20.0, verticalOptions = LayoutOptions.Center,
                children = [
                        yield View.Label(text = "Home", horizontalOptions = LayoutOptions.CenterAndExpand)
                    ]
            )).HasNavigationBar(true).HasBackButton(true)

