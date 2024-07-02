#r "../../_lib/Fornax.Core.dll"
#if !FORNAX
#load "../../loaders/researchloader.fsx"
#endif

open Html

let private SliderHeight = "fit-content"
let private createModalId(slide: Researchloader.FrameSlide) =
  sprintf "More-Content-Modal-%i" slide.Index


let private createResearchModal(slide: Researchloader.FrameSlide) =
  let id = createModalId slide
  // Bulma Modal
  div [Class "modal"; HtmlProperties.Id id] [
    div [Class "modal-background"] []
    div [Class "modal-content"] [
      div [Class "box"] [
        div [Class "content"] [
          !!slide.MainContent
          match slide.MoreContent with
          | Some c ->
            !!c
          | None -> ()
        ]
      ]
    ]
    button [Class "modal-close is-large"; HtmlProperties.Custom("aria-label", "close")] []
  ]


let private createMoreContentModalButton (slide: Researchloader.FrameSlide) =
  let id = createModalId slide
  div [] [
    // Trigger Button
    button [Class "button is-primary is-small js-modal-trigger"; HtmlProperties.Custom("data-target", id)] [
      !!"More"
    ]
  ]

let private createFrameSlide (slide: Researchloader.FrameSlide) =
  let innerFlexBoxContainer = HtmlProperties.Style [Display "flex"; CSSProperties.Height "100%"; AlignItems "center"]
  li [Class "splide__slide"] [
      // container
      div [Class "is-flex fixed-grid research__bg has-1-cols-mobile"; HtmlProperties.Style [CSSProperties.Height "100%"; AlignItems "center"; JustifyContent "center"]] [
          div [Class "grid research__grid"; HtmlProperties.Style [CSSProperties.Custom("justify-items", "end"); CSSProperties.Height "fit-content"; CSSProperties.MaxHeight SliderHeight]] [
              div [Class "cell research__cell"; HtmlProperties.Style [CSSProperties.Width "100%"; CSSProperties.MaxWidth "unset"]] [
                div [innerFlexBoxContainer] [
                  div [] [
                    img [
                      match slide with
                      | { Image = Some i} -> Src i
                      | { ImageUrl = Some i} -> Src i
                      | _ -> ()
                      HtmlProperties.Style [MaxWidth "100%"]
                    ]
                  ]
                ]
              ]
              div [Class "cell research__cell"] [
                div [innerFlexBoxContainer] [
                  div [Class "content research__content"] [
                      !!slide.MainContent
                      match slide.MoreContent with
                      | Some _ ->
                          createMoreContentModalButton(slide)
                      | None -> ()
                  ]
                ]
              ]
          ]
      ]
  ]

let generate (ctx : SiteContents) (_: string) =
    let researchList: Researchloader.FrameSlide list =
        ctx.TryGetValues<Researchloader.FrameSlide> ()
        |> Option.defaultValue Seq.empty
        |> Seq.toList
    div [HtmlProperties.Style [CSSProperties.Height SliderHeight]] [
        section [Class "splide"; HtmlProperties.Custom("aria-label","current research information")] [
            // div [Class "splide__slider"] [
            // ]
            div [Class "splide__track"] [
                ul [Class "splide__list"] [
                    for slide in researchList do
                      createFrameSlide slide
                ]
            ]
            div [Class "splide__progress"] [
                div [Class "splide__progress__bar"] []
            ]
            // https://github.com/Splidejs/splide/issues/1310
            button [Class "splide__toggle"; Type "button"] [
                svg [Class "splide__toggle__play"; HtmlProperties.Custom("viewBox","0 0 24 24"); HtmlProperties.Custom("xmlns", "http://www.w3.org/2000/svg")] [
                    path  [HtmlProperties.Custom("d", "m22 12-20 11v-22l10 5.5z")] []
                ]
                svg [Class "splide__toggle__pause"; HtmlProperties.Custom("viewBox","0 0 24 24"); HtmlProperties.Custom("xmlns", "http://www.w3.org/2000/svg")] [
                    path  [HtmlProperties.Custom("d", "m2 1v22h7v-22zm13 0v22h7v-22z")] []
                ]
            ]
        ]
        script [] [
            !!"""document.addEventListener( 'DOMContentLoaded', function() {
    var splide = new Splide( '.splide', { type: 'loop', autoplay: false } );
    splide.mount();
} );"""
        ]
        section [] [
          for researchSlide in researchList do
            createResearchModal researchSlide
        ]
    ]
