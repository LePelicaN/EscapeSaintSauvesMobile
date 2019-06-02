﻿namespace Fabulous.DynamicViews

open Fabulous.DynamicViews

[<AutoOpen>]
module VideoManagerExtension =

    open Fabulous.Core
    open Fabulous.DynamicViews
    open MediaManager.Forms
    open MediaManager.Video

    //// Define keys for the possible attributes
    let SourceAttribKey = AttributeKey<_> "VideoManager_Source"
    let VideoAspectAttribKey = AttributeKey<_> "VideoManager_VideoAspect"
    let ShowControlsAttribKey = AttributeKey<_> "VideoManager_ShowControls"

    // Fully-qualified name to avoid extending by mistake
    // another View class (like Xamarin.Forms.View)
    type Fabulous.DynamicViews.View with
        /// Describes a VideoView in the view
        //static member VideoView(?prop1: seq<ViewElement>, ?prop2: bool, ... inherited attributes ... ) =
        static member inline VideoView(
                source: obj, ?videoAspect: VideoAspectMode , ?ShowControls: bool,
                // inherited attributes common to all views
                ?horizontalOptions, ?verticalOptions, ?margin, ?gestureRecognizers, ?anchorX, ?anchorY, ?backgroundColor, ?heightRequest,
                ?inputTransparent, ?isEnabled, ?isVisible, ?minimumHeightRequest, ?minimumWidthRequest, ?opacity,
                ?rotation, ?rotationX, ?rotationY, ?scale, ?style, ?translationX, ?translationY, ?widthRequest,
                ?resources, ?styles, ?styleSheets, ?classId, ?styleId, ?automationId, ?created, ?styleClass
            ) =

            // Count the number of additional attributes
            let attribCount = 1 // source
            let incIfSome attr count = match attr with Some _ -> count + 1 | None -> count
            let attribCount = incIfSome videoAspect attribCount
            let attribCount = incIfSome ShowControls attribCount

            // Populate the attributes of the base element
            //let attribs = ViewBuilders.BuildBASE(attribCount, ... inherited attributes ... )
            let attribs = ViewBuilders.BuildView(attribCount, ?horizontalOptions=horizontalOptions, ?verticalOptions=verticalOptions,
                                       ?margin=margin, ?gestureRecognizers=gestureRecognizers, ?anchorX=anchorX, ?anchorY=anchorY,
                                       ?backgroundColor=backgroundColor, ?heightRequest=heightRequest, ?inputTransparent=inputTransparent,
                                       ?isEnabled=isEnabled, ?isVisible=isVisible, ?minimumHeightRequest=minimumHeightRequest,
                                       ?minimumWidthRequest=minimumWidthRequest, ?opacity=opacity, ?rotation=rotation,
                                       ?rotationX=rotationX, ?rotationY=rotationY, ?scale=scale, ?style=style,
                                       ?translationX=translationX, ?translationY=translationY, ?widthRequest=widthRequest,
                                       ?resources=resources, ?styles=styles, ?styleSheets=styleSheets, ?classId=classId, ?styleId=styleId,
                                       ?automationId=automationId, ?created=created, ?styleClass=styleClass)

            // Add our own attributes.
            attribs.Add(SourceAttribKey, source)
            let addIfSome (attribs: AttributesBuilder) prop key = match prop with None -> () | Some v -> attribs.Add(key, v)
            addIfSome attribs videoAspect VideoAspectAttribKey
            addIfSome attribs ShowControls ShowControlsAttribKey

            // The creation method
            let create () = new VideoView()

            // The incremental update method
            let update (prev: ViewElement voption) (source: ViewElement) (target: VideoView) =
                ViewBuilders.UpdateView(prev, source, target)
                source.UpdatePrimitive(prev, target, SourceAttribKey, (fun target v -> target.Source <- v))
                source.UpdatePrimitive(prev, target, VideoAspectAttribKey, (fun target v -> target.VideoAspect <- v))
                source.UpdatePrimitive(prev, target, ShowControlsAttribKey, (fun target v -> target.ShowControls <- v))

            ViewElement.Create<VideoView>(create, update, attribs)

